public class BurdenRewardLevelUp : BurdenReward
{
	public int amount = 1;

	public override string GetDescription()
	{
		return "+ " + amount + " " + StringLocalization.Get("burden_reward_levelup");
	}

	public override void Grant()
	{
		CharacterAttributesPlayer component = Player.Get().GetComponent<CharacterAttributesPlayer>();
		if ((bool)component)
		{
			for (int i = 0; i < amount; i++)
			{
				component.GainXP(component.ExperienceNeededToLevelUp() + 1f);
			}
		}
	}
}
