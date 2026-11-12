public class BurdenRewardUncertain : BurdenRewardRandomAttribute
{
	private bool granted;

	public override void Grant()
	{
		base.Grant();
		granted = true;
	}

	public override void Awake()
	{
		value *= 2f;
		base.Awake();
	}

	public override string GetDescription()
	{
		if (granted)
		{
			return base.GetDescription();
		}
		return StringLocalization.Get("burden_reward_uncertain");
	}
}
