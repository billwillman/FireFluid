public class InputUIElementAttack : InputUIElement
{
	public override void Update()
	{
		base.Update();
		Ability ability = null;
		foreach (Ability ability2 in characterController.GetAbilities())
		{
			if (ability2.slot == AbilitySlot.Attack)
			{
				ability = ability2;
				break;
			}
		}
		actionText.text = StringLocalization.Get("inputaction_attack");
		if (ability != null)
		{
			actionText.text = ability.GetDisplayName();
		}
		if (InputManager.AimPressedThisFrame())
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
