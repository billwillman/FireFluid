using UnityEngine;

public class StatusEffectSticky : StatusEffect
{
	public float movementSpeedMultiplier = 0.5f;

	public StatusEffectSticky()
	{
		tint = new Color(0.5f, 0.8f, 0.2f);
		tickInterval = 0.1f;
	}

	public override string GetTypeIdentifier()
	{
		return "sticky";
	}

	public override void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		base.OnApply(target, owner);
		CharacterAttribute movementSpeed = target.movementSpeed;
		if (movementSpeed.value.Get(GetTypeIdentifier()) == null)
		{
			movementSpeed.value.Push(GetTypeIdentifier(), movementSpeedMultiplier, ValueModifierType.Multiply);
		}
		RendererEffects.Tint(target.gameObject, GetTypeIdentifier(), tint);
		if (Settings.showNumbers)
		{
			TextParticleManager.Show("<sprite name=\"sticky\" tint=1>", target.transform, TextAttributes.COLOR_STICKY, 96f);
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		target.movementSpeed.value.PopAll(GetTypeIdentifier());
		if (target != null && !target.health.IsExhausted())
		{
			RendererEffects.Untint(target.gameObject, GetTypeIdentifier());
		}
	}

	public override string GetShortDescription()
	{
		return StringLocalization.Get("statuseffect_sticky");
	}
}
