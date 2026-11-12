using UnityEngine;

public class Candle : MonoBehaviour
{
	public int level = 1;

	public GameObject flame;

	public ParticleSystem particleSystem;

	public bool burning = true;

	public float timeSinceBurning = 1000f;

	private Vector3 flameScale;

	private Vector3 flamePos;

	private Color smokeColor = Color.white;

	public Material lockedMaterial;

	private void Start()
	{
		flameScale = flame.transform.localScale;
		flamePos = flame.transform.localPosition;
		smokeColor.a = 0f;
		if (!PersistentDataComponent.GetDataInstance().IsLevelUnlocked(level))
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].sharedMaterial = lockedMaterial;
			}
		}
	}

	private void Update()
	{
		if (burning)
		{
			flame.transform.localScale = Vector3.Lerp(flame.transform.localScale, flameScale, Time.unscaledDeltaTime * 8f);
			flame.transform.localPosition = Vector3.Lerp(flame.transform.localPosition, flamePos, Time.unscaledDeltaTime * 8f);
			smokeColor.a = 0f;
			timeSinceBurning = 0f;
		}
		else
		{
			flame.transform.localScale = Vector3.Lerp(flame.transform.localScale, Vector3.zero, Time.unscaledDeltaTime * 16f);
			flame.transform.localPosition = Vector3.Lerp(flame.transform.localPosition, flamePos + Vector3.up * 0.4f, Time.unscaledDeltaTime * 8f);
			timeSinceBurning += Time.unscaledDeltaTime;
			smokeColor.a = 1f / timeSinceBurning * (0.5f + Mathf.PerlinNoise1D(Time.unscaledTime * 4f));
		}
		ParticleSystem.MainModule main = particleSystem.main;
		main.startColor = smokeColor;
	}
}
