using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeCardCollectionUI : MonoBehaviour, IUpgradeSelection
{
	public Image background;

	public UpgradeCardUI cursorSelected;

	private UpgradeCardUI cursorSelectedHold;

	public int gridX = 5;

	public Vector2 gridOffset = new Vector2(0f, -256f);

	public Vector2 cardOffsets = new Vector2(16f, 32f);

	public float cardScale = 0.5f;

	public float upperBorder;

	public UpgradeCardUI prefab;

	private List<UpgradeCardUI> upgradeCards = new List<UpgradeCardUI>();

	private List<UpgradeCardUI> selection = new List<UpgradeCardUI>();

	public UnityEvent<UpgradeEntry, bool> OnSelected;

	public UnityEvent<UpgradeEntry, List<UpgradeCardUI>, UpgradeCardUI> CheckSelection;

	public UnityEvent<List<UpgradeCardUI>> OnValueChanged;

	private RectTransform canvas;

	private float scrollOffsetY;

	[HideInInspector]
	public List<Upgrade> upgradesOverride = new List<Upgrade>();

	public bool showNonRunUpgrades;

	public bool showNonLevelableUpgrades = true;

	public bool scrollable = true;

	public UpgradeCardCollectionUISelectionType selectionType = UpgradeCardCollectionUISelectionType.Multiple;

	public bool closeOnMenuKey = true;

	private bool closed;

	public float chargeSelectTime = -1f;

	private float currentChargeSelectTime;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	public ValueStackBool enableSelection = new ValueStackBool(defaultValue: true);

	public float noInputTime;

	private void Start()
	{
		canvas = GetComponent<RectTransform>();
		List<Upgrade> upgrades = Encyclopedia.Get().upgrades;
		if (upgradesOverride.Count > 0)
		{
			upgrades = upgradesOverride;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (Upgrade item in upgrades)
		{
			if ((showNonRunUpgrades || item.canBeFoundDuringRun) && (showNonLevelableUpgrades || item.GetMaxLevel() > 1))
			{
				UpgradeCardUI component = Object.Instantiate(prefab.gameObject, base.transform).GetComponent<UpgradeCardUI>();
				component.transform.SetSiblingIndex(num + 1);
				UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(item.GetIdentifier());
				CheckSelection.Invoke(upgradeEntry, selection, component);
				component.SetUp(item);
				upgradeCards.Add(component);
				if (selectionType == UpgradeCardCollectionUISelectionType.Single)
				{
					cinematicMachine.PushAction(CardUIAnimations.CreateRevealAnimation(component.transform, (float)(num2 + num3) * 0.05f));
				}
				else
				{
					cinematicMachine.PushAction(CardUIAnimations.CreateRevealAnimationDiagonalFlip(component.transform, (float)(num2 + num3) * 0.05f));
				}
				num++;
				num2 = num % gridX;
				num3 = num / gridX;
			}
		}
		OnValueChanged.Invoke(upgradeCards);
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			upgradeCard.UpdateState();
		}
		UpdatePositions(1f);
		TimeManager.Get()?.paused.Push("UpgradeCardCollectionUI", value: true);
		GetComponent<Canvas>().worldCamera = Camera.main;
		GetComponent<Canvas>().planeDistance = 2f;
		GetComponent<Canvas>().sortingLayerName = "UI";
		UILayer.Open(this);
		InputManager.ConsumeInputs();
	}

	public void QueueRandomize(List<Upgrade> availableUpgrades)
	{
		enableSelection.Push("UpgradeCardCollectionUI", value: false);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			PushRandomize(availableUpgrades);
			return true;
		}));
	}

	private void PushRandomize(List<Upgrade> availableUpgrades)
	{
		int num = 32;
		foreach (UpgradeCardUI cardUI in upgradeCards)
		{
			for (int i = 0; i < num; i++)
			{
				cinematicMachine.PushAction(new CinematicWait(Mathf.Pow(1f * (float)i / (float)(num - 1), 16f)));
				if (i < num - 1)
				{
					cinematicMachine.PushAction(new CinematicFunction(delegate
					{
						Upgrade up = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
						cardUI.transform.localScale = Vector3.one * cardScale * 1.1f;
						cardUI.SetUp(up);
						cardUI.UpdateState();
						SoundEffectManager.Play("mysc.click", 2f);
						return true;
					}));
					continue;
				}
				cinematicMachine.PushAction(new CinematicFunction(delegate
				{
					Upgrade upgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
					cardUI.transform.localScale = Vector3.one * cardScale * 1.1f;
					Upgrade upgrade2 = UpgradeManager.Get().InstantiateUpgrade(upgrade);
					upgrade2.OnDraw();
					cardUI.SetUp(upgrade2);
					cardUI.UpdateState();
					SoundEffectManager.Play("mysc.click", 2f);
					SoundEffectManager.Play("mysc.drum");
					return true;
				}));
			}
		}
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			enableSelection.PopAll("UpgradeCardCollectionUI");
			return true;
		}));
	}

	public Vector3 GetTargetPosition(int i, float scrollOffset)
	{
		UpgradeCardUI upgradeCardUI = upgradeCards[i];
		Vector2 sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;
		return new Vector3(((float)(i % gridX) - (float)(gridX - 1) * 0.5f) * (cardOffsets.x + sizeDelta.x * upgradeCardUI.transform.localScale.x), (float)(-(i / gridX)) * (cardOffsets.y + sizeDelta.y * upgradeCardUI.transform.localScale.y), 0f) + new Vector3(gridOffset.x, gridOffset.y, 0f) + Vector3.up * scrollOffset;
	}

	private void UpdatePositions(float dt)
	{
		float num = (0f - canvas.sizeDelta.y) * 0.5f;
		float num2 = canvas.sizeDelta.y * 0.5f - upperBorder;
		int num3 = 0;
		_ = prefab.GetComponent<RectTransform>().sizeDelta;
		if (InputManager.inputMode == InputMode.KeyboardAndMouse && scrollable)
		{
			scrollOffsetY -= Mouse.current.scroll.value.y * 24f;
		}
		if (upgradeCards.Count > 12)
		{
			scrollOffsetY = Mathf.Max(scrollOffsetY, (0f - upperBorder) * 0.5f + 20f);
			scrollOffsetY = Mathf.Min(scrollOffsetY, 0f - GetTargetPosition(upgradeCards.Count - 1, 0f).y - 180f);
		}
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			upgradeCard.transform.localScale = Vector3.Lerp(upgradeCard.transform.localScale, Vector3.one * cardScale, Mathf.Clamp01(8f * dt));
			Vector3 targetPosition = GetTargetPosition(num3, scrollOffsetY);
			if (selection.Contains(upgradeCard))
			{
				targetPosition += Vector3.up * 16f;
			}
			upgradeCard.transform.localPosition = Vector3.Lerp(upgradeCard.transform.localPosition, targetPosition, Mathf.Clamp01(dt * 8f));
			if (upgradeCard.IsSelected())
			{
				RectTransform component = upgradeCard.GetComponent<RectTransform>();
				Vector2 vector = new Vector2(targetPosition.y - component.sizeDelta.y * upgradeCard.transform.localScale.y * 0.5f, targetPosition.y + component.sizeDelta.y * upgradeCard.transform.localScale.y * 0.5f);
				if (vector.x < num)
				{
					scrollOffsetY += num - vector.x;
				}
				if (vector.y > num2)
				{
					scrollOffsetY -= vector.y - num2;
				}
			}
			num3++;
		}
	}

	public void UpdateCursorNavigationButtons()
	{
		if (cursorSelected == null && upgradeCards.Count > 0)
		{
			cursorSelected = upgradeCards[0];
			cursorSelected.OnSelect();
		}
		int num = upgradeCards.IndexOf(cursorSelected);
		if (InputManager.MoveRightPressedThisFrame())
		{
			num++;
			if (num % gridX == 0)
			{
				num -= gridX;
			}
			else if (num >= upgradeCards.Count)
			{
				num -= num % gridX;
			}
		}
		if (InputManager.MoveLeftPressedThisFrame())
		{
			num--;
			if ((num + gridX) % gridX == gridX - 1)
			{
				num += gridX;
			}
			if (num >= upgradeCards.Count)
			{
				num = upgradeCards.Count - 1;
			}
		}
		if (InputManager.MoveUpPressedThisFrame())
		{
			num -= gridX;
			if (num < 0)
			{
				num += (upgradeCards.Count / gridX + 1) * gridX;
			}
			if (num >= upgradeCards.Count)
			{
				num -= gridX;
			}
		}
		if (InputManager.MoveDownPressedThisFrame())
		{
			num += gridX;
			if (num >= upgradeCards.Count)
			{
				num %= gridX;
			}
		}
		if (upgradeCards[num] != cursorSelected)
		{
			cursorSelected.OnDeselect();
			cursorSelected = upgradeCards[num];
			cursorSelected.OnSelect();
		}
	}

	private void UpdateCursorNavigationMouse()
	{
		if (cursorSelected == null && upgradeCards.Count > 0)
		{
			cursorSelected = upgradeCards[0];
			cursorSelected.OnSelect();
		}
		Vector2 screenPoint = Mouse.current.position.ReadValue();
		int num = -1;
		for (int num2 = upgradeCards.Count - 1; num2 >= 0; num2--)
		{
			RectTransform component = upgradeCards[num2].GetComponent<RectTransform>();
			bool flag = upgradeCards[num2] == cursorSelected;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(component, screenPoint, Camera.main, out var localPoint))
			{
				Rect rect = component.rect;
				if (flag)
				{
					float num3 = 32f;
					rect.xMin -= num3;
					rect.xMax += num3;
					rect.yMin -= num3;
					rect.yMax += num3;
				}
				if (rect.Contains(localPoint))
				{
					num = num2;
					break;
				}
			}
		}
		if (num != -1 && upgradeCards[num] != cursorSelected)
		{
			cursorSelected.OnDeselect();
			cursorSelected = upgradeCards[num];
			cursorSelected.OnSelect();
		}
	}

	private void UpdateSelection()
	{
		if (InputManager.Action == ButtonState.Down)
		{
			cursorSelectedHold = cursorSelected;
			currentChargeSelectTime = 0f;
			if (cursorSelected.IsLocked())
			{
				cursorSelectedHold = null;
				cinematicMachine.PushAction(CardUIAnimations.CreateLockedSelectedAnimation(cursorSelected.transform));
			}
		}
		if (InputManager.Action == ButtonState.Pressed)
		{
			currentChargeSelectTime += Time.unscaledDeltaTime;
			if (cursorSelectedHold != cursorSelected)
			{
				cursorSelectedHold = null;
				currentChargeSelectTime = 0f;
			}
			else
			{
				cursorSelectedHold.transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * currentChargeSelectTime * 0.1f * Time.unscaledDeltaTime;
			}
		}
		if (!(currentChargeSelectTime > chargeSelectTime) || !(cursorSelectedHold == cursorSelected))
		{
			return;
		}
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(cursorSelected.upgrade.GetIdentifier());
		switch (selectionType)
		{
		case UpgradeCardCollectionUISelectionType.Single:
			selection.Clear();
			cinematicMachine.PushAction(CardUIAnimations.CreateHighlightAnimation(cursorSelected.transform));
			OnSelected.Invoke(upgradeEntry, arg1: true);
			break;
		case UpgradeCardCollectionUISelectionType.Multiple:
			if (selection.Contains(cursorSelected))
			{
				selection.Remove(cursorSelected);
				OnSelected.Invoke(upgradeEntry, arg1: false);
			}
			else
			{
				selection.Add(cursorSelected);
				OnSelected.Invoke(upgradeEntry, arg1: true);
			}
			break;
		}
		OnValueChanged.Invoke(upgradeCards);
		cursorSelected.UpdateState();
		cursorSelectedHold = null;
		currentChargeSelectTime = 0f;
	}

	private void Update()
	{
		noInputTime -= Time.unscaledDeltaTime;
		cinematicMachine.UpdateActions(Time.unscaledDeltaTime);
		if (closed)
		{
			if (cinematicMachine.IsBusy())
			{
				Color color = background.color;
				color.a = Mathf.Max(color.a - Time.unscaledDeltaTime * 2f, 0f);
				background.color = color;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if (!UILayer.IsActive(this) || noInputTime > 0f)
			{
				return;
			}
			UpdatePositions(Time.unscaledDeltaTime);
			if (enableSelection.GetCurrentValue())
			{
				if (InputManager.inputMode == InputMode.Gamepad)
				{
					UpdateCursorNavigationButtons();
				}
				else
				{
					UpdateCursorNavigationMouse();
				}
				UpdateSelection();
			}
			if (closeOnMenuKey && (InputManager.Menu == ButtonState.Pressed || InputManager.SecondaryAction == ButtonState.Pressed))
			{
				Close();
			}
		}
	}

	public List<UpgradeCardUI> GetSelection()
	{
		return selection;
	}

	public bool IsAnimating()
	{
		return cinematicMachine.IsBusy();
	}

	public bool IsClosed()
	{
		return closed;
	}

	public void Close()
	{
		if (closed)
		{
			return;
		}
		cinematicMachine.ClearActions();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			if (closeOnMenuKey)
			{
				cinematicMachine.PushAction(CardUIAnimations.CreateMoveAwayAnimation(upgradeCard.transform, (float)(num2 + num3) * 0.15f / (float)upgradeCards.Count));
			}
			else if (upgradeCard == cursorSelected || selection.Contains(upgradeCard))
			{
				cinematicMachine.PushAction(CardUIAnimations.CreateDissolveAnimation(upgradeCard.transform, 0f));
			}
			else
			{
				cinematicMachine.PushAction(CardUIAnimations.CreateMoveAwayAnimation(upgradeCard.transform, (float)(num2 + num3) * 0.15f / (float)upgradeCards.Count));
			}
			num++;
			num2 = num % gridX;
			num3 = num / gridX;
		}
		TimeManager.Get()?.paused.PopAll("UpgradeCardCollectionUI");
		UILayer.Close(this);
		closed = true;
	}

	private void OnDestroy()
	{
	}

	public Upgrade GetSelected()
	{
		return cursorSelected.upgrade;
	}

	public bool Show()
	{
		return !IsClosed();
	}
}
