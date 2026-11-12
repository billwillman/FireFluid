public class CharacterControllerCopy : CharacterController, IAbilityController
{
	public CharacterController target;

	private void Awake()
	{
		target = Player.GetNearestPlayer(base.transform.position).GetComponent<CharacterController>();
	}

	protected override void PullInputs()
	{
		worldAimDirection = target.GetAimDirection();
		worldMoveDirection = target.GetMovementIntention();
		if (target.SecondaryActionIntention())
		{
			if (intent_secondary_state == 0 || intent_secondary_state == 3)
			{
				intent_secondary_state = 1;
			}
			else
			{
				intent_secondary_state = 2;
			}
		}
		else if (intent_secondary_state != 3)
		{
			intent_secondary_state = 3;
		}
		else
		{
			intent_secondary_state = 0;
		}
		if (target.AttackIntention())
		{
			if (intent_attack_state == 0 || intent_attack_state == 3)
			{
				intent_attack_state = 1;
			}
			else
			{
				intent_attack_state = 2;
			}
		}
		else if (intent_attack_state != 3)
		{
			intent_attack_state = 3;
		}
		else
		{
			intent_attack_state = 0;
		}
	}
}
