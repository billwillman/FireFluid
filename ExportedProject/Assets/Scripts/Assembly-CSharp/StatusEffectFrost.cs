using UnityEngine;

public class StatusEffectFrost : StatusEffect
{
	public float movementSpeedMultiplier = 0.5f;

	public float attackSpeedMultiplier = 0.7f;

	public StatusEffectFrost()
	{
		tint = new Color(0.5f, 0.8f, 1f, 0.8f);
	}

	public override string GetTypeIdentifier()
	{
		return "frost";
	}

	public override void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		base.OnApply(target, owner);
		if (target.movementSpeed.value.Get(GetTypeIdentifier()) == null)
		{
			target.movementSpeed.value.Push(GetTypeIdentifier(), movementSpeedMultiplier, ValueModifierType.Multiply);
			target.attackSpeed.value.Push(GetTypeIdentifier(), attackSpeedMultiplier, ValueModifierType.Multiply);
		}
		rendererEffects.AddModifier(GetTypeIdentifier(), tint);
		if (Settings.showNumbers)
		{
			TextParticleManager.Show("<sprite name=\"frozen\" tint=1>", target.transform, TextAttributes.COLOR_FROZEN, 96f);
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		target.movementSpeed.value.PopAll(GetTypeIdentifier());
		target.attackSpeed.value.PopAll(GetTypeIdentifier());
		if (target != null)
		{
			target.health.IsExhausted();
		}
		rendererEffects.RemoveModifier(GetTypeIdentifier());
	}
}
