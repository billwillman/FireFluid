using UnityEngine;

public class ItemModifierNone : MonoBehaviour, IItemModifier
{
	public ItemModifierType type;

	public string GetIdentifier()
	{
		return "";
	}

	public new ItemModifierType GetType()
	{
		return type;
	}

	public void SetItem(Item item)
	{
	}

	public string GetDescription(int level)
	{
		return "";
	}

	public void OnEquip(CharacterAttributes owner)
	{
	}

	public void OnUnequip(CharacterAttributes owner)
	{
	}

	public void Evaluate(int level)
	{
	}
}
