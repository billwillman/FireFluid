public class InputUIElementAllocateBlood : InputUIElement
{
	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_allocateblood");
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
		ShrineOfSacrificeUI shrineOfSacrificeUI = null;
		foreach (object value in UILayer.layers.Values)
		{
			if (value is ShrineOfSacrificeUI)
			{
				shrineOfSacrificeUI = (ShrineOfSacrificeUI)value;
			}
		}
		if (shrineOfSacrificeUI != null)
		{
			return true;
		}
		return false;
	}
}
