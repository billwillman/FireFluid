public class InputUIElementAdvanceDialog : InputUIElement
{
	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_continue");
	}

	public override bool IsShowing()
	{
		if (DialogManager.Get() == null)
		{
			return false;
		}
		if (DialogManager.Get().GetCurrentStyleInstance() is DialogStyleWithButtons)
		{
			return false;
		}
		return DialogManager.Get().IsWaitingForInput();
	}
}
