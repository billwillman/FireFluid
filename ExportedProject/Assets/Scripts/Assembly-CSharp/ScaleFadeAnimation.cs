using UnityEngine;
using UnityEngine.UI;

public class ScaleFadeAnimation : MonoBehaviour
{
	public float fadeSpeed = 2f;

	public float scaleSpeed = 4f;

	public Vector3 targetScaleFactor = new Vector3(3f, 0f, 3f);

	private Color color;

	private Vector3 targetScale;

	private Renderer renderer;

	private Image image;

	private void Start()
	{
		targetScale = base.transform.localScale;
		targetScale.x *= targetScaleFactor.x;
		targetScale.y *= targetScaleFactor.y;
		targetScale.z *= targetScaleFactor.z;
		renderer = GetComponent<Renderer>();
		if ((bool)renderer)
		{
			color = renderer.material.color;
		}
		image = GetComponent<Image>();
		if ((bool)image)
		{
			color = image.color;
		}
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, targetScale, Mathf.Clamp01(Time.deltaTime * scaleSpeed));
		if ((bool)renderer)
		{
			color.a = Mathf.Clamp01(color.a - Time.deltaTime * fadeSpeed);
			renderer.material.color = color;
		}
		if ((bool)image)
		{
			color.a = Mathf.Clamp01(color.a - Time.deltaTime * fadeSpeed);
			image.color = color;
		}
	}
}
