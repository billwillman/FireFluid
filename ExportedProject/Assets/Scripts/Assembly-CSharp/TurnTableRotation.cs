using UnityEngine;

public class TurnTableRotation : MonoBehaviour
{
	public float rotationSpeed = 1f;

	private Vector2 mouseLastFrame;

	private void Start()
	{
		mouseLastFrame = InputManager.screenMouse;
	}

	private void Update()
	{
		if (InputManager.inputMode == InputMode.KeyboardAndMouse)
		{
			if (InputManager.Action == ButtonState.Pressed)
			{
				Vector3 axis = new Vector3(0f, mouseLastFrame.x - InputManager.screenMouse.x, 0f);
				float angle = Time.unscaledDeltaTime * axis.magnitude * 16f * rotationSpeed;
				base.transform.localRotation = Quaternion.AngleAxis(angle, axis) * base.transform.localRotation;
			}
			mouseLastFrame = InputManager.screenMouse;
		}
		if (InputManager.inputMode == InputMode.Gamepad)
		{
			Vector3 axis2 = new Vector3(0f, 0f - InputManager.AimHorizontal, 0f);
			float angle2 = Time.unscaledDeltaTime * axis2.magnitude * 64f * rotationSpeed;
			base.transform.localRotation = Quaternion.AngleAxis(angle2, axis2) * base.transform.localRotation;
		}
	}
}
