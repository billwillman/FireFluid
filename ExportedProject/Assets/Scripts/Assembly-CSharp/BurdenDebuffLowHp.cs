using UnityEngine;

public class BurdenDebuffLowHp : BurdenDebuff
{
	public float health = 1f;

	public override void Activate()
	{
		Player.Get().GetAttributes().ReceiveDamage(Player.Get().GetAttributes().health.GetDynamicValue() - health, Player.Get().GetAttributes(), "BurdenDebuffLowHp");
	}

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_debuff_lowhp").Replace("{hp}", Mathf.RoundToInt(health).ToString() ?? "");
	}
}
