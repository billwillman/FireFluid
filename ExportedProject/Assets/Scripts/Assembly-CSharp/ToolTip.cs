using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(99)]
public class ToolTip : MonoBehaviour
{
	public static List<ToolTip> all = new List<ToolTip>();

	public string text = "";

	private RectTransform targetRect;

	private Canvas canvas;

	public bool forceShowToolTip;

	private bool showToolTip;

	private void Awake()
	{
		targetRect = GetComponent<RectTransform>();
		canvas = ToolTipUI.Get().GetComponent<Canvas>();
		all.Add(this);
	}

	public RectTransform GetTargetRect()
	{
		return targetRect;
	}

	public Canvas GetCanvas()
	{
		return canvas;
	}

	private void Update()
	{
		showToolTip = false;
		if (RectTransformUtility.RectangleContainsScreenPoint(targetRect, Input.mousePosition, canvas.worldCamera))
		{
			showToolTip = true;
		}
	}

	public bool IsShown()
	{
		if (!showToolTip)
		{
			return forceShowToolTip;
		}
		return true;
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}
}
