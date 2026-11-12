public class BurdenDebuffDieOnDamage : BurdenDebuff, IGameEventListener<OnCharacterDealingDamageParams>
{
	public const string DAMAGE_SOURCE_ID = "BurdenDebuffDieOnDamage";

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.damageSource == "BurdenDebuffDieOnDamage")
		{
			return IGameEventResult.SUCCESS;
		}
		if (eventParameters.attacker is CharacterAttributesPlayer)
		{
			return IGameEventResult.SUCCESS;
		}
		if (eventParameters.receiver is CharacterAttributesPlayer)
		{
			eventParameters.receiver.ReceiveDamage(9999f, eventParameters.receiver, "BurdenDebuffDieOnDamage");
		}
		return IGameEventResult.SUCCESS;
	}

	public override void Activate()
	{
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_dieondamage");
	}

	private void OnDestroy()
	{
		GameEventManager.Get()?.OnCharacterDealingDamage.RemoveListener(this);
	}
}
