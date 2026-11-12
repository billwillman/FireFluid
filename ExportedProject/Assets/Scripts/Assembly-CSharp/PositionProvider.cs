using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PositionProvider : MonoBehaviour
{
	public WFCGenerator wFCGenerator;

	private Segmentation segmentation;

	private List<Vector3> positions;

	private List<Vector3> currentSpawnPositions = new List<Vector3>();

	private float spawnPositionMaxDistance = 48f;

	private float spawnPositionMinDistance = 32f;

	public float radius = 120f;

	private static PositionProvider instance;

	public static PositionProvider Get()
	{
		return instance;
	}

	public void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
	}

	private void CheckValidPositions()
	{
		if (segmentation != null && positions != null && positions.Count != 0)
		{
			return;
		}
		positions = new List<Vector3>();
		segmentation = wFCGenerator.GenerateSegmentation();
		int num = segmentation.segmentId[wFCGenerator.startingTile.x * 3, wFCGenerator.startingTile.y * 3];
		for (int i = 0; i < segmentation.GetWidth(); i++)
		{
			for (int j = 0; j < segmentation.GetWidth(); j++)
			{
				if (segmentation.segmentId[j, i] == num)
				{
					positions.Add(wFCGenerator.GetInstantiatePosition((float)(j - 1) / 3f, (float)(i - 1) / 3f));
				}
			}
		}
	}

	public void CalculateSpawnPositions()
	{
		CheckValidPositions();
		currentSpawnPositions.Clear();
		Vector3 position = Player.Get().transform.position;
		float num = spawnPositionMaxDistance * spawnPositionMaxDistance;
		float num2 = spawnPositionMinDistance * spawnPositionMinDistance;
		foreach (Vector3 position2 in positions)
		{
			float sqrMagnitude = (position - position2).sqrMagnitude;
			if (sqrMagnitude < num && sqrMagnitude > num2)
			{
				currentSpawnPositions.Add(position2);
			}
		}
	}

	public Vector3 GetValidPositionFromWFC()
	{
		CheckValidPositions();
		return positions[Random.Range(0, positions.Count)];
	}

	public Vector3 GetSpawnPositionFromWFC()
	{
		CheckValidPositions();
		if (currentSpawnPositions.Count > 0)
		{
			return currentSpawnPositions[Random.Range(0, currentSpawnPositions.Count)];
		}
		int num = 32;
		Vector3 position = Player.Get().transform.position;
		Vector3 vector = positions[Random.Range(0, positions.Count)];
		for (int i = 0; i < num; i++)
		{
			float magnitude = (position - vector).magnitude;
			if (magnitude < spawnPositionMaxDistance && magnitude > spawnPositionMinDistance)
			{
				return vector;
			}
			vector = positions[Random.Range(0, positions.Count)];
		}
		return vector;
	}

	public Vector3 GetSpawnPositionCircular()
	{
		int num = 32;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			Vector3 position = Player.Get().transform.position;
			vector = position + new Vector3(Random.Range(0f - spawnPositionMaxDistance, spawnPositionMaxDistance), 0f, Random.Range(0f - spawnPositionMaxDistance, spawnPositionMaxDistance));
			if (vector.x < position.x && vector.x > position.x - spawnPositionMinDistance)
			{
				vector.x = position.x - spawnPositionMinDistance;
			}
			if (vector.x > position.x && vector.x < position.x + spawnPositionMinDistance)
			{
				vector.x = position.x + spawnPositionMinDistance;
			}
			if (vector.z < position.z && vector.z > position.z - spawnPositionMinDistance)
			{
				vector.z = position.z - spawnPositionMinDistance;
			}
			if (vector.z > position.z && vector.z < position.z + spawnPositionMinDistance)
			{
				vector.z = position.z + spawnPositionMinDistance;
			}
			if (NavMesh.SamplePosition(vector, out var hit, 2f, -1))
			{
				vector = hit.position;
				break;
			}
		}
		return vector;
	}

	public Vector3 GetValidPositionCircular()
	{
		int num = 32;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			Vector3 position = Player.Get().transform.position;
			Vector2 vector2 = Random.insideUnitCircle * radius;
			vector = new Vector3(vector2.x, 0f, vector2.y);
			float num2 = 24f;
			if (vector.x < position.x && vector.x > position.x - num2)
			{
				vector.x = position.x - num2;
			}
			if (vector.x > position.x && vector.x < position.x + num2)
			{
				vector.x = position.x + num2;
			}
			if (vector.z < position.z && vector.z > position.z - num2)
			{
				vector.z = position.z - num2;
			}
			if (vector.z > position.z && vector.z < position.z + num2)
			{
				vector.z = position.z + num2;
			}
			if (NavMesh.SamplePosition(vector, out var hit, 2f, -1))
			{
				vector = hit.position;
				if (vector.sqrMagnitude < radius * radius)
				{
					break;
				}
			}
		}
		return vector;
	}

	public Vector3 GetSpawnPosition()
	{
		if ((bool)wFCGenerator)
		{
			return GetSpawnPositionFromWFC();
		}
		return GetSpawnPositionCircular();
	}

	public Vector3 GetValidPositionOnMap()
	{
		if ((bool)wFCGenerator)
		{
			return GetValidPositionFromWFC();
		}
		return GetValidPositionCircular();
	}

	public Vector3 GetValidPositionNear(Vector3 near)
	{
		if ((bool)wFCGenerator)
		{
			CheckValidPositions();
			Vector3 vector = positions[0];
			float num = (near - vector).sqrMagnitude;
			for (int i = 0; i < positions.Count; i++)
			{
				float sqrMagnitude = (near - positions[i]).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vector = positions[i];
				}
			}
			return vector;
		}
		if (NavMesh.SamplePosition(near, out var hit, 2f, -1))
		{
			return hit.position;
		}
		return near;
	}

	private void Update()
	{
		if (wFCGenerator != null && Time.frameCount % 3 == 0)
		{
			CheckValidPositions();
			CalculateSpawnPositions();
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
