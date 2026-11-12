public class BurdenDebuffXPDamage : BurdenDebuff, IGameEventListener<OnGemCollectedParams>
{
	public const string ATTRIBUTE_MODIFIER_ID = "BurdenDebuffXPDamage";

	public float damagePerGem = 10f;

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnGemCollectedParams eventParameters)
	{
		eventParameters.collector.ReceiveDamage(damagePerGem, eventParameters.collector, "BurdenDebuffXPDamage");
		return IGameEventResult.SUCCESS;
	}

	public override void Activate()
	{
		GameEventManager.Get().OnGemCollected.AddListener(this);
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_xpdamage");
	}

	private void OnDestroy()
	{
		GameEventManager.Get()?.OnGemCollected.RemoveListener(this);
	}
}
