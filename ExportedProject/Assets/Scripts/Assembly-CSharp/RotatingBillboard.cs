using UnityEngine;

[ExecuteInEditMode]
[DefaultExecutionOrder(100)]
public class RotatingBillboard : MonoBehaviour
{
	public float rotationSpeed = 45f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnWillRenderObject()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Vector3 eulerAngles2 = Camera.main.transform.rotation.eulerAngles;
		eulerAngles.x = eulerAngles2.x;
		eulerAngles.y = eulerAngles2.y;
		eulerAngles.z = Time.time * rotationSpeed;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
