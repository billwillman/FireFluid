public class BurdenDebuffLifeDrain : BurdenDebuff
{
	public const string VALUE_MODIFIER_ID = "BurdenDebuffLifeDrain";

	public override void Activate()
	{
		Player.Get().GetAttributes().health.regeneration.Push("BurdenDebuffLifeDrain", -5f, ValueModifierType.Add);
	}

	private void Update()
	{
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_lifedrain");
	}

	private void OnDestroy()
	{
		Player.Get()?.GetAttributes().health.regeneration.PopAll("BurdenDebuffLifeDrain");
	}
}
