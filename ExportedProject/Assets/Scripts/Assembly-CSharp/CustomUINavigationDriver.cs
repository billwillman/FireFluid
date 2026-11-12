using UnityEngine;
using UnityEngine.EventSystems;

public class CustomUINavigationDriver : MonoBehaviour
{
	private EventSystem eventSystem;

	public float analogDeadzone = 0.5f;

	public float initialRepeatDelay = 0.35f;

	public float repeatRate = 0.12f;

	private float nextRepeatTime;

	private MoveDirection lastMoveDir = MoveDirection.None;

	private void Awake()
	{
		base.enabled = false;
	}

	private void Update()
	{
		if (!(eventSystem.currentSelectedGameObject == null))
		{
			HandleNavigation();
			HandleSubmitCancel();
			HandleScroll();
		}
	}

	private void HandleNavigation()
	{
		float moveHorizontal = InputManager.MoveHorizontal;
		float moveVertical = InputManager.MoveVertical;
		if (Mathf.Abs(moveHorizontal) < analogDeadzone && Mathf.Abs(moveVertical) < analogDeadzone)
		{
			lastMoveDir = MoveDirection.None;
			nextRepeatTime = 0f;
			return;
		}
		MoveDirection moveDirection = ((!(Mathf.Abs(moveHorizontal) > Mathf.Abs(moveVertical))) ? ((moveVertical > 0f) ? MoveDirection.Up : MoveDirection.Down) : ((moveHorizontal > 0f) ? MoveDirection.Right : MoveDirection.Left));
		if (moveDirection != lastMoveDir)
		{
			SendMove(moveDirection);
			lastMoveDir = moveDirection;
			nextRepeatTime = Time.unscaledTime + initialRepeatDelay;
		}
		else if (Time.unscaledTime >= nextRepeatTime)
		{
			SendMove(moveDirection);
			nextRepeatTime = Time.unscaledTime + repeatRate;
		}
	}

	private void SendMove(MoveDirection dir)
	{
		AxisEventData eventData = new AxisEventData(eventSystem)
		{
			moveDir = dir
		};
		ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
	}

	private void HandleSubmitCancel()
	{
		if (InputManager.Action == ButtonState.Down)
		{
			BaseEventData eventData = new BaseEventData(eventSystem);
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.submitHandler);
		}
		if (InputManager.MenuBack == ButtonState.Down)
		{
			BaseEventData eventData2 = new BaseEventData(eventSystem);
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, eventData2, ExecuteEvents.cancelHandler);
		}
	}

	private void HandleScroll()
	{
		if (!(Mathf.Abs(InputManager.UIScroll) < 0.01f))
		{
			PointerEventData eventData = new PointerEventData(eventSystem)
			{
				scrollDelta = new Vector2(0f, InputManager.UIScroll)
			};
			ExecuteEvents.ExecuteHierarchy(eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.scrollHandler);
		}
	}
}
