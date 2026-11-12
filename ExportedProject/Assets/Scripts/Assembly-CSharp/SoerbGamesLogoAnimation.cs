using System.Collections.Generic;
using UnityEngine;

public class SoerbGamesLogoAnimation : MonoBehaviour
{
	public float dt = 1f;

	public float time;

	public AudioSource jingleSound;

	public float jinglePlayOffset = 0.1f;

	private bool jinglePlayed;

	public static bool animationFinished;

	public List<Material> animateMaterial = new List<Material>();

	private void Awake()
	{
		animationFinished = false;
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (!jinglePlayed && time >= jinglePlayOffset)
		{
			jingleSound.volume = Settings.fxVolume;
			jingleSound.time = time - jinglePlayOffset;
			jingleSound.Play();
			jinglePlayed = true;
		}
		foreach (Material item in animateMaterial)
		{
			item.SetFloat("_Animation", time - 0.5f);
		}
		if (time > 4f)
		{
			animationFinished = true;
		}
	}
}
