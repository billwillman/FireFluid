using System;
using UnityEngine;

[Serializable]
public class EnemyWave
{
	public float startTime;

	public float duration;

	public int count;

	public GameObject prefab;

	[NonSerialized]
	[HideInInspector]
	public float currentCoolDown;

	[NonSerialized]
	[HideInInspector]
	public float currentCount;
}
