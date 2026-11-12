using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupSpawner : MonoBehaviour
{
	public GameObject prefab;

	public int count = 16;

	public EnemyGroupFormation formation;

	public float regroupingTime = 5f;

	private float currentRegroupingTime;

	private List<GameObject> group = new List<GameObject>();

	private List<GameObject> remove = new List<GameObject>();

	private void Start()
	{
		currentRegroupingTime = regroupingTime;
		for (int i = 0; i < count; i++)
		{
			GameObject item = SpawnEnemy(prefab, PositionProvider.Get().GetSpawnPositionCircular());
			group.Add(item);
		}
		ApplyFormation(group);
	}

	public void ApplyFormation(List<GameObject> enemies)
	{
		float num = 32f;
		switch (formation)
		{
		case EnemyGroupFormation.Wave:
		{
			Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
			Vector3 vector = new Vector3(normalized.x, 0f, normalized.y);
			Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
			float num3 = 2f;
			int num2 = 0;
			{
				foreach (GameObject enemy in enemies)
				{
					Vector3 position2 = Player.Get().transform.position + vector * num + vector2 * ((float)num2 - (float)(enemies.Count - 1) * 0.5f) * num3;
					enemy.transform.position = position2;
					BasicAI component3 = enemy.GetComponent<BasicAI>();
					if ((bool)component3)
					{
						component3.positionIsDirty = true;
					}
					WaveEnemy component4 = enemy.GetComponent<WaveEnemy>();
					if ((bool)component4)
					{
						component4.direction = -vector;
					}
					num2++;
				}
				break;
			}
		}
		case EnemyGroupFormation.Circle:
		{
			int num2 = 0;
			{
				foreach (GameObject enemy2 in enemies)
				{
					float f = MathF.PI * 2f / (float)enemies.Count * (float)num2;
					Vector3 vector = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
					Vector3 position = Player.Get().transform.position + vector * num;
					enemy2.transform.position = position;
					BasicAI component = enemy2.GetComponent<BasicAI>();
					if ((bool)component)
					{
						component.positionIsDirty = true;
					}
					WaveEnemy component2 = enemy2.GetComponent<WaveEnemy>();
					if ((bool)component2)
					{
						component2.direction = -vector;
					}
					num2++;
				}
				break;
			}
		}
		}
	}

	public GameObject SpawnEnemy(GameObject prefab, Vector3 position)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
		EnemyManager.Get().EnemySpawned(gameObject);
		return gameObject;
	}

	private void Update()
	{
		remove.Clear();
		foreach (GameObject item in group)
		{
			if (!item)
			{
				remove.Add(item);
			}
		}
		foreach (GameObject item2 in remove)
		{
			group.Remove(item2);
		}
		currentRegroupingTime -= Time.deltaTime;
		if (currentRegroupingTime < 0f)
		{
			currentRegroupingTime = regroupingTime;
			List<GameObject> list = new List<GameObject>();
			int num = 32;
			foreach (GameObject item3 in group)
			{
				if ((item3.transform.position - Player.Get().transform.position).sqrMagnitude > (float)(num * num))
				{
					list.Add(item3);
				}
			}
			ApplyFormation(list);
		}
		if (group.Count <= 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
