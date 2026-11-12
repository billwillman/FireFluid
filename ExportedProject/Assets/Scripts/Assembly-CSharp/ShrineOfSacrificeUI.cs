using System.Collections.Generic;
using UnityEngine;

public class ShrineOfSacrificeUI : BlendInModelUI
{
	public GameObject sacrificeCardPrefab;

	public Transform sacrificeCardContainer;

	private int selectionIndex;

	private List<SacrificeCardUI> cards = new List<SacrificeCardUI>();

	private FluidSurface fluidSurface;

	public Transform knife;

	private Quaternion knifeRotation;

	private Vector3 knifePosition;

	private float knifeShake;

	public override void Start()
	{
		base.Start();
		foreach (Sacrifice sacrifice in Encyclopedia.Get().sacrifices)
		{
			SacrificeCardUI component = Object.Instantiate(sacrificeCardPrefab, sacrificeCardContainer).GetComponent<SacrificeCardUI>();
			component.SetUp(sacrifice);
			cards.Add(component);
		}
		cards[0].OnSelect();
		fluidSurface = GetComponentInChildren<FluidSurface>();
		knifeRotation = knife.transform.localRotation;
		knifePosition = knife.transform.localPosition;
		startingPositionOffset = -Vector3.up * 8f;
	}

	public void UpdateButtonNavigation()
	{
		if (InputManager.MoveUpPressedThisFrame())
		{
			selectionIndex--;
		}
		if (InputManager.MoveDownPressedThisFrame())
		{
			selectionIndex++;
		}
		selectionIndex = Mathf.Clamp(selectionIndex, 0, cards.Count - 1);
		if (InputManager.MenuNextPage == ButtonState.Down || InputManager.Action == ButtonState.Down || InputManager.AimRightPressedThisFrame() || InputManager.MoveRightPressedThisFrame())
		{
			SacrificeCardUI selectedCard = GetSelectedCard();
			if (selectedCard.sacrifice.CanLevelUp())
			{
				selectedCard.sacrifice.LevelUp();
				selectedCard.UpdateState();
				fluidSurface.SpawnDrop();
				knifeShake = 0.2f;
				SoundEffectManager.Play("sacrifice.hurt");
			}
			else
			{
				SoundEffectManager.Play("mysc.locked");
			}
		}
		if (InputManager.MenuPreviousPage == ButtonState.Down || InputManager.AimLeftPressedThisFrame() || InputManager.MoveLeftPressedThisFrame())
		{
			SacrificeCardUI selectedCard2 = GetSelectedCard();
			if (selectedCard2.sacrifice.CanLevelDown())
			{
				SoundEffectManager.Play("mysc.flick");
				selectedCard2.sacrifice.LevelDown();
				selectedCard2.UpdateState();
			}
			else
			{
				SoundEffectManager.Play("mysc.locked");
			}
		}
	}

	public SacrificeCardUI GetSelectedCard()
	{
		return cards[selectionIndex];
	}

	public override void Update()
	{
		base.Update();
		if (!IsResponsive())
		{
			return;
		}
		Vector3 vector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		knife.localRotation = knifeRotation * Quaternion.Euler(vector * knifeShake * 48f);
		knife.localPosition = knifePosition + vector * knifeShake * 0.5f;
		knifeShake = Mathf.Max(knifeShake - Time.deltaTime, 0f);
		int num = selectionIndex;
		UpdateButtonNavigation();
		if (num != selectionIndex)
		{
			if (num >= 0 && num < cards.Count)
			{
				cards[num].OnDeselect();
			}
			cards[selectionIndex].OnSelect();
		}
	}
}
