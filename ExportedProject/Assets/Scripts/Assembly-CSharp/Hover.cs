using UnityEngine;

public class Hover : MonoBehaviour
{
	public float rayOriginOffset;

	public float distanceToGround = 1f;

	public float slopeThreshold = 8f;

	private float hightLastFrame = 100f;

	private Rigidbody rigidbody;

	public LayerMask mask;

	private bool grounded;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		UpdateSpring(Time.deltaTime);
	}

	public void UpdateSpring(float dt)
	{
		Vector3 linearVelocity = rigidbody.linearVelocity;
		if (!(linearVelocity.y > 0f) && !rigidbody.isKinematic)
		{
			if (Physics.Raycast(base.transform.position + Vector3.up * rayOriginOffset, Vector3.down, out var hitInfo, distanceToGround + slopeThreshold * Mathf.Clamp01(dt), mask))
			{
				Vector3 position = hitInfo.point + Vector3.up * (distanceToGround - rayOriginOffset);
				position.y = Mathf.Lerp(base.transform.position.y, position.y, Mathf.Clamp01(dt * 16f));
				base.transform.position = position;
				linearVelocity.y = 0f;
				rigidbody.linearVelocity = linearVelocity;
				grounded = true;
			}
			else
			{
				grounded = false;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Vector3 vector = base.transform.position - Vector3.up * (distanceToGround - rayOriginOffset);
		Gizmos.DrawLine(base.transform.position + Vector3.up * rayOriginOffset, vector);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(vector, vector - Vector3.up * slopeThreshold * Time.deltaTime);
	}
}
