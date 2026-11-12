using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CircleSpawner : MonoBehaviour
{
	public GameObject prefab;

	[Header("Area")]
	[Min(0.1f)]
	public float radius = 10f;

	public float yHeight;

	[Header("Distribution")]
	[Min(8f)]
	public float minDistance = 2f;

	[Range(1f, 100f)]
	public int rejectionSamples = 30;

	private List<Vector2> cachedPoints = new List<Vector2>();

	private void OnValidate()
	{
		radius = Mathf.Max(0.1f, radius);
		minDistance = Mathf.Max(0.01f, minDistance);
		GeneratePoints();
	}

	private void Start()
	{
		GeneratePoints();
		if (Application.isPlaying)
		{
			Spawn();
		}
	}

	private void GeneratePoints()
	{
		if (radius <= minDistance)
		{
			cachedPoints.Clear();
		}
		else
		{
			cachedPoints = PoissonDiskCircle(radius, minDistance, rejectionSamples);
		}
	}

	private void Spawn()
	{
		foreach (Vector2 cachedPoint in cachedPoints)
		{
			Vector3 position = base.transform.position + new Vector3(cachedPoint.x, yHeight, cachedPoint.y);
			UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity, base.transform);
		}
	}

	private List<Vector2> PoissonDiskCircle(float radius, float minDist, int k)
	{
		float num = minDist / Mathf.Sqrt(2f);
		int num2 = Mathf.CeilToInt(radius * 2f / num);
		Vector2?[,] array = new Vector2?[num2, num2];
		List<Vector2> list = new List<Vector2>();
		List<Vector2> list2 = new List<Vector2>();
		list2.Add(UnityEngine.Random.insideUnitCircle * radius * 0.5f);
		while (list2.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list2.Count);
			Vector2 vector = list2[index];
			bool flag = false;
			for (int i = 0; i < k; i++)
			{
				float f = UnityEngine.Random.value * MathF.PI * 2f;
				float num3 = UnityEngine.Random.Range(minDist, 2f * minDist);
				Vector2 vector2 = vector + new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num3;
				if (vector2.magnitude > radius)
				{
					continue;
				}
				int num4 = (int)((vector2.x + radius) / num);
				int num5 = (int)((vector2.y + radius) / num);
				bool flag2 = true;
				for (int j = Mathf.Max(0, num4 - 2); j <= Mathf.Min(num2 - 1, num4 + 2); j++)
				{
					for (int l = Mathf.Max(0, num5 - 2); l <= Mathf.Min(num2 - 1, num5 + 2); l++)
					{
						if (array[j, l].HasValue && (array[j, l].Value - vector2).sqrMagnitude < minDist * minDist)
						{
							flag2 = false;
							break;
						}
					}
					if (!flag2)
					{
						break;
					}
				}
				if (flag2)
				{
					list.Add(vector2);
					list2.Add(vector2);
					array[num4, num5] = vector2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list2.RemoveAt(index);
			}
		}
		return list;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 center = base.transform.position + Vector3.up * yHeight;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(center, radius);
		Gizmos.color = Color.yellow;
		if (cachedPoints == null)
		{
			return;
		}
		foreach (Vector2 cachedPoint in cachedPoints)
		{
			Gizmos.DrawSphere(base.transform.position + new Vector3(cachedPoint.x, yHeight, cachedPoint.y), 1f);
		}
	}
}
