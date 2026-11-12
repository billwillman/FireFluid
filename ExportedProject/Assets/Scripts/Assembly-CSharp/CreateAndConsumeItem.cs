using UnityEngine;

public class CreateAndConsumeItem : Ability
{
	public ValueStackFloat potency = new ValueStackFloat(0.5f);

	public LootTable lootTable;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		ItemController component = owner.GetComponent<ItemController>();
		if (component == null)
		{
			return base.Trigger(parameters);
		}
		DropChanceEntry drop = lootTable.GetDrop(guaranteeDrop: true);
		Item component2 = lootTable.InstantiateDrop(drop, base.transform.position, addDropAnimation: false).GetComponent<Item>();
		if (component2 == null)
		{
			return base.Trigger(parameters);
		}
		component2.Evaluate();
		ItemModifierAttributeModification[] componentsInChildren = component2.GetComponentsInChildren<ItemModifierAttributeModification>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MultiplyEvaluatedValue(potency.GetCurrentValue());
		}
		TextParticleManager.Show(component2.GetDisplayName(), base.transform.position + Vector3.up * 4f);
		component.Equip(component2);
		return base.Trigger(parameters);
	}
}
