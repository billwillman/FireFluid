using System.Collections.Generic;
using UnityEngine;

public class SoundTrack : MonoBehaviour
{
	private static SoundTrack instance;

	public bool mute;

	public SoundTrackTransitionMode transitionMode = SoundTrackTransitionMode.crossFade;

	public GameObject soundTrackPlayerPrefab;

	public List<SoundTrackPlayer> currentlyActiveTracks = new List<SoundTrackPlayer>();

	public SoundTrackPlayer currentTrack;

	public AudioClip Wind;

	public AudioClip Overworld;

	public AudioClip Underworld;

	public double nextStartTime;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static SoundTrack Get()
	{
		return instance;
	}

	public void SetSoundTrack(AudioClip track, bool loop = true, float volume = 0.8f)
	{
		if (currentTrack != null && currentTrack.audioSource.clip == track)
		{
			return;
		}
		if (track == null && transitionMode == SoundTrackTransitionMode.Queue)
		{
			transitionMode = SoundTrackTransitionMode.crossFade;
		}
		if (currentTrack != null)
		{
			if (transitionMode == SoundTrackTransitionMode.cut)
			{
				foreach (SoundTrackPlayer currentlyActiveTrack in currentlyActiveTracks)
				{
					currentlyActiveTrack.FadeOut(0f);
					currentlyActiveTrack.KillWhenSilent();
				}
			}
			if (transitionMode == SoundTrackTransitionMode.fadeOutFadeIn || transitionMode == SoundTrackTransitionMode.crossFade)
			{
				foreach (SoundTrackPlayer currentlyActiveTrack2 in currentlyActiveTracks)
				{
					currentlyActiveTrack2.FadeOut(3f);
					currentlyActiveTrack2.KillWhenSilent();
				}
			}
			if (transitionMode == SoundTrackTransitionMode.Queue)
			{
				currentTrack.audioSource.loop = false;
				currentTrack.KillWhenSilent();
			}
		}
		if (track != null)
		{
			SoundTrackPlayer component = Object.Instantiate(soundTrackPlayerPrefab).GetComponent<SoundTrackPlayer>();
			component.transform.parent = base.transform;
			component.audioSource.loop = loop;
			component.maxVolume = volume;
			component.audioSource.clip = track;
			currentlyActiveTracks.Add(component);
			if (currentTrack != null && Mathf.Abs(currentTrack.audioSource.clip.length - track.length) < 1f)
			{
				component.audioSource.time = currentTrack.audioSource.time;
			}
			currentTrack = component;
			if (transitionMode == SoundTrackTransitionMode.cut)
			{
				currentTrack.FadeIn(0f);
			}
			if (transitionMode == SoundTrackTransitionMode.crossFade)
			{
				currentTrack.FadeIn(3f);
			}
			if (transitionMode == SoundTrackTransitionMode.fadeOutFadeIn)
			{
				currentTrack.SetMute(3f);
				currentTrack.FadeIn(3f);
			}
			double num = (double)track.samples / (double)track.frequency;
			if (transitionMode == SoundTrackTransitionMode.Queue)
			{
				if (nextStartTime < AudioSettings.dspTime)
				{
					currentTrack.audioSource.Play();
					currentTrack.FadeIn(0f);
					nextStartTime = AudioSettings.dspTime + num;
				}
				else
				{
					currentTrack.audioSource.PlayScheduled(nextStartTime);
					nextStartTime += num;
				}
			}
			else
			{
				currentTrack.audioSource.Play();
				nextStartTime = AudioSettings.dspTime + num;
			}
		}
		else
		{
			currentTrack = null;
		}
	}

	private void Update()
	{
		if (currentTrack != null)
		{
			currentTrack.mute = mute;
		}
		List<SoundTrackPlayer> list = new List<SoundTrackPlayer>();
		foreach (SoundTrackPlayer currentlyActiveTrack in currentlyActiveTracks)
		{
			if (currentlyActiveTrack.killWhenSilent && (((double)currentlyActiveTrack.audioSource.volume < 0.001 && currentlyActiveTrack.volumeChangeSpeed < 0.01f) || !currentlyActiveTrack.audioSource.isPlaying))
			{
				Object.Destroy(currentlyActiveTrack.gameObject);
				list.Add(currentlyActiveTrack);
				if (currentTrack == currentlyActiveTrack)
				{
					currentTrack = null;
				}
			}
		}
		foreach (SoundTrackPlayer item in list)
		{
			currentlyActiveTracks.Remove(item);
		}
		if (currentTrack != null && currentTrack.audioSource.loop && !currentTrack.killWhenSilent && nextStartTime < AudioSettings.dspTime)
		{
			double num = (double)currentTrack.audioSource.clip.samples / (double)currentTrack.audioSource.clip.frequency;
			nextStartTime += num;
		}
	}
}
