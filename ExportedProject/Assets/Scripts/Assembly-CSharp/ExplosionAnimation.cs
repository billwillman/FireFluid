using UnityEngine;

public class ExplosionAnimation : MonoBehaviour
{
	public float fadeSpeed = 2f;

	public float scaleSpeed = 4f;

	public float targetScaleFactor = 2f;

	private Color color;

	private Vector3 scale;

	private Renderer renderer;

	private void Start()
	{
		scale = base.transform.localScale;
		renderer = GetComponent<Renderer>();
		color = renderer.material.color;
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, scale * targetScaleFactor, Mathf.Clamp01(Time.deltaTime * scaleSpeed));
		color.a = Mathf.Clamp01(color.a - Time.deltaTime * fadeSpeed);
		renderer.material.color = color;
	}
}
