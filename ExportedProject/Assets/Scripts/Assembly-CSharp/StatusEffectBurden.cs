using UnityEngine;

public class StatusEffectBurden : StatusEffect
{
	public Burden burden;

	public StatusEffectBurden()
	{
		tint = new Color(0.5f, 0.5f, 0.5f);
		tickInterval = 100000f;
		duration = 1f;
	}

	public override string GetTypeIdentifier()
	{
		return "burden";
	}

	public override string GetShortDescription()
	{
		if (burden == null)
		{
			return "BURDEN MISSING";
		}
		return burden.GetShortDescription();
	}
}
