using UnityEngine;

public class PushEnemiesCircular : MonoBehaviour
{
	public float intensity = 8f;

	public float radiusMultiplier = 0.25f;

	private void Start()
	{
	}

	private float GetRadius()
	{
		return base.transform.localScale.x * radiusMultiplier;
	}

	private void Update()
	{
		foreach (Enemy item in Enemy.all)
		{
			Vector3 vector = item.transform.position - base.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			float radius = GetRadius();
			if (magnitude < radius)
			{
				item.transform.position += vector / (magnitude + 0.01f) * Mathf.Clamp01(1f - magnitude / radius) * Time.deltaTime * intensity;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, GetRadius());
	}
}
