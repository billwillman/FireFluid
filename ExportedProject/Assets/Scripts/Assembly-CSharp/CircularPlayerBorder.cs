using UnityEngine;

[DefaultExecutionOrder(100)]
public class CircularPlayerBorder : MonoBehaviour
{
	public float hardness = 0.1f;

	private void FixedUpdate()
	{
		float num = base.transform.localScale.x - 1f;
		foreach (Player item in Player.all)
		{
			Vector3 vector = base.transform.position - item.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude > num)
			{
				vector = vector.normalized * (magnitude - num) * Mathf.Clamp01(hardness * Time.deltaTime);
				item.GetComponent<CharacterController>().OffsetPosition(vector);
			}
		}
	}
}
