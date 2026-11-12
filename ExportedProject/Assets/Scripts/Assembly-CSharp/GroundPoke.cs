using UnityEngine;

public class GroundPoke : Projectile
{
	public GameObject onDestroyPrefab;

	private void Start()
	{
		base.transform.rotation = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(0f, 360f), Random.Range(-5f, 5f));
	}

	private void OnDestroy()
	{
		if (onDestroyPrefab != null)
		{
			Object.Instantiate(onDestroyPrefab, base.transform.position, Quaternion.identity);
		}
	}
}
