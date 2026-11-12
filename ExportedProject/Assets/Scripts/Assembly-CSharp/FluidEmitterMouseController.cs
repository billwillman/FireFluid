using UnityEngine;

[RequireComponent(typeof(FluidEmitter))]
public class FluidEmitterMouseController : MonoBehaviour
{
	private FluidEmitter emitter;

	public Camera cam;

	public float distanceToCamera = 2f;

	private void Start()
	{
		emitter = GetComponent<FluidEmitter>();
	}

	private void LateUpdate()
	{
		Vector3 position = emitter.transform.position;
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = distanceToCamera;
		Vector3 vector = cam.ScreenToWorldPoint(mousePosition);
		position.x = vector.x;
		position.y = vector.y;
		position.z = vector.z;
		emitter.transform.position = position;
		emitter.transform.rotation = Quaternion.LookRotation(-cam.transform.forward);
		if (!emitter.enabled)
		{
			emitter.enabled = true;
		}
	}
}
