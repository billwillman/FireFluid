using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlant : MonoBehaviour
{
	public PlantData plantData;

	public bool useSeed;

	public int seed;

	[Header("Growth Settings")]
	public float branchLength = 0.5f;

	[Range(0f, 1f)]
	public float branchLengthPerDepthFactor = 0.95f;

	public int maxNewBranchesPerStep = 10;

	[Header("Collision Settings")]
	public float minDistance = 0.3f;

	[Header("Space Colonization")]
	public Vector3 directionalBias = Vector3.up;

	public int crownHeight = 4;

	public int attractionPointCount = 200;

	public float attractionRadius = 2.5f;

	public float killRadius = 0.5f;

	public Vector3 attractionAreaSize = new Vector3(6f, 5f, 6f);

	[Header("Trunk Settings")]
	public int trunkDepth = 5;

	public float trunkAttractionBias = 2f;

	[Header("Fruit Generation")]
	public int fruitsPerGrowStep = 1;

	[Header("Tree Lean")]
	public Vector3 trunkLean = new Vector3(0.1f, 0f, 0f);

	public Vector3 branchLean = new Vector3(-0.05f, 0f, 0f);

	private List<AttractionPoint> attractionPoints = new List<AttractionPoint>();

	private ProceduralPlantRenderer rendererRef;

	private void Awake()
	{
		rendererRef = GetComponent<ProceduralPlantRenderer>();
	}

	private void Start()
	{
		if (plantData == null || plantData.branches.Count == 0)
		{
			InitializeTree();
		}
		GenerateAttractionPoints();
	}

	private void InitializeTree()
	{
		plantData = new PlantData();
		if (useSeed)
		{
			plantData.randomSeed = seed;
		}
		else
		{
			plantData.randomSeed = UnityEngine.Random.Range(0, 999999);
		}
		BranchNode item = new BranchNode
		{
			position = Vector3.zero,
			direction = Vector3.up,
			parentIndex = -1,
			depth = 0
		};
		plantData.branches.Add(item);
	}

	private void GenerateAttractionPoints()
	{
		attractionPoints.Clear();
		System.Random random = new System.Random(plantData.randomSeed);
		for (int i = 0; i < attractionPointCount; i++)
		{
			float x = (float)(random.NextDouble() * (double)attractionAreaSize.x - (double)(attractionAreaSize.x / 2f));
			float y = (float)(random.NextDouble() * (double)attractionAreaSize.y + (double)crownHeight);
			float z = (float)(random.NextDouble() * (double)attractionAreaSize.z - (double)(attractionAreaSize.z / 2f));
			attractionPoints.Add(new AttractionPoint
			{
				position = new Vector3(x, y, z)
			});
		}
	}

	[ContextMenu("Grow Step")]
	public void GrowStep()
	{
		new System.Random(plantData.randomSeed + plantData.growthStep);
		Dictionary<int, List<Vector3>> dictionary = new Dictionary<int, List<Vector3>>();
		foreach (AttractionPoint attractionPoint in attractionPoints)
		{
			float num = float.MaxValue;
			int num2 = -1;
			for (int i = 0; i < plantData.branches.Count; i++)
			{
				float num3 = Vector3.Distance(plantData.branches[i].position, attractionPoint.position);
				if (num3 < killRadius)
				{
					attractionPoint.reached = true;
					num2 = -1;
					break;
				}
				if (num3 < attractionRadius && num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				if (!dictionary.ContainsKey(num2))
				{
					dictionary[num2] = new List<Vector3>();
				}
				dictionary[num2].Add(attractionPoint.position);
			}
		}
		List<BranchNode> list = new List<BranchNode>();
		int num4 = 0;
		foreach (KeyValuePair<int, List<Vector3>> item3 in dictionary)
		{
			if (num4 >= maxNewBranchesPerStep)
			{
				break;
			}
			int key = item3.Key;
			BranchNode branchNode = plantData.branches[key];
			_ = branchNode.depth;
			_ = trunkDepth;
			Vector3 vector = Vector3.zero;
			if (item3.Value.Count > 0)
			{
				foreach (Vector3 item4 in item3.Value)
				{
					vector += (item4 - branchNode.position).normalized;
				}
				vector /= (float)item3.Value.Count;
			}
			else
			{
				if (branchNode.depth >= trunkDepth)
				{
					continue;
				}
				vector = branchNode.direction;
			}
			float num5 = Mathf.Clamp01(1f - (float)branchNode.depth / (float)trunkDepth);
			vector = (vector + directionalBias * num5 * trunkAttractionBias).normalized;
			float t = Mathf.Clamp01((float)branchNode.depth / (float)trunkDepth);
			Vector3 vector2 = Vector3.Lerp(trunkLean, branchLean, t);
			vector += vector2;
			vector.Normalize();
			float num6 = branchLength * Mathf.Pow(branchLengthPerDepthFactor, branchNode.depth);
			Vector3 vector3 = branchNode.position + vector * num6;
			bool flag = false;
			foreach (BranchNode branch in plantData.branches)
			{
				if (Vector3.Distance(branch.position, vector3) < minDistance)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				BranchNode item = new BranchNode
				{
					position = vector3,
					direction = vector,
					parentIndex = key,
					depth = branchNode.depth + 1
				};
				branchNode.children.Add(plantData.branches.Count + list.Count);
				list.Add(item);
				num4++;
			}
		}
		if (num4 == 0 && plantData.branches.Count > 0)
		{
			BranchNode branchNode2 = plantData.branches[plantData.branches.Count - 1];
			if (branchNode2.depth < trunkDepth)
			{
				Vector3 normalized = (new Vector3(0f, crownHeight, 0f) - branchNode2.position + directionalBias * 0.5f).normalized;
				normalized += Vector3.Lerp(trunkLean, branchLean, (float)branchNode2.depth / (float)trunkDepth);
				normalized.Normalize();
				float num7 = branchLength * Mathf.Pow(branchLengthPerDepthFactor, branchNode2.depth);
				Vector3 position = branchNode2.position + normalized * num7;
				BranchNode item2 = new BranchNode
				{
					position = position,
					direction = normalized,
					parentIndex = plantData.branches.Count - 1,
					depth = branchNode2.depth + 1
				};
				branchNode2.children.Add(plantData.branches.Count);
				list.Add(item2);
			}
		}
		plantData.branches.AddRange(list);
		attractionPoints.RemoveAll((AttractionPoint p) => p.reached);
		UpdateFruits();
		CalculateBranchMass();
		plantData.growthStep++;
	}

	[ContextMenu("Grow Step x10")]
	public void GrowStep10()
	{
		for (int i = 0; i < 10; i++)
		{
			GrowStep();
		}
	}

	[ContextMenu("Grow Step x100")]
	public void GrowStep100()
	{
		for (int i = 0; i < 100; i++)
		{
			GrowStep();
		}
	}

	private void CalculateBranchMass()
	{
		for (int num = plantData.branches.Count - 1; num >= 0; num--)
		{
			BranchNode branchNode = plantData.branches[num];
			if (branchNode.children.Count == 0)
			{
				branchNode.mass = 1f;
			}
			else
			{
				float num2 = 0f;
				foreach (int child in branchNode.children)
				{
					num2 += plantData.branches[child].mass;
				}
				branchNode.mass = num2;
			}
		}
	}

	private void UpdateFruits()
	{
		List<BranchNode> list = new List<BranchNode>();
		foreach (BranchNode branch in plantData.branches)
		{
			if (branch.fruitIndex > -1)
			{
				branch.fruitGrowth += 0.2f;
				branch.fruitGrowth = Mathf.Clamp01(branch.fruitGrowth);
			}
			else if (branch.children.Count == 0)
			{
				list.Add(branch);
			}
		}
		if (plantData.numberOfFruits < (plantData.growthStep - trunkDepth) * fruitsPerGrowStep)
		{
			System.Random random = new System.Random(plantData.randomSeed + plantData.growthStep);
			if (list.Count > 0)
			{
				BranchNode branchNode = list[random.Next(0, list.Count)];
				branchNode.fruitIndex = plantData.numberOfFruits;
				branchNode.fruitGrowth = 0f;
				plantData.numberOfFruits++;
			}
		}
	}

	private void Update()
	{
	}

	public string GetAsJson()
	{
		return JsonUtility.ToJson(plantData);
	}

	public void LoadFromJson(string json)
	{
		plantData = JsonUtility.FromJson<PlantData>(json);
		GenerateAttractionPoints();
	}

	private void OnDrawGizmos()
	{
		if (attractionPoints == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		foreach (AttractionPoint attractionPoint in attractionPoints)
		{
			Gizmos.DrawSphere(base.transform.localToWorldMatrix.MultiplyPoint(attractionPoint.position), 0.05f);
		}
	}
}
