using System.Collections.Generic;
using UnityEngine;

public class RandomizeSprite : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	public List<Sprite> sprites = new List<Sprite>();

	public float animationCooldown = 0.1f;

	private float currentAnimationCooldown;

	private Vector3 scale;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		scale = base.transform.localScale;
	}

	private void Update()
	{
		currentAnimationCooldown -= Time.deltaTime;
		if (currentAnimationCooldown < 0f)
		{
			currentAnimationCooldown = animationCooldown;
			spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
			Color color = spriteRenderer.color;
			color.a = 1f;
			spriteRenderer.color = color;
			base.transform.localScale = new Vector3(scale.x * Random.Range(0.6f, 1.4f), scale.y, scale.z);
		}
		Color color2 = spriteRenderer.color;
		color2.a -= Time.deltaTime / animationCooldown;
		spriteRenderer.color = color2;
	}
}
