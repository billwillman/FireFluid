using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogStyle : MonoBehaviour
{
	public string id = "";

	public TextMeshProUGUI text;

	protected List<Image> images = new List<Image>();

	public Canvas canvas;

	public float textScrollSpeed = 32f;

	public float showTimer = 9999f;

	private bool scrolling = true;

	private float currentShowTimer;

	public float blendInSpeed = 1f;

	public float blendOutSpeed = 1f;

	protected float blend;

	private bool destroy;

	private void Awake()
	{
		GetComponentsInChildren(images);
		foreach (Image image in images)
		{
			image.material = new Material(image.material);
			image.material.SetFloat("_Blend", 0f);
			Color color = text.color;
			color.a = 0f;
			text.color = color;
		}
	}

	private void Start()
	{
		canvas.worldCamera = Camera.main;
		canvas.planeDistance = 8f;
	}

	public virtual void Update()
	{
		float num = textScrollSpeed;
		if (InputManager.Action == ButtonState.Pressed)
		{
			num *= 8f;
		}
		float y = text.rectTransform.sizeDelta.y;
		Vector2 anchoredPosition = text.rectTransform.anchoredPosition;
		anchoredPosition.y += num * Time.unscaledDeltaTime;
		anchoredPosition.y = Mathf.Min(anchoredPosition.y, y);
		text.rectTransform.anchoredPosition = anchoredPosition;
		scrolling = anchoredPosition.y < y - 0.01f;
		if (!scrolling)
		{
			currentShowTimer -= Time.unscaledDeltaTime;
			if (InputManager.Action == ButtonState.Down)
			{
				currentShowTimer = -1f;
			}
		}
		if (destroy)
		{
			blend -= Time.unscaledDeltaTime * blendOutSpeed;
			if (blend < 0f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			blend = Mathf.Min(blend + Time.unscaledDeltaTime * blendInSpeed, 1f);
		}
		foreach (Image image in images)
		{
			image.material.SetFloat("_Blend", blend);
		}
		Color color = text.color;
		color.a = blend;
		text.color = color;
	}

	public virtual void SetText(string text)
	{
		this.text.text = text;
		currentShowTimer = showTimer;
	}

	public virtual void OnCreateStyle()
	{
		text.text = "";
	}

	public virtual void OnDestroyStyle()
	{
		destroy = true;
		InputManager.ConsumeInputs();
	}

	public bool IsScrolling()
	{
		return scrolling;
	}

	public virtual bool IsFinished()
	{
		return currentShowTimer < 0f;
	}

	public void ForceFinished()
	{
		currentShowTimer = -1f;
	}
}
