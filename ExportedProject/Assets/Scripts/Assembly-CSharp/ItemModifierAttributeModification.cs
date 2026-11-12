using System.Collections.Generic;
using UnityEngine;

public class ItemModifierAttributeModification : MonoBehaviour, IItemModifier
{
	public string identifier;

	public ItemModifierType type;

	public List<AttributeModification> modifications = new List<AttributeModification>();

	public Item item;

	public string GetIdentifier()
	{
		return identifier;
	}

	public new ItemModifierType GetType()
	{
		return type;
	}

	public virtual string GetDescription(int level)
	{
		string text = "";
		foreach (AttributeModification modification in modifications)
		{
			text = text + modification.GetDescription(level, evaluated: true, showAbilityName: true, showValueRange: false) + "<br>";
		}
		return text;
	}

	public void SetItem(Item item)
	{
		this.item = item;
	}

	public virtual void OnEquip(CharacterAttributes owner)
	{
		string instanceIdentifier = item.GetInstanceIdentifier();
		foreach (AttributeModification modification in modifications)
		{
			modification.ApplyModificationOnContext(owner.gameObject, instanceIdentifier);
		}
	}

	public void OnUnequip(CharacterAttributes owner)
	{
		string instanceIdentifier = item.GetInstanceIdentifier();
		foreach (AttributeModification modification in modifications)
		{
			modification.RemoveModificationFromContext(owner.gameObject, instanceIdentifier);
		}
	}

	public virtual void Evaluate(int level)
	{
		foreach (AttributeModification modification in modifications)
		{
			modification.EvaluateModificationValue(level);
		}
	}

	public void MultiplyEvaluatedValue(float multiplier)
	{
		foreach (AttributeModification modification in modifications)
		{
			modification.MultiplyEvaluatedValue(multiplier);
		}
	}
}
