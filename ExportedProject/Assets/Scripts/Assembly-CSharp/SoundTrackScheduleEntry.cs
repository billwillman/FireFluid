using System;
using UnityEngine;

[Serializable]
public class SoundTrackScheduleEntry
{
	public AudioClip audioClip;

	public float time;

	public SoundTrackTransitionMode transitionMode;

	public float volume = 0.8f;

	[HideInInspector]
	public bool scheduled;
}
