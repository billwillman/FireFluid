public class InputUIElementSecondary : InputUIElement
{
	public override void Update()
	{
		base.Update();
		Ability ability = null;
		foreach (Ability ability2 in characterController.GetAbilities())
		{
			if (ability2.slot == AbilitySlot.SecondaryAction)
			{
				ability = ability2;
				break;
			}
		}
		actionText.text = StringLocalization.Get("inputaction_secondary");
		if (ability != null)
		{
			actionText.text = ability.GetDisplayName();
		}
		if (InputManager.SecondaryAction == ButtonState.Pressed)
		{
			inputUsed = true;
		}
	}

	public override bool IsShowing()
	{
		if (!base.IsShowing())
		{
			return false;
		}
		if ((bool)RunManager.Get() && RunManager.Get().state == RunState.Ongoing)
		{
			return true;
		}
		return false;
	}
}
