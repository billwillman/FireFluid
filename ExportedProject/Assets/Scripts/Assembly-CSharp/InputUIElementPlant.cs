using UnityEngine;
using UnityEngine.UI;

public class InputUIElementPlant : InputUIElement
{
	private PlantUI plant;

	public override void Update()
	{
		base.Update();
		if (!(plant == null))
		{
			actionText.text = plant.GetInteractionToolTip();
		}
	}

	public void LateUpdate()
	{
		bool activeInHierarchy = inputGraphic.gameObject.activeInHierarchy;
		if (!plant.CanInteract())
		{
			inputGraphic.gameObject.SetActive(value: false);
		}
		else
		{
			inputGraphic.gameObject.SetActive(value: true);
		}
		if (activeInHierarchy ^ inputGraphic.gameObject.activeInHierarchy)
		{
			LayoutGroup[] componentsInChildren = GetComponentsInChildren<LayoutGroup>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(componentsInChildren[i].GetComponent<RectTransform>());
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<LayoutGroup>().GetComponent<RectTransform>());
		}
	}

	public override bool IsShowing()
	{
		if ((bool)characterController && !characterController.enableControlls.GetCurrentValue())
		{
			return false;
		}
		if ((bool)DialogManager.Get() && DialogManager.Get().IsInDialog())
		{
			return false;
		}
		plant = null;
		foreach (object value in UILayer.layers.Values)
		{
			if (value is PlantUI)
			{
				plant = (PlantUI)value;
			}
		}
		if (plant != null)
		{
			return true;
		}
		return false;
	}
}
