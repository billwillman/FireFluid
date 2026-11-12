using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursorUI : MonoBehaviour
{
	public List<MouseCursorStyle> styles = new List<MouseCursorStyle>();

	[Header("References")]
	public Image cursorImage;

	public Sprite normalSprite;

	public Sprite pressedSprite;

	private MouseCursorStyleType type;

	[Header("Settings")]
	public bool hideSystemCursor = true;

	private CanvasScaler canvasScaler;

	private void Start()
	{
		if (hideSystemCursor)
		{
			Cursor.visible = false;
		}
		canvasScaler = GetComponentInParent<CanvasScaler>();
	}

	private void Update()
	{
		if (InputManager.inputMode != InputMode.KeyboardAndMouse)
		{
			cursorImage.enabled = false;
			return;
		}
		cursorImage.enabled = true;
		if (type != Settings.cursorStyle)
		{
			MouseCursorStyle mouseCursorStyle = null;
			foreach (MouseCursorStyle style in styles)
			{
				if (style.type == Settings.cursorStyle)
				{
					mouseCursorStyle = style;
					break;
				}
			}
			if (mouseCursorStyle != null)
			{
				type = mouseCursorStyle.type;
				normalSprite = mouseCursorStyle.normal;
				pressedSprite = mouseCursorStyle.pressed;
			}
		}
		canvasScaler.scaleFactor = Settings.cursorSize;
		Vector2 vector = Input.mousePosition;
		cursorImage.rectTransform.position = vector;
		if (Input.GetMouseButton(0))
		{
			cursorImage.sprite = pressedSprite;
		}
		else
		{
			cursorImage.sprite = normalSprite;
		}
	}
}
