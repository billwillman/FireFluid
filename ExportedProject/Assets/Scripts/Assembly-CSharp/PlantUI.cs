using TMPro;
using UnityEngine;

public class PlantUI : BlendInModelUI
{
	private WorldButtonController buttonController;

	private InteractableHighlighter highlighter;

	private ProceduralPlant plant;

	public TextMeshProUGUI growText;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	public override void Start()
	{
		base.Start();
		buttonController = GetComponent<WorldButtonController>();
		highlighter = GetComponent<InteractableHighlighter>();
		plant = GetComponentInChildren<ProceduralPlant>();
	}

	public WorldButton GetCurrentlySelected()
	{
		if (buttonController == null)
		{
			return null;
		}
		return buttonController.GetCurrentlySelected();
	}

	public override void Update()
	{
		if (!DialogManager.Get().IsInDialog())
		{
			cinematicMachine.UpdateActions(Time.deltaTime);
		}
		base.Update();
		if (IsResponsive())
		{
			growText.text = ProceduralPlantGrowth.GetCostPerGrow() + "<sprite name=currency> " + StringLocalization.Get("tree_action");
			if (InputManager.Character == ButtonState.Down || InputManager.SecondaryAction == ButtonState.Down || InputManager.MenuNextPage == ButtonState.Down)
			{
				SpendCurrency();
			}
			if (buttonController.GetCurrentlySelected() != null)
			{
				highlighter.DrawInteractableHighlight(buttonController.GetCurrentlySelected());
			}
		}
	}

	private void Shake(float duration, float intensity)
	{
		float t = 0f;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			model.localRotation *= Quaternion.Euler(Random.Range(0f - intensity, intensity), Random.Range(0f - intensity, intensity), Random.Range(0f - intensity, intensity));
			t += dt;
			return (t > duration) ? true : false;
		}));
	}

	public void SpendCurrency()
	{
		int costPerGrow = ProceduralPlantGrowth.GetCostPerGrow();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(costPerGrow))
		{
			PersistentDataComponent.GetDataInstance().growthStepsTree++;
			PersistentDataComponent.GetDataInstance().SpendCurrency(costPerGrow);
			SoundEffectManager.Play("tree.grow");
			SoundEffectManager.Play("tree.growtone");
		}
		else
		{
			SoundEffectManager.Play("mysc.locked");
			Shake(0.1f, 0.2f);
		}
	}

	public string GetInteractionToolTip()
	{
		int costPerGrow = ProceduralPlantGrowth.GetCostPerGrow();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(costPerGrow))
		{
			return StringLocalization.Get("tree_action") + " (" + costPerGrow + "<sprite name=currency>)";
		}
		return "<color=#ff0000>" + StringLocalization.Get("general_notenoughcurrency") + costPerGrow + "</color>";
	}

	public bool CanInteract()
	{
		return PersistentDataComponent.GetDataInstance().CanSpendCurrency(ProceduralPlantGrowth.GetCostPerGrow());
	}
}
