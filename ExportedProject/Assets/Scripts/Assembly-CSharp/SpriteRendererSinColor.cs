using UnityEngine;
using UnityEngine.UI;

public class SpriteRendererSinColor : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private Image image;

	public float frequency = 4f;

	public Color colorA;

	public Color colorB;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		image = GetComponent<Image>();
	}

	private void Update()
	{
		float t = Mathf.Sin(Time.time * frequency) * 0.5f + 0.5f;
		if ((bool)spriteRenderer)
		{
			spriteRenderer.color = Color.Lerp(colorA, colorB, t);
		}
		if ((bool)image)
		{
			image.color = Color.Lerp(colorA, colorB, t);
		}
	}
}
