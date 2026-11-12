using System.Collections.Generic;
using UnityEngine;

public class RandomAttributeUpgrade : AttributeUpgrade
{
	public int numberOfAttributes = 3;

	public float valueIncrementsFactor = 0.1f;

	public int valueMinIncrements = -2;

	public int valueMaxIncrements = 2;

	public override void Awake()
	{
		GenerateAttributeModifications();
		base.Awake();
	}

	public override bool RequirementsMet(Player player)
	{
		return BaseRequirementsMet(player);
	}

	public override string GetDescription()
	{
		return GetDescription(showAbilityName: true);
	}

	private void GenerateAttributeModifications()
	{
		attributeModifications.Clear();
		List<string> attributePaths = new List<string>();
		ValueModification.GetAllValueStacks(Player.Get().gameObject, ref attributePaths);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (num == 0 && num2 < 10)
		{
			for (int i = 0; i < numberOfAttributes; i++)
			{
				AttributeModification attributeModification = new AttributeModification();
				string text = attributePaths[Random.Range(0, attributePaths.Count)];
				attributePaths.Remove(text);
				attributeModification.attributePath = text;
				attributeModification.levelBehaviour = AttributeModificationLevelBehaviour.PowerByLevel;
				attributeModification.type = ValueModifierType.Multiply;
				attributeModification.displayStyle = AttributeModificationDisplayStyle.AddPercentage;
				num3 = ((i % 2 == 0) ? Random.Range(valueMinIncrements, valueMaxIncrements + 1) : (-num3));
				object obj = ValueModification.Get(Player.Get().gameObject, text);
				bool flag = true;
				if (obj is ValueStackFloat)
				{
					ValueStackFloat valueStackFloat = (ValueStackFloat)obj;
					flag = flag && Mathf.Abs(valueStackFloat.GetCurrentValue()) > 0.0001f;
				}
				if (obj is CharacterAttribute)
				{
					CharacterAttribute characterAttribute = (CharacterAttribute)obj;
					flag = flag && Mathf.Abs(characterAttribute.GetCurrentValue()) > 0.0001f;
				}
				if (obj is ValueStackInt)
				{
					flag = false;
				}
				if (num3 == 0)
				{
					flag = false;
				}
				if (flag)
				{
					attributeModification.value = 1f + (float)num3 * valueIncrementsFactor;
					attributeModifications.Add(attributeModification);
					num++;
				}
			}
			num2++;
		}
	}
}
