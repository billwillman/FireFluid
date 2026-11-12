public class InputUIElementHarvest : InputUIElement
{
	private PlantUI plant;

	public override void Start()
	{
		base.Start();
		_ = plant == null;
	}

	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("tree_fruit_harvest");
	}

	public override bool IsShowing()
	{
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
		if (plant == null)
		{
			return false;
		}
		if (plant.GetCurrentlySelected() == null)
		{
			return false;
		}
		if (!UILayer.IsActive(plant))
		{
			return false;
		}
		if (plant.GetCurrentlySelected().GetComponent<Fruit>().IsFullyGrown())
		{
			return true;
		}
		return false;
	}
}
