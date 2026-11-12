using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class WorldButtonController : MonoBehaviour
{
	public UnityEvent<WorldButton> OnButtonEnter;

	public UnityEvent<WorldButton> OnButtonExit;

	public UnityEvent<WorldButton> OnButtonPressed;

	private WorldButton currentButton;

	public bool enableGamepadSupport;

	public LayerMask layerMask;

	private void Update()
	{
		if (InputManager.inputMode == InputMode.KeyboardAndMouse)
		{
			if (Mouse.current == null)
			{
				return;
			}
			Vector3 pos = InputManager.screenMouse;
			pos.z = Camera.main.nearClipPlane;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out var hitInfo, 1000f, layerMask))
			{
				WorldButton component = hitInfo.collider.GetComponent<WorldButton>();
				if (!component)
				{
					return;
				}
				if (currentButton != component)
				{
					component.OnEnter?.Invoke();
					OnButtonEnter?.Invoke(component);
					if ((bool)currentButton)
					{
						currentButton.OnExit?.Invoke();
						OnButtonExit?.Invoke(component);
					}
					currentButton = component;
				}
				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					component.OnPressed?.Invoke();
					OnButtonPressed?.Invoke(component);
					InputManager.ConsumeInputs();
				}
			}
			else if (currentButton != null)
			{
				currentButton.OnExit?.Invoke();
				OnButtonExit?.Invoke(currentButton);
				currentButton = null;
			}
		}
		else
		{
			if (!enableGamepadSupport || UILayer.GetNumberOfOpenLayers() != 1 || DialogManager.Get().IsInDialog())
			{
				return;
			}
			if (currentButton == null)
			{
				if (WorldButton.all.Count > 0)
				{
					currentButton = WorldButton.all[0];
					currentButton.OnEnter?.Invoke();
					OnButtonEnter?.Invoke(currentButton);
				}
				return;
			}
			int num = WorldButton.all.IndexOf(currentButton);
			int num2 = num;
			if (InputManager.MoveLeftPressedThisFrame())
			{
				num2 = (num + 1) % WorldButton.all.Count;
			}
			else if (InputManager.MoveRightPressedThisFrame())
			{
				num2 = (num + WorldButton.all.Count - 1) % WorldButton.all.Count;
			}
			if (num != num2)
			{
				currentButton.OnExit?.Invoke();
				OnButtonExit?.Invoke(currentButton);
				currentButton = WorldButton.all[num2];
				currentButton.OnEnter?.Invoke();
				OnButtonEnter?.Invoke(currentButton);
			}
			if (InputManager.Action == ButtonState.Down)
			{
				currentButton.OnPressed?.Invoke();
				OnButtonPressed?.Invoke(currentButton);
				InputManager.ConsumeInputs();
			}
		}
	}

	public WorldButton GetCurrentlySelected()
	{
		return currentButton;
	}
}
