using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlendInModelUI : MonoBehaviour
{
	public Transform model;

	public RectTransform uiContainer;

	protected Quaternion baseRotation;

	protected Vector3 basePosition;

	public Material backgroundMaterial;

	public bool animateIfInDialog;

	public bool enableManualClose = true;

	protected bool close;

	protected bool pauseTime;

	protected Vector3 startingPositionOffset = -Vector3.up * 4f;

	protected Quaternion startingRotationOffset = Quaternion.identity;

	protected string valueModifierId = "blendInModelUI";

	public virtual void Awake()
	{
		baseRotation = model.localRotation;
		basePosition = model.localPosition;
		base.transform.SetParent(Camera.main.transform);
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		GetComponentInChildren<Canvas>().worldCamera = Camera.main;
		model.localPosition += startingPositionOffset;
		model.localRotation *= startingRotationOffset;
		backgroundMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
		if ((bool)uiContainer)
		{
			uiContainer.anchoredPosition += -Vector2.up * 1024f;
		}
		InputManager.DisableInputs(1f);
	}

	public virtual void Close()
	{
		close = true;
		UILayer.Close(this);
		InputManager.ConsumeInputs();
		TimeManager.Get()?.paused.PopAll(valueModifierId);
		CanvasGroup canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public virtual void Start()
	{
		SoundEffectManager.Play("mysc.brush");
		UILayer.Open(this);
		InputManager.ConsumeInputs();
		if (pauseTime)
		{
			TimeManager.Get()?.paused.Push(valueModifierId, value: true);
		}
	}

	public bool IsResponsive()
	{
		if (((bool)DialogManager.Get() && DialogManager.Get().IsInDialog()) || close)
		{
			return false;
		}
		if (!UILayer.IsActive(this))
		{
			return false;
		}
		return true;
	}

	public virtual void Update()
	{
		Color color = backgroundMaterial.GetColor("_Color");
		if (close)
		{
			Vector3 vector = basePosition + startingPositionOffset;
			model.localPosition = Vector3.Lerp(model.localPosition, vector, Time.unscaledDeltaTime * 4f);
			model.localRotation = Quaternion.Lerp(model.localRotation, baseRotation * startingRotationOffset, Time.unscaledDeltaTime * 4f);
			if ((bool)uiContainer)
			{
				uiContainer.anchoredPosition = Vector3.Lerp(uiContainer.anchoredPosition, -Vector2.up * 1024f, Time.unscaledDeltaTime * 4f);
			}
			if ((model.localPosition - vector).sqrMagnitude < 0.1f)
			{
				Object.Destroy(base.gameObject);
			}
			backgroundMaterial.SetColor("_Color", Color.Lerp(color, new Color(0f, 0f, 0f, 0f), Time.unscaledDeltaTime * 8f));
			return;
		}
		backgroundMaterial.SetColor("_Color", Color.Lerp(color, new Color(0f, 0f, 0f, 0.8f), Time.unscaledDeltaTime * 2f));
		if (animateIfInDialog || !DialogManager.Get().IsInDialog())
		{
			model.localRotation = Quaternion.Lerp(model.localRotation, baseRotation, Time.unscaledDeltaTime);
			model.localPosition = Vector3.Lerp(model.localPosition, basePosition, Time.unscaledDeltaTime * 4f);
			if ((bool)uiContainer)
			{
				uiContainer.anchoredPosition = Vector3.Lerp(uiContainer.anchoredPosition, Vector3.zero, Time.unscaledDeltaTime * 4f);
			}
		}
		if (IsResponsive())
		{
			if (enableManualClose && (InputManager.MenuBack == ButtonState.Down || InputManager.Menu == ButtonState.Down))
			{
				Close();
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (UILayer.IsActive(this) && InputManager.inputMode == InputMode.Gamepad && (currentSelectedGameObject == null || !currentSelectedGameObject.activeInHierarchy))
			{
				GetComponentInChildren<Selectable>()?.Select();
			}
		}
	}
}
