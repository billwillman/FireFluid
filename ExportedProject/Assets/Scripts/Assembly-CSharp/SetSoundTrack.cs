using UnityEngine;

public class SetSoundTrack : MonoBehaviour
{
	public bool setOnStart = true;

	public AudioClip track;

	public float volume = 0.8f;

	private void Start()
	{
		if (setOnStart)
		{
			Set();
		}
	}

	public void Set()
	{
		SoundTrack.Get()?.SetSoundTrack(track, loop: true, volume);
	}

	public void SetNone()
	{
		SoundTrack.Get()?.SetSoundTrack(null);
	}
}
