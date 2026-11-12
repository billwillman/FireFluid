using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEffect
{
	public string id;

	public List<AudioClip> clips;

	[Range(0f, 2f)]
	public float volume = 1f;

	public Vector2 pitchRange = new Vector2(1f, 1f);

	public Vector2 cooldownRange = new Vector2(0f, 0f);

	[Range(0f, 1f)]
	public float chanceToPlay = 1f;
}
