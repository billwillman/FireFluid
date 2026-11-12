using TMPro;
using UnityEngine;

public class ItemUI : BlendInModelUI
{
	public new TextMeshProUGUI name;

	public TextMeshProUGUI description;

	public ItemController itemController;

	public Item item;

	private GameObject detailedModelInstance;

	private Vector2 mouseLastFrame;

	public override void Awake()
	{
		startingPositionOffset = -Vector3.up * 32f;
		pauseTime = true;
		mouseLastFrame = InputManager.screenMouse;
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		detailedModelInstance = Object.Instantiate(item.detailedModel);
		detailedModelInstance.transform.parent = model;
		detailedModelInstance.transform.localPosition = Vector3.zero;
		detailedModelInstance.transform.localRotation = Quaternion.identity;
		detailedModelInstance.SetActive(value: true);
		SoundEffectManager.Play(item.dropSound);
		name.text = item.GetDisplayName();
		description.text = item.GetDescription();
	}

	public override void Update()
	{
		base.Update();
		if (!IsResponsive())
		{
			return;
		}
		if (InputManager.inputMode == InputMode.KeyboardAndMouse)
		{
			if (InputManager.Action == ButtonState.Pressed)
			{
				Vector3 axis = new Vector3(InputManager.screenMouse.y - mouseLastFrame.y, mouseLastFrame.x - InputManager.screenMouse.x, 0f);
				float angle = Time.unscaledDeltaTime * axis.magnitude * 16f;
				detailedModelInstance.transform.localRotation = Quaternion.AngleAxis(angle, axis) * detailedModelInstance.transform.localRotation;
			}
			mouseLastFrame = InputManager.screenMouse;
		}
		if (InputManager.inputMode == InputMode.Gamepad)
		{
			Vector3 axis2 = new Vector3(InputManager.AimVertical, 0f - InputManager.AimHorizontal, 0f);
			float angle2 = Time.unscaledDeltaTime * axis2.magnitude * 64f;
			detailedModelInstance.transform.localRotation = Quaternion.AngleAxis(angle2, axis2) * detailedModelInstance.transform.localRotation;
		}
	}

	public void Accept()
	{
		itemController.Equip(item);
	}
}
