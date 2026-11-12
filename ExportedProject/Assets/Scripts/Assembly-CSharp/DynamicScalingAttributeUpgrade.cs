using System.Collections.Generic;
using UnityEngine;

public class DynamicScalingAttributeUpgrade : AttributeUpgrade
{
	public string dynamicAttributePath = "CharacterAttributes.health";

	private DynamicCharacterAttribute dynamicAttribute;

	public DynamicScalingAttributeUpgradeType type;

	private List<DynamicScalingValueStack> scaledValueStacks = new List<DynamicScalingValueStack>();

	public override void Apply(Player player)
	{
		base.Apply(player);
		object obj = ValueModification.Get(player.gameObject, dynamicAttributePath);
		if (obj is DynamicCharacterAttribute)
		{
			dynamicAttribute = (DynamicCharacterAttribute)obj;
			scaledValueStacks.Clear();
			foreach (AttributeModification attributeModification in attributeModifications)
			{
				object obj2 = ValueModification.Get(player.gameObject, attributeModification.attributePath);
				if (obj2 == null)
				{
					continue;
				}
				if (obj2 is ValueStackFloat || obj2 is CharacterAttribute)
				{
					DynamicScalingValueStack dynamicScalingValueStack = new DynamicScalingValueStack();
					if (obj2 is ValueStackFloat)
					{
						dynamicScalingValueStack.valueStack = (ValueStackFloat)obj2;
					}
					else if (obj2 is CharacterAttribute)
					{
						dynamicScalingValueStack.valueStack = ((CharacterAttribute)obj2).value;
					}
					if (attributeModification.type == ValueModifierType.Multiply)
					{
						dynamicScalingValueStack.minValue = 1f;
					}
					else
					{
						dynamicScalingValueStack.minValue = 0f;
					}
					dynamicScalingValueStack.maxValue = attributeModification.GetModificationValue();
					dynamicScalingValueStack.modificationId = GetIdentifier();
					dynamicScalingValueStack.scalingType = type;
					dynamicScalingValueStack.modifierType = attributeModification.type;
					dynamicScalingValueStack.dynamicAttribute = dynamicAttribute;
					scaledValueStacks.Add(dynamicScalingValueStack);
				}
				else
				{
					Debug.Log("WARNING (DynamicScalingAttributeUpgrade): " + attributeModification.attributePath + " is not of type ValueStackFloat.");
				}
			}
			DynamicScalingAttributeUpdater dynamicScalingAttributeUpdater = player.GetComponent<DynamicScalingAttributeUpdater>();
			if (dynamicScalingAttributeUpdater == null)
			{
				dynamicScalingAttributeUpdater = player.gameObject.AddComponent<DynamicScalingAttributeUpdater>();
			}
			{
				foreach (DynamicScalingValueStack scaledValueStack in scaledValueStacks)
				{
					dynamicScalingAttributeUpdater.dynamicScalingValueStacks.Add(scaledValueStack);
				}
				return;
			}
		}
		Debug.Log("WARNING (DynamicScalingAttributeUpgrade): " + dynamicAttributePath + " is not of type DynamicCharacterAttribute.");
	}
}
