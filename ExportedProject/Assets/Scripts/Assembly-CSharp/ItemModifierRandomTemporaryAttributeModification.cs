using System.Collections.Generic;
using UnityEngine;

public class ItemModifierRandomTemporaryAttributeModification : ItemModifierTemporaryAttributeModification
{
	private void GenerateAttributeModifications()
	{
		int num = 3;
		modifications.Clear();
		List<string> attributePaths = new List<string>();
		ValueModification.GetAllValueStacks(Player.Get().gameObject, ref attributePaths);
		int num2 = 0;
		int num3 = 0;
		while (num2 == 0 && num3 < 10)
		{
			for (int i = 0; i < num; i++)
			{
				float num4 = Random.Range(-0.1f, 0.5f);
				AttributeModification attributeModification = new AttributeModification();
				string attributePath = attributePaths[Random.Range(0, attributePaths.Count)];
				attributePaths.Remove(attributePath);
				attributeModification.attributePath = attributePath;
				attributeModification.type = ValueModifierType.Multiply;
				attributeModification.displayStyle = AttributeModificationDisplayStyle.AddPercentage;
				object obj = ValueModification.Get(Player.Get().gameObject, attributePath);
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
					attributeModification.value = 1f + num4;
					attributeModification.EvaluateModificationValue(1);
					modifications.Add(attributeModification);
					num2++;
				}
			}
			num3++;
		}
	}

	public override void Evaluate(int level)
	{
		GenerateAttributeModifications();
		base.Evaluate(level);
	}
}
