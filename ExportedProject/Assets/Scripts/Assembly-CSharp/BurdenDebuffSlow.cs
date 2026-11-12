public class BurdenDebuffSlow : BurdenDebuff
{
	public const string ATTRIBUTE_MODIFIER_ID = "BurdenDebuffSlow";

	public float movementSpeedMultiplier = 0.2f;

	public override void Activate()
	{
		Player.Get().GetAttributes().movementSpeed.value.Push("BurdenDebuffSlow", movementSpeedMultiplier, ValueModifierType.Multiply);
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_slow");
	}

	private void OnDestroy()
	{
		Player.Get()?.GetAttributes().movementSpeed.value.PopAll("BurdenDebuffSlow");
	}
}
