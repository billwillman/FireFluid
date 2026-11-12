using System;
using UnityEngine;

[Serializable]
public class SpawnPrefabOnTransform
{
	public Transform transform;

	public GameObject prefab;

	public Vector3 offset = Vector3.zero;
}
