public interface IItemModifier
{
	string GetIdentifier();

	string GetDescription(int level);

	new ItemModifierType GetType();

	void SetItem(Item item);

	void OnEquip(CharacterAttributes owner);

	void OnUnequip(CharacterAttributes owner);

	void Evaluate(int level);
}
