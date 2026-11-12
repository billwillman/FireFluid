using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectsKillable : MonoBehaviour, IKillable
{
	public bool destroyOnKill;

	private bool triggered;

	public List<GameObject> activateObjects = new List<GameObject>();

	public List<GameObject> deactivateObjects = new List<GameObject>();

	public string onKillSound;

	public void OnKill()
	{
		if (!triggered)
		{
			triggered = true;
			foreach (GameObject activateObject in activateObjects)
			{
				activateObject.SetActive(value: true);
			}
			foreach (GameObject deactivateObject in deactivateObjects)
			{
				deactivateObject.SetActive(value: false);
			}
		}
		SoundEffectManager.Play(onKillSound, base.transform.position);
		if (destroyOnKill)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
