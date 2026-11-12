using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class FreeCameraController : MonoBehaviour
{
	[Range(0.01f, 100f)]
	public float movementSpeed = 0.25f;

	[Range(0.01f, 10f)]
	public float rotationSpeed = 0.25f;

	[Range(0.01f, 0.99f)]
	public float smoothing = 0.1f;

	private Camera camera;

	public Vector3 previousMousePos = Vector3.zero;

	public float angleY;

	public float angleX;

	public Vector3 position;

	private void Start()
	{
		previousMousePos = Mouse.current.position.ReadValue();
		angleX = base.transform.rotation.eulerAngles.x;
		angleY = base.transform.rotation.eulerAngles.y;
		position = base.transform.position;
		camera = GetComponent<Camera>();
		camera.transform.rotation = Quaternion.Euler(angleX, angleY, 0f);
	}

	private void Update()
	{
		if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
		{
			previousMousePos = Mouse.current.position.ReadValue();
		}
		if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
		{
			_ = (Vector3)Mouse.current.position.ReadValue();
			angleY += Mouse.current.delta.ReadValue().x * rotationSpeed;
			angleX -= Mouse.current.delta.ReadValue().y * rotationSpeed;
		}
		if (Keyboard.current.wKey.isPressed)
		{
			position += camera.transform.forward * movementSpeed * Time.deltaTime;
		}
		if (Keyboard.current.sKey.isPressed)
		{
			position += -camera.transform.forward * movementSpeed * Time.deltaTime;
		}
		if (Keyboard.current.aKey.isPressed)
		{
			position += -camera.transform.right * movementSpeed * Time.deltaTime;
		}
		if (Keyboard.current.dKey.isPressed)
		{
			position += camera.transform.right * movementSpeed * Time.deltaTime;
		}
		if (Keyboard.current.eKey.isPressed || Keyboard.current.spaceKey.isPressed)
		{
			position += camera.transform.up * movementSpeed * Time.deltaTime;
		}
		if (Keyboard.current.qKey.isPressed || Keyboard.current.leftShiftKey.isPressed)
		{
			position += -camera.transform.up * movementSpeed * Time.deltaTime;
		}
		previousMousePos = Mouse.current.position.ReadValue();
		angleX = Mathf.Clamp(angleX, -90f, 90f);
		base.transform.rotation = Quaternion.Lerp(Quaternion.Euler(angleX, angleY, 0f), base.transform.rotation, smoothing);
		base.transform.position = Vector3.Lerp(position, base.transform.position, smoothing);
	}
}
