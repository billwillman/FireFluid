using UnityEngine;

public class SummonTimeBubble : SpawnAtTarget
{
	public ValueStackInt numberOfBubbles = new ValueStackInt(1);

	public ValueStackFloat bubbleCoolDown = new ValueStackFloat(20f);

	public ValueStackBool stunOnEnter = new ValueStackBool(defaultValue: false);

	public ValueStackBool explodeOnExpire = new ValueStackBool(defaultValue: false);

	public ValueStackFloat damageMultiplier = new ValueStackFloat(4f);

	private float inputCooldown;

	public override void Update()
	{
		base.Update();
		cooldownStacks = numberOfBubbles.GetCurrentValue();
		coolDown = bubbleCoolDown.GetCurrentValue();
		inputCooldown -= Time.deltaTime;
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (inputCooldown > 0f)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		inputCooldown = 1f;
		return base.Trigger(parameters);
	}

	public override GameObject Spawn(AbilityCastParameters parameters)
	{
		GameObject obj = base.Spawn(parameters);
		TimeBubble component = obj.GetComponent<TimeBubble>();
		component.stunOnEnter = stunOnEnter.GetCurrentValue();
		component.damage = Player.Get().GetAttributes().damage.GetCurrentValue() * damageMultiplier.GetCurrentValue();
		if (component.damage > 0.0001f)
		{
			component.explodeOnExpire = explodeOnExpire.GetCurrentValue();
		}
		else
		{
			component.explodeOnExpire = false;
		}
		component.radius *= Mathf.Sqrt(Player.Get().GetAttributes().area.GetCurrentValue());
		return obj;
	}
}
