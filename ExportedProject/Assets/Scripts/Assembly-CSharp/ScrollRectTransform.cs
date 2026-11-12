using UnityEngine;

public class ScrollRectTransform : MonoBehaviour
{
	public float scrollSpeed = 24f;

	private float scrollOffset;

	private RectTransform contentRect;

	private RectTransform viewportRect;

	public MonoBehaviour parentUILayer;

	private void Start()
	{
		contentRect = GetComponent<RectTransform>();
		viewportRect = contentRect.parent as RectTransform;
	}

	private void Update()
	{
		if (UILayer.IsActive(parentUILayer))
		{
			float num = InputManager.UIScroll * scrollSpeed;
			scrollOffset += num;
			float height = contentRect.rect.height;
			float height2 = viewportRect.rect.height;
			float min = Mathf.Min(0f, height - height2);
			float max = Mathf.Max(0f, height - height2);
			scrollOffset = Mathf.Clamp(scrollOffset, min, max);
			contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, scrollOffset);
		}
	}
}
