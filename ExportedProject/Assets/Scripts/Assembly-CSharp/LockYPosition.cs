using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LockYPosition : MonoBehaviour
{
	public float fixedY;

	private Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 position = rb.position;
		position.y = fixedY;
		rb.position = position;
		Vector3 linearVelocity = rb.linearVelocity;
		linearVelocity.y = 0f;
		rb.linearVelocity = linearVelocity;
	}
}
