using System.Collections.Generic;
using UnityEngine;

public class StandardKillable : MonoBehaviour, IKillable
{
	public bool destroyOnKill = true;

	public bool multiplyObjectScale;

	public List<SpawnPrefabOnTransform> onKillPrefabs = new List<SpawnPrefabOnTransform>();

	public string onKillSound;

	public void OnKill()
	{
		foreach (SpawnPrefabOnTransform onKillPrefab in onKillPrefabs)
		{
			if (onKillPrefab.prefab == null)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(onKillPrefab.prefab);
			if (onKillPrefab.transform != null)
			{
				gameObject.transform.position = onKillPrefab.transform.position + onKillPrefab.offset;
				gameObject.transform.rotation = onKillPrefab.transform.rotation;
				Vector3 localScale = gameObject.transform.localScale;
				localScale.x *= onKillPrefab.transform.root.localScale.x;
				localScale.y *= onKillPrefab.transform.root.localScale.y;
				localScale.z *= onKillPrefab.transform.root.localScale.z;
				gameObject.transform.localScale = localScale;
			}
			else
			{
				gameObject.transform.position = base.transform.position + onKillPrefab.offset;
			}
			if (multiplyObjectScale)
			{
				Vector3 localScale2 = gameObject.transform.localScale;
				localScale2.x *= base.transform.localScale.x;
				localScale2.y *= base.transform.localScale.y;
				localScale2.z *= base.transform.localScale.z;
				gameObject.transform.localScale = localScale2;
			}
		}
		SoundEffectManager.Play(onKillSound, base.transform.position);
		if (destroyOnKill)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
