using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class BannerUI : MonoBehaviour
{
	private class BannerUICall
	{
		public string text;

		public Color textColor;

		public Color backgroundColor;

		public string animationBlendIn;

		public string animationBlendOut;
	}

	private enum AnimationMode
	{
		IDLE = 0,
		FORWARD = 1,
		BACKWARD = 2
	}

	[Range(0f, 1f)]
	public float blend;

	public Material blendMaterial;

	private Text text;

	private List<Image> images = new List<Image>();

	private static BannerUI instance;

	private List<BannerUICall> displayCalls = new List<BannerUICall>();

	public string animationBlendIn = "";

	public string animationBlendOut = "";

	public float animationSpeedFadeIn = 0.2f;

	public float animationSpeedFadeOut = 0.5f;

	private AnimationMode animationMode;

	private float animationTime;

	private Animator animator;

	public void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		animator = GetComponent<Animator>();
		animator.speed = 0f;
		text = GetComponentInChildren<Text>();
		GetComponentsInChildren(images);
	}

	public static void Display(string text)
	{
		Display(text, Color.white, Color.white, BannerUIAnimationStyle.Impact);
	}

	public static void Display(string text, Color textColor)
	{
		Display(text, textColor, Color.white, BannerUIAnimationStyle.Impact);
	}

	public static void Display(string text, Color textColor, Color backgroundColor)
	{
		Display(text, textColor, backgroundColor, BannerUIAnimationStyle.Impact);
	}

	public static void Display(string text, Color textColor, Color backgroundColor, BannerUIAnimationStyle animationStyle)
	{
		BannerUICall bannerUICall = new BannerUICall();
		bannerUICall.text = text;
		bannerUICall.textColor = textColor;
		bannerUICall.backgroundColor = backgroundColor;
		switch (animationStyle)
		{
		case BannerUIAnimationStyle.Impact:
			bannerUICall.animationBlendIn = "BlendIn_Impact";
			bannerUICall.animationBlendOut = "BlendOut_Impact";
			break;
		case BannerUIAnimationStyle.Swipe:
			bannerUICall.animationBlendIn = "BlendIn_Swipe";
			bannerUICall.animationBlendOut = "BlendOut_Swipe";
			break;
		case BannerUIAnimationStyle.TextOnly:
			bannerUICall.animationBlendIn = "BlendIn_TextOnly";
			bannerUICall.animationBlendOut = "BlendOut_TextOnly";
			break;
		case BannerUIAnimationStyle.TextStay:
			bannerUICall.animationBlendIn = "BlendIn_TextOnly";
			bannerUICall.animationBlendOut = "TextStay";
			break;
		}
		instance.displayCalls.Add(bannerUICall);
	}

	private void Update()
	{
		if (blendMaterial != null)
		{
			blendMaterial.SetFloat("_Blend", blend);
		}
		switch (animationMode)
		{
		case AnimationMode.IDLE:
			if (displayCalls.Count <= 0)
			{
				break;
			}
			text.text = displayCalls[0].text;
			text.color = displayCalls[0].textColor;
			animationBlendIn = displayCalls[0].animationBlendIn;
			animationBlendOut = displayCalls[0].animationBlendOut;
			foreach (Image image in images)
			{
				image.color = displayCalls[0].textColor;
			}
			displayCalls.RemoveAt(0);
			animationMode = AnimationMode.FORWARD;
			break;
		case AnimationMode.FORWARD:
			animationTime += Time.deltaTime * animationSpeedFadeIn;
			animator.Play(animationBlendIn, 0, Mathf.Clamp01(animationTime));
			if ((double)animationTime > 1.0)
			{
				animationTime = 0f;
				animationMode = AnimationMode.BACKWARD;
			}
			break;
		case AnimationMode.BACKWARD:
			animationTime += Time.deltaTime * animationSpeedFadeOut;
			animator.Play(animationBlendOut, 0, Mathf.Clamp01(animationTime));
			if ((double)animationTime > 1.0)
			{
				animationTime = 0f;
				animationMode = AnimationMode.IDLE;
			}
			break;
		}
	}
}
