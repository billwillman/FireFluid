using UnityEngine;

public class SoundTrackPlayer : MonoBehaviour
{
	public bool mute;

	public AudioSource audioSource;

	public float maxVolume;

	public float currentVolume;

	public float volumeChangeSpeed = 1f;

	private float muteTime;

	public bool killWhenSilent;

	public void FadeOut(float time)
	{
		audioSource.loop = false;
		if ((double)time > 0.001)
		{
			volumeChangeSpeed = (0f - currentVolume) / time;
		}
		else
		{
			volumeChangeSpeed = -100f;
		}
	}

	public void FadeIn(float time)
	{
		if ((double)time > 0.001)
		{
			volumeChangeSpeed = maxVolume / time;
		}
		else
		{
			volumeChangeSpeed = 100f;
		}
	}

	public void SetMute(float time)
	{
		muteTime = time;
	}

	public void KillWhenSilent()
	{
		killWhenSilent = true;
	}

	private void Update()
	{
		audioSource.mute = mute;
		muteTime -= Time.deltaTime;
		if (muteTime > 0f)
		{
			currentVolume -= Time.deltaTime;
			currentVolume = Mathf.Clamp(currentVolume, 0f, maxVolume);
		}
		else
		{
			currentVolume += volumeChangeSpeed * Time.deltaTime;
			currentVolume = Mathf.Clamp(currentVolume, 0f, maxVolume);
		}
		audioSource.volume = currentVolume * Settings.musicVolume;
	}
}
