using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class DialogManager : MonoBehaviour
{
	private static DialogManager instance;

	public DialogStyle defaultStyle;

	public List<DialogStyle> styles = new List<DialogStyle>();

	private DialogStyle currentStyle;

	public float maxAudioVolume = 0.1f;

	private AudioClip queuedAudioClip;

	private AudioSource audioSource;

	private float audioFadeVolume;

	public static DialogManager Get()
	{
		return instance;
	}

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
			audioSource = GetComponent<AudioSource>();
			if (defaultStyle == null)
			{
				Debug.LogError("DialogManager: default style not set up!");
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (currentStyle != null && currentStyle.IsFinished())
		{
			currentStyle.OnDestroyStyle();
			currentStyle = null;
		}
		if (!IsInDialog())
		{
			queuedAudioClip = null;
		}
		if (queuedAudioClip != audioSource.clip)
		{
			audioFadeVolume = Mathf.Max(audioFadeVolume - Time.deltaTime * 2f, 0f);
			if (audioFadeVolume < 0.01f)
			{
				audioSource.clip = queuedAudioClip;
				if (queuedAudioClip != null)
				{
					audioSource.Play();
				}
			}
		}
		else
		{
			audioFadeVolume = Mathf.Min(audioFadeVolume + Time.deltaTime * 2f, 1f);
		}
		audioSource.volume = audioFadeVolume * maxAudioVolume;
	}

	private void SetStyle(DialogStyle style)
	{
		if (currentStyle == null || currentStyle.id != style.id)
		{
			if (currentStyle != null)
			{
				currentStyle.OnDestroyStyle();
			}
			currentStyle = Object.Instantiate(style.gameObject).GetComponent<DialogStyle>();
			currentStyle.OnCreateStyle();
		}
	}

	public DialogStyle GetCurrentStyleInstance()
	{
		return currentStyle;
	}

	public void ShowDialog(string text)
	{
		ShowDialog(text, "", null);
	}

	public void ShowDialog(string text, string styleId)
	{
		ShowDialog(text, styleId, null);
	}

	public void ShowDialog(string text, string styleId, AudioClip audioClip)
	{
		DialogStyle dialogStyle = null;
		queuedAudioClip = audioClip;
		foreach (DialogStyle style in styles)
		{
			if (style.id == styleId)
			{
				dialogStyle = style;
			}
		}
		if (dialogStyle == null)
		{
			dialogStyle = defaultStyle;
		}
		text = StringLocalization.Parse(text);
		SetStyle(dialogStyle);
		currentStyle.SetText(text);
	}

	public void ForceStop()
	{
		currentStyle.ForceFinished();
	}

	public bool IsInDialog()
	{
		return currentStyle != null;
	}

	public bool IsWaitingForInput()
	{
		if (IsInDialog() && !currentStyle.IsScrolling() && currentStyle.showTimer > 60f)
		{
			return true;
		}
		return false;
	}
}
