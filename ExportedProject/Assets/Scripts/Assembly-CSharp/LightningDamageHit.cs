using UnityEngine;

public class LightningDamageHit : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private Color color;

	public float decayTime = 0.5f;

	private float animationTimer;

	private void UpdateSprite(SpriteRenderer spriteRenderer, Vector3 pos, Color color)
	{
		if (!(spriteRenderer == null))
		{
			spriteRenderer.color = color;
			Vector3 eulerAngles = Camera.main.transform.rotation.eulerAngles;
			Vector3 euler = default(Vector3);
			euler.x = eulerAngles.x;
			euler.y = eulerAngles.y;
			euler.z = Random.Range(0f, 360f);
			spriteRenderer.transform.position = pos;
			spriteRenderer.transform.rotation = Quaternion.Euler(euler);
		}
	}

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		color = spriteRenderer.color;
		UpdateSprite(spriteRenderer, base.transform.position, color);
	}

	private void Update()
	{
		color.a -= Time.deltaTime / decayTime;
		spriteRenderer.color = color;
		animationTimer -= Time.deltaTime;
		if (animationTimer < 0f)
		{
			animationTimer = 0.1f;
			UpdateSprite(spriteRenderer, base.transform.position, color);
		}
		if (color.a <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
