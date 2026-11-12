using UnityEngine;

public class StatusEffectStunned : StatusEffect
{
	public StatusEffectStunned()
	{
		duration = 2f;
		tint = new Color(0.5f, 0.2f, 0.8f, 0.8f);
	}

	public override string GetTypeIdentifier()
	{
		return "stunned";
	}

	public override void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		base.OnApply(target, owner);
		CharacterAttribute movementSpeed = target.movementSpeed;
		if (movementSpeed.value.Get(GetTypeIdentifier()) == null)
		{
			movementSpeed.value.Push(GetTypeIdentifier(), 0f);
		}
		movementSpeed = target.attackSpeed;
		if (movementSpeed.value.Get(GetTypeIdentifier()) == null)
		{
			movementSpeed.value.Push(GetTypeIdentifier(), 0f);
		}
		rendererEffects.AddModifier(GetTypeIdentifier(), tint);
	}

	public override void OnRemove()
	{
		base.OnRemove();
		if (target != null && !target.health.IsExhausted())
		{
			rendererEffects.RemoveModifier(GetTypeIdentifier());
			target.movementSpeed.value.PopAll(GetTypeIdentifier());
			target.attackSpeed.value.PopAll(GetTypeIdentifier());
		}
	}
}
