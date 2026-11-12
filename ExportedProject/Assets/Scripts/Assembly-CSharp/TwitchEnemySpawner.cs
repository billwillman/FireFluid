using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TwitchEnemySpawner : MonoBehaviour
{
	[Header("Spawn Trigger Settings")]
	public TwitchTriggerType triggerType = TwitchTriggerType.ChannelPoints;

	[Header("Enemy spawn rules (editable in Inspector)")]
	public List<TwitchEnemySpawnRule> spawnRules = new List<TwitchEnemySpawnRule>();

	[Header("Spawn Settings")]
	public Transform spawnParent;

	public Vector3 spawnAreaCenter = Vector3.zero;

	public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

	private TwitchManager twitchManager;

	private void Awake()
	{
		twitchManager = GetComponent<TwitchManager>();
		if (twitchManager == null)
		{
			Debug.LogError("[TwitchEnemySpawner] No TwitchManager found on the same GameObject!");
			base.enabled = false;
		}
	}

	private void Start()
	{
		ITwitchClient client = twitchManager.GetClient();
		if (client != null)
		{
			if (triggerType == TwitchTriggerType.Bits)
			{
				client.OnBitsReceived += HandleBits;
			}
			else if (triggerType == TwitchTriggerType.ChannelPoints)
			{
				client.OnChannelPointRedeemed += HandleChannelPoints;
			}
		}
	}

	private void HandleBits(string username, int bits)
	{
		Debug.Log($"[TwitchEnemySpawner] {username} cheered {bits} bits!");
		TrySpawn(username, bits);
	}

	private void HandleChannelPoints(string username, string reward)
	{
		Debug.Log("[TwitchEnemySpawner] " + username + " redeemed reward '" + reward + "'");
		TwitchEnemySpawnRule twitchEnemySpawnRule = spawnRules.Find((TwitchEnemySpawnRule r) => string.Equals(r.rewardName, reward, StringComparison.OrdinalIgnoreCase));
		if (twitchEnemySpawnRule != null)
		{
			for (int num = 0; num < twitchEnemySpawnRule.amountToSpawn; num++)
			{
				SpawnEnemy(twitchEnemySpawnRule.enemyPrefab);
			}
			NotificationManager.PushNotification($"{username} sent {twitchEnemySpawnRule.amountToSpawn} {twitchEnemySpawnRule.enemyPrefab.name}(s) into battle with '{reward}'!");
		}
	}

	private void TrySpawn(string username, int bits)
	{
		if (spawnRules == null || spawnRules.Count == 0)
		{
			return;
		}
		spawnRules.Sort((TwitchEnemySpawnRule a, TwitchEnemySpawnRule b) => b.bitsCost.CompareTo(a.bitsCost));
		int num = bits;
		List<string> list = new List<string>();
		foreach (TwitchEnemySpawnRule spawnRule in spawnRules)
		{
			if (num >= spawnRule.bitsCost)
			{
				int num2 = num / spawnRule.bitsCost;
				num -= num2 * spawnRule.bitsCost;
				int num3 = spawnRule.amountToSpawn * num2;
				for (int num4 = 0; num4 < num3; num4++)
				{
					SpawnEnemy(spawnRule.enemyPrefab);
				}
				list.Add($"{num3} {spawnRule.enemyPrefab.name}(s)");
			}
		}
		if (list.Count > 0)
		{
			NotificationManager.PushNotification(username + " sent " + string.Join(", ", list) + " into battle!");
		}
	}

	private void SpawnEnemy(GameObject prefab)
	{
		if (!prefab)
		{
			Debug.LogWarning("[TwitchEnemySpawner] Missing prefab!");
		}
		else
		{
			EnemyManager.Get().SpawnEnemy(prefab.GetComponent<BasicAI>());
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
	}
}
