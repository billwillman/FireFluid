using System.Collections.Generic;
using UnityEngine;

public class BurdenRewardRandomAttribute : BurdenReward
{
	public int numberOfAttributes = 1;

	public float value = 0.2f;

	public List<AttributeModification> attributeModifications = new List<AttributeModification>();

	public virtual void Awake()
	{
		GenerateAttributeModifications();
	}

	public virtual List<string> FilterAttributes(List<string> attributePaths)
	{
		return attributePaths;
	}

	public void GenerateAttributeModifications(bool includeHidden = false, ValueModifierType type = ValueModifierType.Multiply, AttributeModificationDisplayStyle displayStyle = AttributeModificationDisplayStyle.AddPercentage)
	{
		attributeModifications.Clear();
		List<string> attributePaths = new List<string>();
		ValueModification.GetAllValueStacks(Player.Get().gameObject, ref attributePaths, traverseChildren: true, includeHidden);
		List<string> list = FilterAttributes(attributePaths);
		if (list.Count > 0)
		{
			attributePaths = list;
		}
		int num = 0;
		int num2 = 0;
		while (num == 0 && num2 < 10)
		{
			for (int i = 0; i < numberOfAttributes; i++)
			{
				AttributeModification attributeModification = new AttributeModification();
				string text = attributePaths[Random.Range(0, attributePaths.Count)];
				attributePaths.Remove(text);
				attributeModification.attributePath = text;
				attributeModification.type = type;
				attributeModification.displayStyle = displayStyle;
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
				if (flag)
				{
					attributeModification.value = value;
					if (type == ValueModifierType.Multiply)
					{
						attributeModification.value += 1f;
					}
					attributeModification.EvaluateModificationValue(1);
					attributeModifications.Add(attributeModification);
					num++;
				}
			}
			num2++;
		}
	}

	public override string GetDescription()
	{
		string text = "";
		int num = 0;
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			text += attributeModification.GetDescription(0, evaluated: true, showAbilityName: true);
			if (num < attributeModifications.Count - 1)
			{
				text += "<br>";
			}
			num++;
		}
		return text;
	}

	public override void Grant()
	{
		Player.Get().GetAttributes();
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			attributeModification.ApplyModificationOnContext(Player.Get().gameObject, "BurdenOfRewardRandomAttribute");
		}
	}
}
