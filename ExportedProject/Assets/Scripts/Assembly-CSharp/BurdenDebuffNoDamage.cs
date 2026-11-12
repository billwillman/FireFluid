public class BurdenDebuffNoDamage : BurdenDebuff
{
	public const string VALUE_MODIFIER_ID = "BurdenDebuffNoDamage";

	public override void Activate()
	{
		Player.Get().GetAttributes().damage.value.Push("BurdenDebuffNoDamage", 0f);
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_nodamage");
	}

	private void OnDestroy()
	{
		Player.Get()?.GetAttributes().damage.value.PopAll("BurdenDebuffNoDamage");
	}
}
