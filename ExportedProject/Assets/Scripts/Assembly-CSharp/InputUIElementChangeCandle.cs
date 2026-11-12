public class InputUIElementChangeCandle : InputUIElement
{
	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_changecandle");
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
		CandlesUI candlesUI = null;
		foreach (object value in UILayer.layers.Values)
		{
			if (value is CandlesUI)
			{
				candlesUI = (CandlesUI)value;
			}
		}
		if (candlesUI != null)
		{
			return true;
		}
		return false;
	}
}
