using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningStrike : MonoBehaviour
{
	private static int numberOfLightningStrikes;

	public SpriteRenderer startGraphic;

	public SpriteRenderer endGraphic;

	private float animationTimer;

	public Vector3 start;

	public Vector3 end;

	public int numberOfPoints = 64;

	public float decayTime = 0.5f;

	private Vector3[] lightningPoints;

	public float randomization = 0.2f;

	public int perlinOctaves = 3;

	public float perlinNoiseFrequency = 4f;

	public float perlinNoiseAmplitude = 2f;

	public float perlinMovement = 8f;

	public float randomMovement = 2f;

	private LineRenderer lineRenderer;

	private Color startColor;

	private Color endColor;

	private int seed;

	private void Start()
	{
		if (Settings.performanceMode)
		{
			numberOfPoints /= 2;
		}
		if (numberOfLightningStrikes > 8)
		{
			numberOfPoints /= 2;
		}
		if (numberOfLightningStrikes > 16)
		{
			numberOfPoints /= 2;
		}
		if (numberOfPoints < 2)
		{
			numberOfPoints = 2;
		}
		lineRenderer = GetComponent<LineRenderer>();
		seed = Random.Range(0, 9999);
		GenerateLineRendererPoints();
		startColor = lineRenderer.startColor;
		endColor = lineRenderer.endColor;
		numberOfLightningStrikes++;
		UpdateSprite(startGraphic, start, startColor);
		UpdateSprite(endGraphic, end, endColor);
	}

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

	private void Update()
	{
		startColor.a -= Time.deltaTime / decayTime;
		endColor.a -= Time.deltaTime / decayTime;
		lineRenderer.startColor = startColor;
		lineRenderer.endColor = endColor;
		animationTimer -= Time.deltaTime;
		if (animationTimer < 0f)
		{
			animationTimer = 0.1f;
			UpdateSprite(startGraphic, start, startColor);
			UpdateSprite(endGraphic, end, endColor);
		}
		MoveLineRendererPoints();
		if (startColor.a <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void GenerateLineRendererPoints()
	{
		lineRenderer.positionCount = numberOfPoints;
		if (lightningPoints == null || lightningPoints.Length != numberOfPoints)
		{
			lightningPoints = new Vector3[numberOfPoints];
			lineRenderer.positionCount = numberOfPoints;
		}
		Vector3 vector = (start - end) / (numberOfPoints - 1);
		for (int i = 0; i < numberOfPoints; i++)
		{
			float num = 1f * (float)i / (float)(numberOfPoints - 1);
			lightningPoints[i] = start - vector * i;
			lightningPoints[i] += new Vector3(Random.Range(0f - randomization, randomization), Random.Range(0f - randomization, randomization), Random.Range(0f - randomization, randomization));
			for (int j = 0; j < perlinOctaves; j++)
			{
				float num2 = perlinNoiseFrequency * (float)(1 << j);
				float num3 = perlinNoiseAmplitude / (float)(1 << j);
				lightningPoints[i] += new Vector3(Mathf.PerlinNoise1D(num * num2 + (float)seed) - 0.5f, Mathf.PerlinNoise1D(num * num2 + (float)seed - 1623f) - 0.5f, Mathf.PerlinNoise1D(num * num2 + (float)seed - 134f) - 0.5f) * num3;
			}
		}
		lightningPoints[0] = start;
		lightningPoints[numberOfPoints - 1] = end;
		lineRenderer.SetPositions(lightningPoints);
	}

	private void MoveLineRendererPoints()
	{
		for (int i = 1; i < numberOfPoints - 1; i++)
		{
			float num = 1f * (float)i / (float)(numberOfPoints - 1);
			Vector3 vector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			lightningPoints[i] += vector * randomMovement * Time.deltaTime;
			lightningPoints[i] += new Vector3(Mathf.PerlinNoise1D(num * perlinNoiseFrequency * 2f + (float)seed) - 0.5f, Mathf.PerlinNoise1D(num * perlinNoiseFrequency * 2f + (float)seed - 1623f) - 0.5f, Mathf.PerlinNoise1D(num * perlinNoiseFrequency * 2f + (float)seed - 134f) - 0.5f) * perlinMovement * Time.deltaTime;
		}
		lineRenderer.SetPositions(lightningPoints);
	}

	private void OnDestroy()
	{
		numberOfLightningStrikes--;
	}
}
