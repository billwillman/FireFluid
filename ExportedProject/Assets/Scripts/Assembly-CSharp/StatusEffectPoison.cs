using UnityEngine;

public class StatusEffectPoison : StatusEffect
{
	public float damageMultiplier = 1f;

	private Color damageTickHighlightColor = new Color(0.5f, 3f, 0.5f, 1f);

	public StatusEffectPoison()
	{
		tint = new Color(0.5f, 1f, 0.5f, 0.8f);
	}

	public override string GetTypeIdentifier()
	{
		return "poison";
	}

	public override void OnTick(float deltaTime)
	{
		base.OnTick(deltaTime);
		if (target.ReceiveDamage(owner.damage.GetCurrentValue() * damageMultiplier, owner, GetTypeIdentifier()) > 0.01f)
		{
			rendererEffects.PlayEffect(GetTypeIdentifier(), damageTickHighlightColor, 0.075f, EffectMode.Solid);
		}
	}

	public override void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		base.OnApply(target, owner);
		rendererEffects.AddModifier(GetTypeIdentifier(), tint);
		if (Settings.showNumbers)
		{
			TextParticleManager.Show("<sprite name=\"poisoned\" tint=1>", target.transform, TextAttributes.COLOR_POISON, 96f);
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		if (target != null && !target.health.IsExhausted())
		{
			rendererEffects.RemoveModifier(GetTypeIdentifier());
		}
	}
}
