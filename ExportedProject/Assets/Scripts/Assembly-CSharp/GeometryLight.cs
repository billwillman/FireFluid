using UnityEngine;

public class GeometryLight : MonoBehaviour
{
	public float flickerMin = 0.8f;

	public float flickerMax = 1.2f;

	public float flickerFrequency = 8f;

	private Vector3 originalScale;

	private float noiseOffset;

	private void Start()
	{
		originalScale = base.transform.localScale;
		noiseOffset = Random.value * 4242f;
	}

	private void Update()
	{
		base.transform.localScale = originalScale * (Mathf.PerlinNoise(flickerFrequency + Time.time * flickerFrequency + noiseOffset, 0f) * (flickerMax - flickerMin) + flickerMin);
	}
}
