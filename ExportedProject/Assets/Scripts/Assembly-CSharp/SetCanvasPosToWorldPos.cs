using UnityEngine;

[DefaultExecutionOrder(1)]
public class SetCanvasPosToWorldPos : MonoBehaviour
{
	private Camera camera;

	private RectTransform rect;

	public RectTransform canvasRect;

	public Transform target;

	private void Start()
	{
		camera = Camera.main;
		rect = GetComponent<RectTransform>();
		Update();
	}

	private void Update()
	{
		if (Vector3.Dot(target.position - camera.transform.position, camera.transform.forward) < 0f)
		{
			rect.anchoredPosition = Vector3.up * 100000f;
			return;
		}
		Bounds bounds = target.GetComponentsInChildren<Renderer>().ComputeBounds();
		Vector3 position = bounds.center + new Vector3(0f, bounds.extents.y, 0f);
		Vector2 vector = camera.WorldToViewportPoint(position);
		Vector2 anchoredPosition = new Vector2(vector.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f, vector.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
		rect.anchoredPosition = anchoredPosition;
	}
}
