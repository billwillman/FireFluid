using System;
using UnityEngine;

[Serializable]
public class TwitchEnemySpawnRule
{
	[Tooltip("If using Bits: required bit cost. If using ChannelPoints: leave at 0.")]
	public int bitsCost;

	[Tooltip("If using ChannelPoints: the exact reward name to trigger this spawn.")]
	public string rewardName;

	[Tooltip("How many enemies to spawn when this triggers once.")]
	public int amountToSpawn;

	[Tooltip("Enemy prefab to spawn.")]
	public GameObject enemyPrefab;
}
