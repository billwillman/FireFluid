using UnityEngine;

public class ItemModifierTemporaryAttributeModification : ItemModifierAttributeModification
{
	public Color tint = Color.yellow;

	public float duration;

	public override string GetDescription(int level)
	{
		string text = "<b>" + StringLocalization.Get("item_temporaryattribute").Replace("{seconds}", Mathf.RoundToInt(duration).ToString()) + "</b><br>";
		foreach (AttributeModification modification in modifications)
		{
			text = text + modification.GetDescription(level, evaluated: true, showAbilityName: true, showValueRange: false) + "<br>";
		}
		return text;
	}

	public override void OnEquip(CharacterAttributes owner)
	{
		StatusEffectManager.Apply(new StatusEffectAttributeModification
		{
			identifier = item.GetInstanceIdentifier(),
			modifications = modifications,
			duration = duration,
			isStackable = false,
			tint = tint
		}, owner, owner);
	}
}
