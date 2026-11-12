using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraController : MonoBehaviour
{
	public ValueStackBool enableController = new ValueStackBool(defaultValue: true);

	public Transform target;

	public float distanceToTarget = 64f;

	public Vector3 offset;

	private float shake;

	private void Start()
	{
	}

	private void Update()
	{
		if (enableController.GetCurrentValue())
		{
			base.transform.position = GetTargetCameraPosition(target.position);
		}
		if (Settings.enableScreenShakes)
		{
			base.transform.position += Random.insideUnitSphere * shake;
		}
		shake = Mathf.Lerp(shake, 0f, Time.unscaledDeltaTime);
	}

	public void Shake(float intensity)
	{
		shake = Mathf.Max(shake, intensity);
	}

	public Vector3 GetTargetCameraPosition(Vector3 targetPosition)
	{
		return targetPosition - base.transform.forward * distanceToTarget + offset;
	}
}
