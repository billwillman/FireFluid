using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class ScatterObjects : MonoBehaviour
{
	public int seed = -1;

	public List<ScatterGroup> groups = new List<ScatterGroup>();

	public float radius = 16f;

	public float innerRadius = 2f;

	public float spacing = 1f;

	public float randomness = 0.5f;

	[Range(0f, 1f)]
	public float groupingFactor = 0.5f;

	public float groupingScale = 1f;

	private List<Vector2> groupingOffsets = new List<Vector2>();

	public Vector3 randomRotation;

	public float randomScale;

	public bool buildOnStart;

	public NavMeshSurface navMeshSurface;

	[ContextMenu("Clear")]
	public void Clear()
	{
		if (groups.Count != groupingOffsets.Count)
		{
			groupingOffsets.Clear();
			for (int i = 0; i < groups.Count; i++)
			{
				groupingOffsets.Add(new Vector2(Random.Range(-9999f, 9999f), Random.Range(-9999f, 9999f)));
			}
		}
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in base.transform)
		{
			list.Add(item.gameObject);
		}
		foreach (GameObject item2 in list)
		{
			Object.DestroyImmediate(item2);
		}
	}

	[ContextMenu("Build")]
	public void Build()
	{
		Clear();
		Random.InitState(seed);
		spacing = Mathf.Max(0.01f, spacing);
		Vector3 zero = Vector3.zero;
		for (float num = 0f - radius; num <= radius; num += spacing)
		{
			for (float num2 = 0f - radius; num2 <= radius; num2 += spacing)
			{
				if (num2 * num2 + num * num > radius * radius)
				{
					continue;
				}
				Vector2 vector = Random.insideUnitCircle * randomness;
				zero.x = num2 + vector.x;
				zero.z = num + vector.y;
				if (zero.x * zero.x + zero.z * zero.z < innerRadius * innerRadius)
				{
					continue;
				}
				GameObject gameObject = null;
				float num3 = 0f;
				int num4 = 0;
				foreach (ScatterGroup group in groups)
				{
					Vector2 vector2 = groupingOffsets[num4];
					float num5 = Mathf.Lerp(Random.value, Mathf.PerlinNoise(num2 * groupingScale + vector2.x, num * groupingScale + vector2.y), groupingFactor);
					if (num5 * group.weight > num3)
					{
						num3 = num5;
						gameObject = group.prefab;
					}
					num4++;
				}
				if (!(gameObject == null))
				{
					GameObject gameObject2 = Object.Instantiate(gameObject, base.transform);
					gameObject2.transform.localPosition = zero;
					gameObject2.transform.localRotation = Quaternion.Euler(Random.Range(0f - randomRotation.x, randomRotation.x), Random.Range(0f - randomRotation.y, randomRotation.y), Random.Range(0f - randomRotation.z, randomRotation.z)) * gameObject2.transform.localRotation;
					gameObject2.transform.localScale *= 1f + Random.Range(0f - randomScale, randomScale);
				}
			}
		}
		if ((bool)navMeshSurface)
		{
			navMeshSurface.BuildNavMesh();
		}
	}

	private void Start()
	{
		if (buildOnStart)
		{
			if (seed < 0)
			{
				seed = Random.Range(0, int.MaxValue);
			}
			Build();
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
