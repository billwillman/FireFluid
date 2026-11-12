using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[DefaultExecutionOrder(-1000)]
public class InputManager : MonoBehaviour
{
	public static Transform AimReference;

	public static ValueStackBool enableInputs = new ValueStackBool(defaultValue: true);

	private static float disableInputsTimer = 0f;

	private static InputManager instance;

	public static ButtonState Menu = ButtonState.Idle;

	public static ButtonState MenuBack = ButtonState.Idle;

	public static ButtonState MenuNextPage = ButtonState.Idle;

	public static ButtonState MenuPreviousPage = ButtonState.Idle;

	public static ButtonState Character = ButtonState.Idle;

	public static ButtonState Action = ButtonState.Idle;

	public static ButtonState SecondaryAction = ButtonState.Idle;

	public static float MoveHorizontal = 0f;

	public static float MoveVertical = 0f;

	public static float MoveHorizontalLastFrame = 0f;

	public static float MoveVerticalLastFrame = 0f;

	public static float AimHorizontal = 0f;

	public static float AimVertical = 0f;

	public static float AimHorizontalLastFrame = 0f;

	public static float AimVerticalLastFrame = 0f;

	public static float LookZoom = 0f;

	private float lookStickSensitivity = 4f;

	public static Vector2 screenMouse = Vector2.zero;

	public static Vector3 worldMouse = Vector3.zero;

	public static InputMode inputMode = InputMode.KeyboardAndMouse;

	public static float UIScroll = 0f;

	public static bool MoveLeftPressedThisFrame()
	{
		if (MoveHorizontal < -0.5f)
		{
			return MoveHorizontalLastFrame > -0.5f;
		}
		return false;
	}

	public static bool MoveRightPressedThisFrame()
	{
		if (MoveHorizontal > 0.5f)
		{
			return MoveHorizontalLastFrame < 0.5f;
		}
		return false;
	}

	public static bool MoveUpPressedThisFrame()
	{
		if (MoveVertical > 0.5f)
		{
			return MoveVerticalLastFrame < 0.5f;
		}
		return false;
	}

	public static bool MoveDownPressedThisFrame()
	{
		if (MoveVertical < -0.5f)
		{
			return MoveVerticalLastFrame > -0.5f;
		}
		return false;
	}

	public static bool MovePressedThisFrame()
	{
		if (!MoveUpPressedThisFrame() && !MoveDownPressedThisFrame() && !MoveLeftPressedThisFrame())
		{
			return MoveRightPressedThisFrame();
		}
		return true;
	}

	public static bool AimLeftPressedThisFrame()
	{
		if (AimHorizontal < -0.5f)
		{
			return AimHorizontalLastFrame > -0.5f;
		}
		return false;
	}

	public static bool AimRightPressedThisFrame()
	{
		if (AimHorizontal > 0.5f)
		{
			return AimHorizontalLastFrame < 0.5f;
		}
		return false;
	}

	public static bool AimUpPressedThisFrame()
	{
		if (AimVertical > 0.5f)
		{
			return AimVerticalLastFrame < 0.5f;
		}
		return false;
	}

	public static bool AimDownPressedThisFrame()
	{
		if (AimVertical < -0.5f)
		{
			return AimVerticalLastFrame > -0.5f;
		}
		return false;
	}

	public static bool AimPressedThisFrame()
	{
		if (!AimUpPressedThisFrame() && !AimDownPressedThisFrame() && !AimLeftPressedThisFrame())
		{
			return AimRightPressedThisFrame();
		}
		return true;
	}

	private void MapUnityKeyControlToButton(ref ButtonState b, KeyControl key, bool additive = true)
	{
		if (!additive)
		{
			b = ButtonState.Idle;
		}
		if (key.isPressed)
		{
			b = ButtonState.Pressed;
		}
		if (key.wasPressedThisFrame)
		{
			b = ButtonState.Down;
		}
		if (key.wasReleasedThisFrame)
		{
			b = ButtonState.Up;
		}
	}

	private void MapUnityButtonControlToButton(ref ButtonState b, ButtonControl button, bool additive = true)
	{
		if (!additive)
		{
			b = ButtonState.Idle;
		}
		if (button.isPressed)
		{
			b = ButtonState.Pressed;
		}
		if (button.wasPressedThisFrame)
		{
			b = ButtonState.Down;
		}
		if (button.wasReleasedThisFrame)
		{
			b = ButtonState.Up;
		}
	}

	private void MapUnityAxisToAxis(ref float axis, string unityAxisName, bool additive = true)
	{
		if (additive)
		{
			axis += Input.GetAxisRaw(unityAxisName);
		}
		else
		{
			axis = Input.GetAxisRaw(unityAxisName);
		}
	}

	private void MapUnityKeyControlToAxis(ref float axis, KeyControl positiveKey, KeyControl negativeKey, bool additive = true)
	{
		if (!additive)
		{
			axis = 0f;
		}
		if (positiveKey.isPressed)
		{
			axis += 1f;
		}
		if (negativeKey.isPressed)
		{
			axis -= 1f;
		}
	}

	private void MapUnityVector2ToAxis(ref float axisX, ref float axisY, Vector2 v, bool additive = true)
	{
		if (additive)
		{
			axisX += v.x;
			axisY += v.y;
		}
		else
		{
			axisX = v.x;
			axisY = v.y;
		}
	}

	public static InputManager GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			if (base.transform.parent == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private static void ResetInputs()
	{
		Menu = ButtonState.Idle;
		MenuBack = ButtonState.Idle;
		MenuNextPage = ButtonState.Idle;
		MenuPreviousPage = ButtonState.Idle;
		Character = ButtonState.Idle;
		MoveHorizontalLastFrame = MoveHorizontal;
		MoveVerticalLastFrame = MoveVertical;
		AimHorizontalLastFrame = AimHorizontal;
		AimVerticalLastFrame = AimVertical;
		Action = ButtonState.Idle;
		SecondaryAction = ButtonState.Idle;
		MoveHorizontal = 0f;
		MoveVertical = 0f;
		AimHorizontal = 0f;
		AimVertical = 0f;
		LookZoom = 0f;
		UIScroll = 0f;
	}

	public static void DisableInputs(float time)
	{
		disableInputsTimer = time;
		ResetInputs();
	}

	public static void ConsumeInputs()
	{
		ResetInputs();
	}

	private void Update()
	{
		ResetInputs();
		disableInputsTimer -= Time.unscaledDeltaTime;
		if (enableInputs.GetCurrentValue() && !(disableInputsTimer > 0f))
		{
			MapUnityKeyControlToButton(ref Menu, Keyboard.current.escapeKey);
			MapUnityKeyControlToButton(ref MenuBack, Keyboard.current.backspaceKey);
			MapUnityKeyControlToButton(ref MenuNextPage, Keyboard.current.tabKey);
			MapUnityKeyControlToButton(ref Character, Keyboard.current.enterKey);
			MapUnityKeyControlToButton(ref Character, Keyboard.current.iKey);
			MapUnityKeyControlToButton(ref Character, Keyboard.current.cKey);
			MapUnityKeyControlToButton(ref Character, Keyboard.current.tabKey);
			MapUnityKeyControlToButton(ref Action, Keyboard.current.eKey);
			MapUnityKeyControlToButton(ref Action, Keyboard.current.enterKey);
			MapUnityButtonControlToButton(ref Action, Mouse.current.leftButton);
			MapUnityKeyControlToButton(ref SecondaryAction, Keyboard.current.leftShiftKey);
			MapUnityKeyControlToButton(ref SecondaryAction, Keyboard.current.spaceKey);
			MapUnityButtonControlToButton(ref SecondaryAction, Mouse.current.rightButton);
			MapUnityKeyControlToAxis(ref MoveHorizontal, Keyboard.current.dKey, Keyboard.current.aKey);
			MapUnityKeyControlToAxis(ref MoveHorizontal, Keyboard.current.rightArrowKey, Keyboard.current.leftArrowKey);
			MapUnityKeyControlToAxis(ref MoveVertical, Keyboard.current.wKey, Keyboard.current.sKey);
			MapUnityKeyControlToAxis(ref MoveVertical, Keyboard.current.upArrowKey, Keyboard.current.downArrowKey);
			if (Mouse.current != null)
			{
				UIScroll += Mouse.current.scroll.value.y;
				screenMouse = Mouse.current.position.value;
			}
			float axisX = 0f;
			MapUnityVector2ToAxis(ref axisX, ref LookZoom, Mouse.current.scroll.ReadValue());
			if (Mouse.current.leftButton.isPressed)
			{
				MapUnityVector2ToAxis(ref AimHorizontal, ref AimVertical, (Mouse.current.position.ReadValue() - new Vector2(Screen.width, Screen.height) / 2f).normalized);
			}
			if ((Keyboard.current != null && Keyboard.current.anyKey.isPressed) || (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)))
			{
				inputMode = InputMode.KeyboardAndMouse;
			}
			if (Gamepad.current != null && (Gamepad.current.buttonSouth.isPressed || Gamepad.current.buttonNorth.isPressed || Gamepad.current.buttonEast.isPressed || Gamepad.current.buttonWest.isPressed || Gamepad.current.dpad.left.isPressed || Gamepad.current.dpad.right.isPressed || Gamepad.current.dpad.up.isPressed || Gamepad.current.dpad.down.isPressed || Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.025f || Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.025f))
			{
				inputMode = InputMode.Gamepad;
			}
			if (Gamepad.current != null)
			{
				MapUnityButtonControlToButton(ref Menu, Gamepad.current.startButton);
				MapUnityButtonControlToButton(ref MenuBack, Gamepad.current.buttonEast);
				MapUnityButtonControlToButton(ref MenuBack, Gamepad.current.buttonNorth);
				MapUnityButtonControlToButton(ref MenuNextPage, Gamepad.current.rightTrigger);
				MapUnityButtonControlToButton(ref MenuNextPage, Gamepad.current.rightShoulder);
				MapUnityButtonControlToButton(ref MenuPreviousPage, Gamepad.current.leftTrigger);
				MapUnityButtonControlToButton(ref MenuPreviousPage, Gamepad.current.leftShoulder);
				MapUnityButtonControlToButton(ref Character, Gamepad.current.selectButton);
				MapUnityButtonControlToButton(ref Action, Gamepad.current.buttonSouth);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.buttonEast);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.buttonWest);
				MapUnityVector2ToAxis(ref MoveHorizontal, ref MoveVertical, Gamepad.current.leftStick.ReadValue());
				MapUnityVector2ToAxis(ref AimHorizontal, ref AimVertical, Gamepad.current.rightStick.ReadValue() * lookStickSensitivity);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.rightShoulder);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.leftShoulder);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.rightTrigger);
				MapUnityButtonControlToButton(ref SecondaryAction, Gamepad.current.leftTrigger);
				MapUnityVector2ToAxis(ref MoveHorizontal, ref MoveVertical, new Vector2(Gamepad.current.dpad.right.value - Gamepad.current.dpad.left.value, Gamepad.current.dpad.up.value - Gamepad.current.dpad.down.value));
				ButtonState b = ButtonState.Idle;
				MapUnityButtonControlToButton(ref b, Gamepad.current.rightTrigger);
				MapUnityButtonControlToButton(ref b, Gamepad.current.rightShoulder);
				ButtonState b2 = ButtonState.Idle;
				MapUnityButtonControlToButton(ref b2, Gamepad.current.leftTrigger);
				MapUnityButtonControlToButton(ref b2, Gamepad.current.leftShoulder);
				UIScroll += ((b == ButtonState.Pressed) ? (Time.unscaledDeltaTime * 16f) : 0f);
				UIScroll -= ((b2 == ButtonState.Pressed) ? (Time.unscaledDeltaTime * 16f) : 0f);
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(worldMouse, 0.5f);
	}
}
