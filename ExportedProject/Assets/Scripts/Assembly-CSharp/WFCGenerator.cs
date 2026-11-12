using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
	public List<WFCModule> modules = new List<WFCModule>();

	public WFCModule contradictionModule;

	private List<WFCModule> builtModules = new List<WFCModule>();

	public float moduleScale = 2f;

	public int width = 10;

	public int height = 10;

	private WFCSlot[,] slots;

	public NavMeshSurface navMeshSurface;

	public Vector2Int startingTile;

	private const string GAMEOBJECT_MODULES = "Modules";

	public WFCMask borderMask = new WFCMask("222");

	private WFCMask voidMask = new WFCMask("222");

	private WFCMask emptyMask = new WFCMask("000");

	private WFCMask wildcardMask = new WFCMask("???");

	public const int SEGEMENTS_PER_MODULE = 3;

	private void Start()
	{
		Generate();
	}

	private void Update()
	{
	}

	[ContextMenu("WFC/Generate")]
	public void Generate()
	{
		startingTile = new Vector2Int(width / 2, height / 2);
		Clear();
		BuildModules();
		InitializeGrid();
		ForceStartPosition();
		if (RunWFC())
		{
			OpenUnreachableRooms();
			CloseUnreachableRooms();
			OpenUnreachableRooms();
			ChangeModule(startingTile, GetFittingModules(emptyMask, emptyMask, emptyMask, emptyMask)[0], 1);
			InstantiateResult();
		}
		else
		{
			Debug.Log("WFC failed to find solution on first pass!");
		}
		navMeshSurface.BuildNavMesh();
	}

	[ContextMenu("WFC/Clear")]
	private void Clear()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
		builtModules.Clear();
	}

	private void BuildModules()
	{
		GameObject gameObject = new GameObject("Modules");
		gameObject.transform.parent = base.transform;
		foreach (WFCModule module in modules)
		{
			if (module.rotatable)
			{
				for (int i = 0; i < 4; i++)
				{
					WFCModule component = Object.Instantiate(module, gameObject.transform).GetComponent<WFCModule>();
					component.transform.rotation = Quaternion.Euler(0f, 90 * i, 0f);
					component.name = component.name + "_R" + 90 * i;
					for (int j = 0; j < i; j++)
					{
						component.RotateMasksClockwise();
					}
					component.gameObject.SetActive(value: false);
					builtModules.Add(component);
				}
			}
			else
			{
				WFCModule component2 = Object.Instantiate(module, gameObject.transform).GetComponent<WFCModule>();
				component2.gameObject.SetActive(value: false);
				builtModules.Add(component2);
			}
		}
	}

	private void InitializeGrid()
	{
		slots = new WFCSlot[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				slots[i, j] = new WFCSlot(builtModules);
				ResetSlot(i, j);
			}
		}
	}

	private List<WFCModule> GetFittingModules(WFCMask up, WFCMask down, WFCMask left, WFCMask right, string middle = "")
	{
		List<WFCModule> list = new List<WFCModule>();
		foreach (WFCModule builtModule in builtModules)
		{
			if (builtModule.up.Fits(up) && builtModule.down.Fits(down) && builtModule.left.Fits(left) && builtModule.right.Fits(right) && (middle == "" || builtModule.middle.Contains(middle)))
			{
				list.Add(builtModule);
			}
		}
		return list;
	}

	private void ForceStartPosition()
	{
		ForceCollapseSlot(startingTile, emptyMask, emptyMask, emptyMask, emptyMask);
		ForceCollapseSlot(startingTile + Vector2Int.up, emptyMask, emptyMask, emptyMask, emptyMask);
		ForceCollapseSlot(startingTile + Vector2Int.down, emptyMask, emptyMask, emptyMask, emptyMask);
		ForceCollapseSlot(startingTile + Vector2Int.left, emptyMask, emptyMask, emptyMask, emptyMask);
		ForceCollapseSlot(startingTile + Vector2Int.right, emptyMask, emptyMask, emptyMask, emptyMask);
	}

	public void ForceCollapseSlot(Vector2Int pos, WFCModule module)
	{
		slots[pos.x, pos.y].possibleModules = new List<WFCModule> { module };
		Propagate(pos);
	}

	public void ForceCollapseSlot(Vector2Int pos, WFCMask up, WFCMask down, WFCMask left, WFCMask right)
	{
		slots[pos.x, pos.y].possibleModules = GetFittingModules(up, down, left, right);
		Collapse(pos);
		Propagate(pos);
	}

	private bool RunWFC()
	{
		while (true)
		{
			Vector2Int? lowestEntropyCell = GetLowestEntropyCell();
			if (!lowestEntropyCell.HasValue)
			{
				break;
			}
			if (slots[lowestEntropyCell.Value.x, lowestEntropyCell.Value.y].possibleModules.Count == 0)
			{
				return false;
			}
			Collapse(lowestEntropyCell.Value);
			Propagate(lowestEntropyCell.Value);
		}
		return true;
	}

	private void Collapse(Vector2Int pos)
	{
		WFCSlot wFCSlot = slots[pos.x, pos.y];
		float num = 0f;
		foreach (WFCModule possibleModule in wFCSlot.possibleModules)
		{
			num += possibleModule.weight;
		}
		float num2 = Random.value * num;
		WFCModule item = GetCollapsedModule(wFCSlot);
		float num3 = 0f;
		foreach (WFCModule possibleModule2 in wFCSlot.possibleModules)
		{
			num3 += possibleModule2.weight;
			if (num2 <= num3)
			{
				item = possibleModule2;
				break;
			}
		}
		wFCSlot.possibleModules = new List<WFCModule> { item };
	}

	private void Propagate(Vector2Int start)
	{
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		queue.Enqueue(start);
		while (queue.Count > 0)
		{
			Vector2Int vector2Int = queue.Dequeue();
			WFCSlot neighbor = slots[vector2Int.x, vector2Int.y];
			(Vector2Int, Direction, Direction)[] directions = DirectionHelper.Directions;
			for (int i = 0; i < directions.Length; i++)
			{
				(Vector2Int, Direction, Direction) tuple = directions[i];
				Vector2Int vector2Int2 = vector2Int + tuple.Item1;
				if (InBounds(vector2Int2))
				{
					WFCSlot wFCSlot = slots[vector2Int2.x, vector2Int2.y];
					int count = wFCSlot.possibleModules.Count;
					UpdatePossibleModules(wFCSlot, neighbor, tuple.Item3);
					if (wFCSlot.possibleModules.Count < count && !queue.Contains(vector2Int2))
					{
						queue.Enqueue(vector2Int2);
					}
				}
			}
		}
	}

	private void UpdatePossibleModules(WFCSlot slot, WFCSlot neighbor, Direction neighbourDirection)
	{
		List<WFCModule> list = new List<WFCModule>();
		foreach (WFCModule possibleModule in slot.possibleModules)
		{
			bool flag = neighbor.possibleModules.Count == 0;
			foreach (WFCModule possibleModule2 in neighbor.possibleModules)
			{
				if (possibleModule.SatisfiesConstraints(possibleModule2, neighbourDirection))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(possibleModule);
			}
		}
		foreach (WFCModule item in list)
		{
			slot.possibleModules.Remove(item);
		}
	}

	private void UpdatePossibleModules(Vector2Int p)
	{
		WFCSlot slot = slots[p.x, p.y];
		(Vector2Int, Direction, Direction)[] directions = DirectionHelper.Directions;
		for (int i = 0; i < directions.Length; i++)
		{
			(Vector2Int, Direction, Direction) tuple = directions[i];
			Vector2Int p2 = p + tuple.Item1;
			if (InBounds(p2))
			{
				UpdatePossibleModules(slot, slots[p2.x, p2.y], tuple.Item2);
			}
		}
	}

	public WFCModule GetCollapsedModule(WFCSlot slot)
	{
		if (slot.possibleModules.Count > 0)
		{
			return slot.possibleModules[0];
		}
		return contradictionModule;
	}

	private bool InBounds(Vector2Int p)
	{
		if (p.x >= 0 && p.x < width && p.y >= 0)
		{
			return p.y < height;
		}
		return false;
	}

	private Vector3 GetInstantiatePosition(int x, int y)
	{
		return base.transform.position + new Vector3(((float)x - (float)width * 0.5f) * moduleScale, 0f, ((float)y - (float)height * 0.5f) * moduleScale);
	}

	public Vector3 GetInstantiatePosition(float x, float y)
	{
		return base.transform.position + new Vector3((x - (float)width * 0.5f) * moduleScale, 0f, (y - (float)height * 0.5f) * moduleScale);
	}

	[ContextMenu("WFC/Instantiate")]
	private void InstantiateResult()
	{
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.name != "Modules")
			{
				Object.Destroy(item.gameObject);
			}
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				WFCModule collapsedModule = GetCollapsedModule(slots[i, j]);
				GameObject obj = Object.Instantiate(collapsedModule.gameObject, GetInstantiatePosition(i, j), collapsedModule.transform.rotation);
				obj.SetActive(value: true);
				obj.transform.parent = base.transform;
			}
		}
	}

	private float CalculateEntropy(WFCSlot slot)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (WFCModule possibleModule in slot.possibleModules)
		{
			num += possibleModule.weight;
			num2 += possibleModule.weight * Mathf.Log(possibleModule.weight);
		}
		return Mathf.Log(num) - num2 / num;
	}

	private Vector2Int? GetLowestEntropyCell()
	{
		float num = 2.1474836E+09f;
		Vector2Int? result = null;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				WFCSlot wFCSlot = slots[i, j];
				if (!wFCSlot.IsCollapsed())
				{
					float num2 = CalculateEntropy(wFCSlot);
					num2 += Random.value * 0.0001f;
					if (num2 < num)
					{
						num = num2;
						result = new Vector2Int(i, j);
					}
				}
			}
		}
		return result;
	}

	private bool HasConnection(WFCModule module, Direction dir)
	{
		if ((dir switch
		{
			Direction.UP => module.up, 
			Direction.DOWN => module.down, 
			Direction.LEFT => module.left, 
			Direction.RIGHT => module.right, 
			_ => null, 
		}).value.Contains('0'))
		{
			return module.middle.Contains('0');
		}
		return false;
	}

	private List<Vector2Int> Inverse(HashSet<Vector2Int> set)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Vector2Int item = new Vector2Int(i, j);
				if (!set.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	[ContextMenu("WFC/Close Rooms")]
	public void CloseRooms()
	{
		CloseUnreachableRooms();
		InstantiateResult();
	}

	public Segmentation GenerateSegmentation()
	{
		Segmentation segmentation = new Segmentation(width * 3, height * 3);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				WFCModule collapsedModule = GetCollapsedModule(slots[j, i]);
				int num = j * 3;
				int num2 = i * 3;
				segmentation.occupied[num, num2] = collapsedModule.down.value[0] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.down.value[1] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.down.value[2] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num = j * 3;
				num2++;
				segmentation.occupied[num, num2] = collapsedModule.left.value[1] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.middle[0] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.right.value[1] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num = j * 3;
				num2++;
				segmentation.occupied[num, num2] = collapsedModule.up.value[0] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.up.value[1] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
				num++;
				segmentation.occupied[num, num2] = collapsedModule.up.value[2] != '0';
				segmentation.segmentId[num, num2] = (segmentation.occupied[num, num2] ? (-1) : (j + i * width * 3));
				segmentation.slotPosition[num, num2] = new Vector2Int(j, i);
			}
		}
		segmentation.CalculateSegmentation();
		return segmentation;
	}

	private HashSet<Vector2Int> CalculateReachable(Vector2Int startingTile)
	{
		Segmentation segmentation = GenerateSegmentation();
		int num = segmentation.segmentId[startingTile.x * 3, startingTile.y * 3];
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
		for (int i = 0; i < segmentation.GetHeight(); i++)
		{
			for (int j = 0; j < segmentation.GetWidth(); j++)
			{
				if (segmentation.segmentId[j, i] == num)
				{
					hashSet.Add(segmentation.slotPosition[j, i]);
				}
			}
		}
		return hashSet;
	}

	private HashSet<Vector2Int> CalculateUnreachable(Vector2Int startingTile)
	{
		Segmentation segmentation = GenerateSegmentation();
		int num = segmentation.segmentId[startingTile.x * 3, startingTile.y * 3];
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
		for (int i = 0; i < segmentation.GetHeight(); i++)
		{
			for (int j = 0; j < segmentation.GetWidth(); j++)
			{
				if (segmentation.segmentId[j, i] != num && segmentation.segmentId[j, i] != -1)
				{
					hashSet.Add(segmentation.slotPosition[j, i]);
				}
			}
		}
		return hashSet;
	}

	private HashSet<Vector2Int> CalculateHalfReachable(Vector2Int startingTile)
	{
		Segmentation segmentation = GenerateSegmentation();
		int num = segmentation.segmentId[startingTile.x * 3, startingTile.y * 3];
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
		HashSet<Vector2Int> hashSet2 = new HashSet<Vector2Int>();
		for (int i = 0; i < segmentation.GetHeight(); i++)
		{
			for (int j = 0; j < segmentation.GetWidth(); j++)
			{
				if (segmentation.segmentId[j, i] == num)
				{
					hashSet.Add(segmentation.slotPosition[j, i]);
				}
				if (segmentation.segmentId[j, i] != num && segmentation.segmentId[j, i] != -1)
				{
					hashSet2.Add(segmentation.slotPosition[j, i]);
				}
			}
		}
		HashSet<Vector2Int> hashSet3 = new HashSet<Vector2Int>();
		foreach (Vector2Int item in hashSet)
		{
			if (hashSet2.Contains(item))
			{
				hashSet3.Add(item);
			}
		}
		return hashSet3;
	}

	private void CloseUnreachableRooms()
	{
		HashSet<Vector2Int> set = CalculateReachable(startingTile);
		List<Vector2Int> list = Inverse(set);
		List<WFCModule> fittingModules = GetFittingModules(voidMask, voidMask, voidMask, voidMask);
		foreach (Vector2Int item in list)
		{
			ChangeModule(item, fittingModules[Random.Range(0, fittingModules.Count)], 1);
		}
	}

	[ContextMenu("WFC/Open Rooms")]
	private void OpenRooms()
	{
		OpenUnreachableRooms();
		InstantiateResult();
	}

	private void OpenUnreachableRooms(int amount = 64)
	{
		for (int i = 0; i < amount; i++)
		{
			if (!OpenUnreachableRoom())
			{
				break;
			}
		}
	}

	private bool OpenUnreachableRoom()
	{
		foreach (Vector2Int item in CalculateHalfReachable(startingTile))
		{
			WFCModule collapsedModule = GetCollapsedModule(slots[item.x, item.y]);
			List<WFCModule> fittingModules = GetFittingModules(collapsedModule.up, collapsedModule.down, collapsedModule.left, collapsedModule.right, "0");
			if (fittingModules.Count > 0)
			{
				ChangeModule(item, fittingModules[Random.Range(0, fittingModules.Count)], 0);
				return true;
			}
		}
		return false;
	}

	private List<Vector2Int> GetRegion(Vector2Int center, int radius)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				Vector2Int vector2Int = center + new Vector2Int(i, j);
				if (InBounds(vector2Int))
				{
					list.Add(vector2Int);
				}
			}
		}
		return list;
	}

	private void ResetSlot(int x, int y)
	{
		WFCMask up = wildcardMask;
		WFCMask down = wildcardMask;
		WFCMask left = wildcardMask;
		WFCMask right = wildcardMask;
		if (x == 0)
		{
			left = borderMask;
		}
		if (x == width - 1)
		{
			right = borderMask;
		}
		if (y == 0)
		{
			down = borderMask;
		}
		if (y == height - 1)
		{
			up = borderMask;
		}
		slots[x, y].possibleModules = GetFittingModules(up, down, left, right);
	}

	private void ResetRegion(List<Vector2Int> region)
	{
		foreach (Vector2Int item in region)
		{
			ResetSlot(item.x, item.y);
		}
	}

	private void PropagateRegion(List<Vector2Int> region, Vector2Int start)
	{
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>(region);
		queue.Enqueue(start);
		while (queue.Count > 0)
		{
			Vector2Int vector2Int = queue.Dequeue();
			(Vector2Int, Direction, Direction)[] directions = DirectionHelper.Directions;
			for (int i = 0; i < directions.Length; i++)
			{
				(Vector2Int, Direction, Direction) tuple = directions[i];
				Vector2Int item = vector2Int + tuple.Item1;
				if (hashSet.Contains(item))
				{
					int count = slots[item.x, item.y].possibleModules.Count;
					UpdatePossibleModules(slots[item.x, item.y], slots[vector2Int.x, vector2Int.y], tuple.Item3);
					if (slots[item.x, item.y].possibleModules.Count < count)
					{
						queue.Enqueue(item);
					}
				}
			}
		}
	}

	private void CollapseRegion(List<Vector2Int> region)
	{
		while (true)
		{
			Vector2Int? vector2Int = null;
			float num = float.MaxValue;
			foreach (Vector2Int item in region)
			{
				WFCSlot wFCSlot = slots[item.x, item.y];
				if (!wFCSlot.IsCollapsed())
				{
					float num2 = CalculateEntropy(wFCSlot);
					num2 += Random.value * 0.0001f;
					if (num2 < num)
					{
						num = num2;
						vector2Int = item;
					}
				}
			}
			if (vector2Int.HasValue)
			{
				Collapse(vector2Int.Value);
				PropagateRegion(region, vector2Int.Value);
				continue;
			}
			break;
		}
	}

	public void ChangeModule(Vector2Int pos, WFCModule module, int radius = 2)
	{
		List<Vector2Int> region = GetRegion(pos, radius);
		ResetRegion(region);
		foreach (Vector2Int item in region)
		{
			UpdatePossibleModules(item);
		}
		ForceCollapseSlot(pos, module);
		CollapseRegion(region);
	}

	private void OnDrawGizmosSelected()
	{
		if (slots == null)
		{
			return;
		}
		float a = 0.5f;
		Color[] array = new Color[6]
		{
			new Color(1f, 0f, 0f, a),
			new Color(0f, 1f, 0f, a),
			new Color(0f, 0f, 1f, a),
			new Color(1f, 0f, 1f, a),
			new Color(0f, 1f, 1f, a),
			new Color(1f, 1f, 0f, a)
		};
		Segmentation segmentation = GenerateSegmentation();
		for (int i = 0; i < segmentation.GetHeight(); i++)
		{
			for (int j = 0; j < segmentation.GetWidth(); j++)
			{
				if (segmentation.segmentId[j, i] >= 0)
				{
					Gizmos.color = array[segmentation.segmentId[j, i] % array.Length];
					Gizmos.DrawCube(GetInstantiatePosition((float)(j - 1) / 3f, (float)(i - 1) / 3f), Vector3.one * 16f / 3f);
				}
			}
		}
	}
}
