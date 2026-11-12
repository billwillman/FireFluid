using System.Collections.Generic;
using UnityEngine;

public class CandlesUI : BlendInModelUI
{
	private int selectedCandleIndex;

	public List<Candle> candles = new List<Candle>();

	public List<Material> lightWorldPosMaterials = new List<Material>();

	private float shake;

	public override void Start()
	{
		int num = 0;
		foreach (Candle candle in candles)
		{
			if (candle.level == PersistentDataComponent.GetDataInstance().currentlySelectedLevel)
			{
				selectedCandleIndex = num;
				break;
			}
			num++;
		}
		LightCandle(selectedCandleIndex);
		base.Start();
		startingPositionOffset = -Vector3.up * 16f;
	}

	public void UpdateButtonNavigation()
	{
		if (InputManager.MoveRightPressedThisFrame())
		{
			selectedCandleIndex++;
		}
		if (InputManager.MoveLeftPressedThisFrame())
		{
			selectedCandleIndex--;
		}
		selectedCandleIndex = Mathf.Clamp(selectedCandleIndex, 0, candles.Count - 1);
		if (InputManager.Action == ButtonState.Down)
		{
			Close();
		}
	}

	public void LightCandle(int index)
	{
		Candle candle = candles[index];
		IInteractableHint[] componentsInChildren;
		foreach (Candle candle2 in candles)
		{
			candle2.burning = false;
			componentsInChildren = candle2.GetComponentsInChildren<IInteractableHint>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEndShowHint();
			}
		}
		candle.burning = true;
		componentsInChildren = candle.GetComponentsInChildren<IInteractableHint>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnStartShowHint(Player.Get().GetComponent<InteractionController>());
		}
		PersistentDataComponent.GetDataInstance().currentlySelectedLevel = candle.level;
	}

	public void SetMaterialLightPos()
	{
		Candle candle = candles[selectedCandleIndex];
		foreach (Material lightWorldPosMaterial in lightWorldPosMaterials)
		{
			lightWorldPosMaterial.SetVector("_LightWorldPos", candle.flame.transform.position);
		}
	}

	public override void Update()
	{
		base.Update();
		model.transform.localPosition += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Mathf.Max(shake, 0f);
		shake -= Time.deltaTime;
		if (!IsResponsive())
		{
			return;
		}
		int num = selectedCandleIndex;
		UpdateButtonNavigation();
		if (num != selectedCandleIndex)
		{
			Candle candle = candles[selectedCandleIndex];
			if (PersistentDataComponent.GetDataInstance().IsLevelUnlocked(candle.level))
			{
				LightCandle(selectedCandleIndex);
			}
			else
			{
				selectedCandleIndex = num;
				SoundEffectManager.Play("mysc.locked");
				shake = 0.1f;
			}
		}
		SetMaterialLightPos();
		_ = candles[selectedCandleIndex];
	}
}
