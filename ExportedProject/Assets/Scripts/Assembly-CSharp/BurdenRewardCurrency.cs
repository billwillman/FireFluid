using UnityEngine;

public class BurdenRewardCurrency : BurdenReward
{
	public int amount = 3;

	public override string GetDescription()
	{
		return ((amount >= 0) ? "+ " : "- ") + Mathf.Abs(amount) + "<sprite name=currency>";
	}

	public override void Grant()
	{
		PersistentDataComponent.GetDataInstance().CollectCurrency(amount);
	}
}
