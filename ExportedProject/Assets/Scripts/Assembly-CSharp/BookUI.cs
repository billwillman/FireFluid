using System;
using System.Collections.Generic;
using UnityEngine;

public class BookUI : BlendInModelUI, IUpgradeSelection
{
	public float pageFlipSpeed = 4f;

	public BookPageUI page1;

	public BookPageUI page2;

	public BookPageUI page3;

	public BookPageUI page4;

	public GameObject pageTurnIndicatorRight;

	public GameObject pageTurnIndicatorLeft;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private int pageFlips;

	private List<Upgrade> upgrades = new List<Upgrade>();

	private int selectedPagePartIndex = -1;

	private BookPagePartUI selectedPagePart;

	private BookPagePartUI pressedDownPagePart;

	public Vector3 pageElevation = new Vector3(0f, 0f, -0.1f);

	private float currentInputHoldTime;

	public float inputHoldTime = 2f;

	public GameObject bookSymbolParticleEmitter;

	public UpgradeFilter filter;

	private IUpgradeInteraction interaction;

	public Texture2D emptyTexture;

	public Texture2D denyTexture;

	public Texture2D lockedTexture;

	public Texture2D demoLockedTexture;

	[ColorUsage(true, true)]
	public Color selectedColor = new Color(1f, 0.5f, 0f);

	[ColorUsage(true, true)]
	public Color selectedGreyColor = new Color(0.5f, 0.5f, 0.5f);

	[ColorUsage(true, true)]
	public Color unselectedColor = new Color(1f, 0.5f, 0f);

	[ColorUsage(true, true)]
	public Color noInteractionColor = new Color(0.4f, 0.4f, 0.4f);

	[ColorUsage(true, true)]
	public Color greyColor = new Color(0.3f, 0.3f, 0.3f);

	private bool anyPageTurned;

	public override void Awake()
	{
		startingRotationOffset = Quaternion.Euler(-45f, 0f, 0f);
		base.Awake();
		interaction = GetComponent<IUpgradeInteraction>();
	}

	public override void Start()
	{
		anyPageTurned = PersistentDataComponent.GetDataInstance().GetData("anyPageTurned") != null;
		foreach (Upgrade upgrade in Encyclopedia.Get().upgrades)
		{
			switch (filter)
			{
			case UpgradeFilter.Upgradable:
				if (upgrade.unlockedFromTheStart && upgrade.GetMaxLevel() <= 1)
				{
					continue;
				}
				break;
			case UpgradeFilter.Deniable:
				if (!upgrade.canBeFoundDuringRun)
				{
					continue;
				}
				break;
			}
			upgrades.Add(upgrade);
		}
		page1.book = this;
		page2.book = this;
		page3.book = this;
		page4.book = this;
		ResetAnimationState();
		UpdatePageUpgrades();
		UpdateHighlightColors(100f);
		SelectPagePart(page2.pageParts[6]);
		page2.transform.localPosition = Vector3.zero;
		page4.transform.localPosition = pageElevation;
		page3.SetAnimationTimeRightToLeft(1f);
		float t = 0f;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			float num = 1f - t;
			page2.transform.localPosition = Vector3.Lerp(pageElevation, Vector3.zero, num);
			page4.transform.localPosition = Vector3.Lerp(Vector3.zero, pageElevation, num);
			page3.SetAnimationTimeRightToLeft(num);
			t += dt;
			if (t > 1f)
			{
				ResetAnimationState();
				return true;
			}
			return false;
		}));
		base.Start();
	}

	public override void Close()
	{
		SoundEffectManager.Play("mysc.brush");
		base.Close();
	}

	public float GetCurrentInputHoldTime()
	{
		return currentInputHoldTime;
	}

	public BookPagePartUI GetCurrentSelectedPagePart()
	{
		return selectedPagePart;
	}

	private int GetPagePartIndex(BookPagePartUI pagePart)
	{
		int num = 0;
		foreach (BookPagePartUI pagePart2 in page1.pageParts)
		{
			if (pagePart2 == pagePart)
			{
				return num;
			}
			num++;
		}
		foreach (BookPagePartUI pagePart3 in page2.pageParts)
		{
			if (pagePart3 == pagePart)
			{
				return num;
			}
			num++;
		}
		foreach (BookPagePartUI pagePart4 in page3.pageParts)
		{
			if (pagePart4 == pagePart)
			{
				return num;
			}
			num++;
		}
		foreach (BookPagePartUI pagePart5 in page4.pageParts)
		{
			if (pagePart5 == pagePart)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private BookPagePartUI GetPagePart(int index)
	{
		int num = index / 12;
		int index2 = index % 12;
		return num switch
		{
			0 => page1.pageParts[index2], 
			1 => page2.pageParts[index2], 
			2 => page3.pageParts[index2], 
			3 => page4.pageParts[index2], 
			_ => null, 
		};
	}

	private void SelectPagePart(BookPagePartUI selection, bool playSound = true)
	{
		if (selection == null || !(selection.upgrade == null))
		{
			selectedPagePart = selection;
			selectedPagePartIndex = GetPagePartIndex(selection);
			if (playSound)
			{
				SoundEffectManager.Play("mysc.lowclick");
			}
		}
	}

	private void UpdateButtonNavigation()
	{
		if (selectedPagePart == null || selectedPagePartIndex == -1)
		{
			SelectPagePart(page2.pageParts[6]);
		}
		int num = GetPagePartIndex(selectedPagePart);
		int num2 = num % 6 / 2;
		int num3 = num / 6;
		_ = num % 2;
		if (InputManager.MoveUpPressedThisFrame() && num2 > 0)
		{
			num -= 2;
			SelectPagePart(GetPagePart(num));
		}
		if (InputManager.MoveDownPressedThisFrame() && num2 < 2)
		{
			num += 2;
			SelectPagePart(GetPagePart(num));
		}
		if (InputManager.MoveRightPressedThisFrame())
		{
			if (num % 2 == 0)
			{
				num++;
			}
			else if (num3 % 2 == 1)
			{
				num = num - 1 + 6;
			}
			else if (NextPage())
			{
				num = num - 1 + 6;
			}
			for (int i = 0; i < 6; i++)
			{
				if (GetPagePart(num).upgrade != null)
				{
					break;
				}
				num--;
			}
			SelectPagePart(GetPagePart(num));
		}
		if (InputManager.MoveLeftPressedThisFrame())
		{
			if (num % 2 == 1)
			{
				num--;
			}
			else if (num3 % 2 == 0)
			{
				num = num + 1 - 6;
			}
			else if (PreviousPage())
			{
				num = num + 1 - 6;
			}
			SelectPagePart(GetPagePart(num));
		}
		if (InputManager.MenuNextPage == ButtonState.Down)
		{
			NextPage();
		}
		if (InputManager.MenuPreviousPage == ButtonState.Down)
		{
			PreviousPage();
		}
	}

	public override void Update()
	{
		if (!DialogManager.Get().IsInDialog())
		{
			cinematicMachine.UpdateActions(Time.deltaTime);
		}
		base.Update();
		if (!IsResponsive())
		{
			pageTurnIndicatorLeft.SetActive(value: false);
			pageTurnIndicatorRight.SetActive(value: false);
			return;
		}
		pageTurnIndicatorLeft.SetActive(HasPreviousPage());
		pageTurnIndicatorRight.SetActive(HasNextPage());
		if (!anyPageTurned)
		{
			page3.SetAnimationTimeRightToLeft(Mathf.Clamp01(Mathf.Sin(Time.unscaledTime * 2f) - 0.7f));
		}
		BookPagePartUI bookPagePartUI = selectedPagePart;
		if (!DialogManager.Get().IsInDialog() && !cinematicMachine.IsBusy())
		{
			UpdateButtonNavigation();
		}
		if (selectedPagePart != null && bookPagePartUI == selectedPagePart && selectedPagePart.selectable)
		{
			if (InputManager.Action == ButtonState.Down)
			{
				pressedDownPagePart = selectedPagePart;
			}
			else if (InputManager.Action == ButtonState.Pressed && pressedDownPagePart == selectedPagePart)
			{
				currentInputHoldTime += Time.unscaledDeltaTime;
				if (currentInputHoldTime > 0.1f)
				{
					SoundEffectManager.Play("mysc.magic", currentInputHoldTime / inputHoldTime);
				}
			}
			else
			{
				currentInputHoldTime = 0f;
			}
		}
		else
		{
			currentInputHoldTime = 0f;
		}
		if (selectedPagePart != null && InputManager.Action == ButtonState.Down && !selectedPagePart.selectable)
		{
			SoundEffectManager.Play("mysc.locked");
			Shake(0.1f, 0.2f);
		}
		float num = currentInputHoldTime;
		model.localRotation *= Quaternion.Euler(UnityEngine.Random.Range(0f - num, num), UnityEngine.Random.Range(0f - num, num), UnityEngine.Random.Range(0f - num, num));
		if (currentInputHoldTime > inputHoldTime)
		{
			currentInputHoldTime = 0f;
			InputPressed(selectedPagePart);
			pressedDownPagePart = null;
		}
	}

	private void InputPressed(BookPagePartUI pagePart)
	{
		ParticleSystem[] componentsInChildren = UnityEngine.Object.Instantiate(bookSymbolParticleEmitter).GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem.ShapeModule shape = componentsInChildren[i].shape;
			shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
			shape.skinnedMeshRenderer = pagePart.rendererGameObject.GetComponent<SkinnedMeshRenderer>();
			shape.texture = pagePart.upgrade.icon.texture;
			Camera.main.GetComponent<CameraController>().Shake(0.1f);
		}
		interaction.OnUpgradeInteraction(pagePart.upgrade);
		SoundEffectManager.Play("mysc.impact");
		UpdatePageUpgrades();
		UpdateHighlightColors(100f);
	}

	private void UpdatePagePartState(BookPagePartUI pagePart)
	{
		pagePart.selectable = interaction.CanInteractUpgrade(pagePart.upgrade);
		if (PersistentDataComponent.GetDataInstance() != null && pagePart.upgrade != null && PersistentDataComponent.GetDataInstance().GetUpgradeEntry(pagePart.upgrade.GetIdentifier()).level > 0)
		{
			pagePart.locked = false;
		}
		else
		{
			pagePart.locked = true;
		}
	}

	private void ResetAnimationState()
	{
		page1.ResetAnimationLeft();
		page1.transform.localPosition = Vector3.zero;
		page2.ResetAnimationLeft();
		page2.transform.localPosition = pageElevation;
		page3.ResetAnimationRight();
		page3.transform.localPosition = pageElevation;
		page4.ResetAnimationRight();
		page4.transform.localPosition = Vector3.zero;
	}

	private void Shake(float duration, float intensity)
	{
		float t = 0f;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			_ = currentInputHoldTime;
			model.localRotation *= Quaternion.Euler(UnityEngine.Random.Range(0f - intensity, intensity), UnityEngine.Random.Range(0f - intensity, intensity), UnityEngine.Random.Range(0f - intensity, intensity));
			t += dt;
			return (t > duration) ? true : false;
		}));
	}

	public void UpdatePageUpgrades()
	{
		page1.UpdateUpgrades(upgrades, -18 + pageFlips * 6 * 2);
		foreach (BookPagePartUI pagePart in page1.pageParts)
		{
			UpdatePagePartState(pagePart);
		}
		page2.UpdateUpgrades(upgrades, -6 + pageFlips * 6 * 2);
		foreach (BookPagePartUI pagePart2 in page2.pageParts)
		{
			UpdatePagePartState(pagePart2);
		}
		page3.UpdateUpgrades(upgrades, 6 + pageFlips * 6 * 2);
		foreach (BookPagePartUI pagePart3 in page3.pageParts)
		{
			UpdatePagePartState(pagePart3);
		}
		page4.UpdateUpgrades(upgrades, 18 + pageFlips * 6 * 2);
		foreach (BookPagePartUI pagePart4 in page4.pageParts)
		{
			UpdatePagePartState(pagePart4);
		}
	}

	public void UpdateHighlightColors(float t)
	{
		page1.UpdateColors(100f);
		page2.UpdateColors(100f);
		page3.UpdateColors(100f);
		page4.UpdateColors(100f);
	}

	public bool HasNextPage()
	{
		if ((pageFlips + 1) * 6 * 2 >= upgrades.Count)
		{
			return false;
		}
		return true;
	}

	public bool NextPage()
	{
		if (!HasNextPage())
		{
			return false;
		}
		anyPageTurned = true;
		float t = 0f;
		SoundEffectManager.Play("mysc.pageturn");
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			page2.transform.localPosition = Vector3.Lerp(pageElevation, Vector3.zero, t);
			page4.transform.localPosition = Vector3.Lerp(Vector3.zero, pageElevation, t);
			page3.SetAnimationTimeRightToLeft(t);
			t += dt * pageFlipSpeed;
			return t > 1f;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ResetAnimationState();
			pageFlips++;
			selectedPagePartIndex -= 12;
			selectedPagePartIndex = Math.Clamp(selectedPagePartIndex, 18, 29);
			UpdatePageUpgrades();
			SelectPagePart(GetPagePart(selectedPagePartIndex), playSound: false);
			UpdateHighlightColors(100f);
			return true;
		}));
		return true;
	}

	public bool HasPreviousPage()
	{
		if (pageFlips <= 0)
		{
			return false;
		}
		return true;
	}

	public bool PreviousPage()
	{
		if (!HasPreviousPage())
		{
			return false;
		}
		float t = 0f;
		SoundEffectManager.Play("mysc.pageturn");
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			page1.transform.localPosition = Vector3.Lerp(Vector3.zero, pageElevation, t);
			page3.transform.localPosition = Vector3.Lerp(pageElevation, Vector3.zero, t);
			page2.SetAnimationTimeLeftToRight(t);
			t += dt * pageFlipSpeed;
			return t > 1f;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ResetAnimationState();
			pageFlips--;
			selectedPagePartIndex += 12;
			selectedPagePartIndex = Math.Clamp(selectedPagePartIndex, 18, 29);
			UpdatePageUpgrades();
			SelectPagePart(GetPagePart(selectedPagePartIndex), playSound: false);
			UpdateHighlightColors(100f);
			return true;
		}));
		return true;
	}

	public void UINextPage()
	{
		if (!cinematicMachine.IsBusy())
		{
			NextPage();
		}
	}

	public void UIPreviousPage()
	{
		if (!cinematicMachine.IsBusy())
		{
			PreviousPage();
		}
	}

	public bool IsBusy()
	{
		return cinematicMachine.IsBusy();
	}

	public Upgrade GetSelected()
	{
		return selectedPagePart.upgrade;
	}

	public bool Show()
	{
		return !close;
	}

	public string GetInteractionToolTip()
	{
		return interaction.GetInteractionToolTip(GetSelected());
	}

	public bool CanInteract()
	{
		return interaction.CanInteractUpgrade(GetSelected());
	}
}
