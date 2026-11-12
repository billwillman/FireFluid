using System.Collections.Generic;
using UnityEngine;

public class StatusEffectAttributeModification : StatusEffect
{
	public string identifier = "attributemodification";

	public List<AttributeModification> modifications = new List<AttributeModification>();

	public StatusEffectAttributeModification()
	{
		tint = new Color(1f, 1f, 0.5f);
	}

	public override string GetTypeIdentifier()
	{
		return identifier;
	}

	public override void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		base.OnApply(target, owner);
		RendererEffects.Highlight(owner.gameObject, GetTypeIdentifier(), tint, duration, EffectMode.Sine);
		foreach (AttributeModification modification in modifications)
		{
			modification.ApplyModificationOnContext(target.gameObject, GetTypeIdentifier());
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		if (!(target != null))
		{
			return;
		}
		foreach (AttributeModification modification in modifications)
		{
			modification.RemoveModificationFromContext(target.gameObject, GetTypeIdentifier());
		}
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override string GetShortDescription()
	{
		string text = "";
		foreach (AttributeModification modification in modifications)
		{
			text = text + modification.GetDescription(0, evaluated: true, showAbilityName: false, showValueRange: false) + "<br>";
		}
		return text + "<color=#888888>" + Mathf.FloorToInt(duration) + "<size=50%>s</size></color>";
	}
}
