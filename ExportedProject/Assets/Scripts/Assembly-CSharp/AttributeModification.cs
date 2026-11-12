using System;
using UnityEngine;

[Serializable]
public class AttributeModification
{
	public string attributePath = "CharacterAttributes.damage";

	public float value = 0.1f;

	public ValueModifierType type = ValueModifierType.Add;

	public AttributeModificationLevelBehaviour levelBehaviour;

	public AttributeModificationDisplayStyle displayStyle;

	private float evaluatedValue = float.NaN;

	public AttributeModification()
	{
	}

	public AttributeModification(AttributeModification copyFrom)
	{
		attributePath = copyFrom.attributePath;
		value = copyFrom.value;
		type = copyFrom.type;
		levelBehaviour = copyFrom.levelBehaviour;
		displayStyle = copyFrom.displayStyle;
		evaluatedValue = copyFrom.evaluatedValue;
	}

	public string GetDescription()
	{
		return GetDescription(0, evaluated: true);
	}

	public string GetDescription(int level)
	{
		return GetDescription(level, evaluated: false);
	}

	public string GetDescription(int level, bool evaluated, bool showAbilityName = false, bool showValueRange = true)
	{
		float num = GetModificationValueMin(level);
		float num2 = GetModificationValueMax(level);
		if (num > num2)
		{
			float num3 = num;
			num = num2;
			num2 = num3;
		}
		string text;
		if (displayStyle == AttributeModificationDisplayStyle.AddPercentage)
		{
			if (type == ValueModifierType.Multiply)
			{
				text = Mathf.RoundToInt(num * 100f - 100f) + "%";
				if (Mathf.Abs(num2 - num) > 0.001f)
				{
					text = text + "-" + Mathf.RoundToInt(num2 * 100f - 100f) + "%";
				}
			}
			else
			{
				text = Mathf.RoundToInt(num * 100f) + "%";
				if (Mathf.Abs(num2 - num) > 0.001f)
				{
					text = text + "-" + Mathf.RoundToInt(num2 * 100f) + "%";
				}
			}
		}
		else
		{
			text = num.ToString("0.##");
			if (Mathf.Abs(num2 - num) > 0.001f)
			{
				text = text + "-" + num2.ToString("0.##");
			}
		}
		string text2;
		if (!evaluated)
		{
			text2 = "<b>" + text + "</b>";
		}
		else
		{
			text2 = displayStyle switch
			{
				AttributeModificationDisplayStyle.Int => "<b>" + Mathf.RoundToInt(GetModificationValue()) + "</b>", 
				AttributeModificationDisplayStyle.AddPercentage => (type != ValueModifierType.Multiply) ? ("<b>" + Mathf.RoundToInt(GetModificationValue() * 100f) + "%") : ("<b>" + ((GetModificationValue() >= 1f) ? "+" : "") + Mathf.RoundToInt(GetModificationValue() * 100f - 100f) + "%"), 
				_ => "<b>" + GetModificationValue().ToString("0.##") + "</b>", 
			};
			if (showValueRange && !Mathf.Approximately(num, num2))
			{
				text2 = text2 + "<color=#aaaaaa>[" + text + "]</color>";
			}
		}
		string text3 = "";
		if (showAbilityName)
		{
			text3 = ValueModification.GetAbilityName(attributePath, Player.Get().gameObject);
			if (text3 != "")
			{
				text3 = " <color=#ffbb88><size=75%>(" + text3 + ")</size></color>";
			}
		}
		switch (type)
		{
		case ValueModifierType.Add:
			text2 = ((value >= 0f) ? "+" : "") + text2 + " <color=#ffeedd>" + ValueModification.AttributePathToName(attributePath) + "</color>" + text3;
			break;
		case ValueModifierType.Multiply:
			text2 = text2 + " <color=#ffeedd>" + ValueModification.AttributePathToName(attributePath) + "</color>" + text3;
			break;
		case ValueModifierType.Set:
			text2 = "<color=#ffeedd>" + ValueModification.AttributePathToName(attributePath) + text3 + "</color> -> " + text2;
			break;
		}
		return text2;
	}

	public float GetModificationValueMin(int level)
	{
		if (level == 0)
		{
			switch (type)
			{
			case ValueModifierType.Multiply:
				return 1f;
			case ValueModifierType.Add:
				return 0f;
			}
		}
		return levelBehaviour switch
		{
			AttributeModificationLevelBehaviour.BasePowerByLevel => Mathf.Max(value, Mathf.Pow(value, level)), 
			AttributeModificationLevelBehaviour.BaseMultiplyByLevel => Mathf.Max(value, value * (float)level), 
			_ => value, 
		};
	}

	public float GetModificationValueMax(int level)
	{
		if (level == 0)
		{
			switch (type)
			{
			case ValueModifierType.Multiply:
				return 1f;
			case ValueModifierType.Add:
				return 0f;
			}
		}
		switch (levelBehaviour)
		{
		case AttributeModificationLevelBehaviour.MultiplyByLevel:
		case AttributeModificationLevelBehaviour.BaseMultiplyByLevel:
			return Mathf.Max(value, value * (float)level);
		case AttributeModificationLevelBehaviour.PowerByLevel:
		case AttributeModificationLevelBehaviour.BasePowerByLevel:
			return Mathf.Pow(value, level);
		case AttributeModificationLevelBehaviour.LeaveUnchanged:
			return value;
		default:
			return value;
		}
	}

	public void EvaluateModificationValue()
	{
		EvaluateModificationValue(1);
	}

	public void EvaluateModificationValue(int level)
	{
		float num = GetModificationValueMin(level);
		float num2 = GetModificationValueMax(level);
		if (num > num2)
		{
			float num3 = num;
			num = num2;
			num2 = num3;
		}
		evaluatedValue = UnityEngine.Random.Range(num, num2);
	}

	public float GetModificationValue()
	{
		return evaluatedValue;
	}

	public void MultiplyEvaluatedValue(float multiplier)
	{
		switch (type)
		{
		case ValueModifierType.Add:
			evaluatedValue *= 0.5f;
			break;
		case ValueModifierType.Multiply:
			evaluatedValue = (evaluatedValue - 1f) * 0.5f + 1f;
			break;
		}
	}

	public bool HasAttributePath(object context)
	{
		return ValueModification.Has(context, attributePath);
	}

	public void ApplyModificationOnContext(object context, string modifyerId, bool fillDynamic = true)
	{
		if (!HasAttributePath(context))
		{
			return;
		}
		if (float.IsNaN(evaluatedValue))
		{
			Debug.LogWarning("Applying non-evaluated value: " + attributePath + "(" + modifyerId + ")");
			EvaluateModificationValue();
		}
		object obj = ValueModification.Get(context, attributePath);
		float modificationValue = GetModificationValue();
		if (obj is ValueStackFloat)
		{
			((ValueStackFloat)obj).Push(modifyerId, modificationValue, type);
		}
		else if (obj is ValueStackBool)
		{
			((ValueStackBool)obj).Push(modifyerId, value > 0.5f, type);
		}
		else if (obj is ValueStackInt)
		{
			ValueStackInt obj2 = (ValueStackInt)obj;
			modificationValue = Mathf.RoundToInt(modificationValue);
			obj2.Push(modifyerId, Mathf.RoundToInt(modificationValue), type);
		}
		else if (obj is DynamicCharacterAttribute)
		{
			DynamicCharacterAttribute dynamicCharacterAttribute = (DynamicCharacterAttribute)obj;
			float currentValue = dynamicCharacterAttribute.GetCurrentValue();
			dynamicCharacterAttribute.value.Push(modifyerId, modificationValue, type);
			if (fillDynamic && dynamicCharacterAttribute.GetCurrentValue() > currentValue)
			{
				dynamicCharacterAttribute.Fill(dynamicCharacterAttribute.GetCurrentValue() - currentValue);
			}
		}
		else if (obj is CharacterAttribute)
		{
			((CharacterAttribute)obj).value.Push(modifyerId, modificationValue, type);
		}
		else
		{
			Debug.Log("Field [" + obj?.ToString() + "] was found with non compatible type [" + obj.GetType()?.ToString() + "]. (" + modifyerId + ")");
		}
	}

	public void RemoveModificationFromContext(object context, string modifyerId)
	{
		if (!HasAttributePath(context))
		{
			return;
		}
		if (float.IsNaN(evaluatedValue))
		{
			Debug.LogWarning("Applying non-evaluated value: " + attributePath + "(" + modifyerId + ")");
			EvaluateModificationValue();
		}
		object obj = ValueModification.Get(context, attributePath);
		if (obj is ValueStackFloat)
		{
			((ValueStackFloat)obj).PopAll(modifyerId);
		}
		else if (obj is ValueStackBool)
		{
			((ValueStackBool)obj).PopAll(modifyerId);
		}
		else if (obj is ValueStackInt)
		{
			((ValueStackInt)obj).PopAll(modifyerId);
		}
		else if (obj is DynamicCharacterAttribute)
		{
			DynamicCharacterAttribute dynamicCharacterAttribute = (DynamicCharacterAttribute)obj;
			float currentValue = dynamicCharacterAttribute.GetCurrentValue();
			dynamicCharacterAttribute.value.PopAll(modifyerId);
			if (dynamicCharacterAttribute.GetCurrentValue() > currentValue)
			{
				dynamicCharacterAttribute.Fill(dynamicCharacterAttribute.GetCurrentValue() - currentValue);
			}
		}
		else if (obj is CharacterAttribute)
		{
			((CharacterAttribute)obj).value.PopAll(modifyerId);
		}
		else
		{
			Debug.Log("Field [" + obj?.ToString() + "] was found with non compatible type [" + obj.GetType()?.ToString() + "]. (" + modifyerId + ")");
		}
	}
}
