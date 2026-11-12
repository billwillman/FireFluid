using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
	public ToolTipBox toolTipBox;

	private Canvas canvas;

	private static ToolTipUI instance;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		canvas = GetComponentInChildren<Canvas>();
	}

	public static ToolTipUI Get()
	{
		return instance;
	}

	private void LateUpdate()
	{
		ToolTip toolTip = null;
		foreach (ToolTip item in ToolTip.all)
		{
			if (item.IsShown())
			{
				toolTip = item;
				break;
			}
		}
		if (toolTip != null)
		{
			if (InputManager.inputMode != InputMode.KeyboardAndMouse && !toolTip.forceShowToolTip)
			{
				toolTip = null;
			}
			else if (InputManager.Action == ButtonState.Pressed && !toolTip.forceShowToolTip)
			{
				toolTip = null;
			}
			else if (toolTip != null && toolTip.text == "")
			{
				toolTip = null;
			}
		}
		if (toolTip != null)
		{
			toolTipBox.gameObject.SetActive(value: true);
			toolTipBox.text.text = toolTip.text;
			if (toolTip.GetTargetRect() != null)
			{
				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(toolTip.GetCanvas().worldCamera, toolTip.GetTargetRect().position);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out var localPoint);
				toolTipBox.rect.anchoredPosition = localPoint;
				ClampTooltipToCanvas();
			}
			else
			{
				Vector2 screenPoint2 = RectTransformUtility.WorldToScreenPoint(toolTip.GetCanvas().worldCamera, toolTip.transform.position);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint2, canvas.worldCamera, out var localPoint2);
				toolTipBox.rect.anchoredPosition = localPoint2;
				ClampTooltipToCanvas();
			}
		}
		else
		{
			toolTipBox.gameObject.SetActive(value: false);
		}
	}

	private void ClampTooltipToCanvas()
	{
		RectTransform rectTransform = canvas.transform as RectTransform;
		RectTransform rect = toolTipBox.rect;
		Vector2 anchoredPosition = rect.anchoredPosition;
		Vector2 size = rect.rect.size;
		Vector2 size2 = rectTransform.rect.size;
		float num = size.x * 0.5f;
		float num2 = size.y * 0.5f;
		anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, (0f - size2.x) * 0.5f + num, size2.x * 0.5f - num);
		anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, (0f - size2.y) * 0.5f + num2, size2.y * 0.5f - num2);
		rect.anchoredPosition = anchoredPosition;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
