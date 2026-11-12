using UnityEngine;

public class SoundEffectOnStart : MonoBehaviour
{
	public string soundEffectId;

	public bool playOnLocation = true;

	private void Start()
	{
		if (playOnLocation)
		{
			SoundEffectManager.Play(soundEffectId, base.transform.position, base.transform);
		}
		else
		{
			SoundEffectManager.Play(soundEffectId);
		}
	}
}
