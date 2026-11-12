using UnityEngine;

public class PushPlayerCircular : MonoBehaviour
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
		foreach (Player item in Player.all)
		{
			Vector3 vector = item.transform.position - base.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude < 0.001f)
			{
				vector = Random.insideUnitSphere;
				vector.y = 0f;
				magnitude = vector.magnitude;
			}
			float radius = GetRadius();
			if (magnitude < radius)
			{
				if ((bool)item.GetCharacterController())
				{
					item.GetCharacterController().OffsetPosition(vector / magnitude * Mathf.Clamp01(1f - magnitude / radius) * Time.deltaTime * intensity);
				}
				else
				{
					item.transform.position += vector / magnitude * Mathf.Clamp01(1f - magnitude / radius) * Time.deltaTime * intensity;
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, GetRadius());
	}
}
