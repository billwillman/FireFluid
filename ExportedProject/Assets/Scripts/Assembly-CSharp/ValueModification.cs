using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ValueModification
{
	public static object Get(object context, string attributePath)
	{
		string text = attributePath;
		object obj = context;
		int num;
		do
		{
			num = text.IndexOf(".");
			string text2;
			if (num > -1)
			{
				text2 = text.Substring(0, num);
				text = text.Substring(num + 1);
			}
			else
			{
				text2 = text;
			}
			if (obj is GameObject)
			{
				obj = GetChildWithClass((GameObject)obj, text2);
				continue;
			}
			FieldInfo field = obj.GetType().GetField(text2);
			if (field == null)
			{
				if (obj is MonoBehaviour)
				{
					obj = GetChildWithClass((MonoBehaviour)obj, text2);
				}
			}
			else
			{
				obj = field.GetValue(obj);
			}
		}
		while (obj != null && num > -1);
		return obj;
	}

	public static string AttributePathToName(string attributePath, int maxStepsBack = 2)
	{
		if (string.IsNullOrEmpty(attributePath))
		{
			return "*";
		}
		string[] array = attributePath.Split('.');
		int num = array.Length;
		maxStepsBack = Math.Min(maxStepsBack, num);
		for (int num2 = maxStepsBack; num2 >= 1; num2--)
		{
			int startIndex = num - num2;
			string text = string.Join(".", array, startIndex, num2);
			string stringID = "attribute_" + text;
			if (StringLocalization.Has(stringID))
			{
				return StringLocalization.Get(stringID);
			}
		}
		return "*" + attributePath;
	}

	public static string GetAbilityName(string attributePath, object context)
	{
		for (int i = 0; i < 3; i++)
		{
			int num = attributePath.LastIndexOf('.');
			if (num < 0)
			{
				return "";
			}
			attributePath = attributePath.Substring(0, num);
			object obj = Get(context, attributePath);
			if (obj is Ability)
			{
				return StringLocalization.Get("ability_" + ((Ability)obj).GetIdentifier());
			}
		}
		return "";
	}

	public static bool Has(object context, string attribute)
	{
		return Get(context, attribute) != null;
	}

	public static float GetAsFloat(object context, string attribute)
	{
		object obj = Get(context, attribute);
		if (obj == null)
		{
			return 0f;
		}
		if (obj is ValueStackFloat)
		{
			return ((ValueStackFloat)obj).GetCurrentValue();
		}
		if (obj is ValueStackBool)
		{
			if (!((ValueStackBool)obj).GetCurrentValue())
			{
				return 0f;
			}
			return 1f;
		}
		if (obj is ValueStackInt)
		{
			return ((ValueStackInt)obj).GetCurrentValue();
		}
		if (obj is DynamicCharacterAttribute)
		{
			return ((DynamicCharacterAttribute)obj).GetCurrentValue();
		}
		if (obj is CharacterAttribute)
		{
			return ((CharacterAttribute)obj).GetCurrentValue();
		}
		Debug.Log("Field [" + attribute + "] was found with non compatible type. (" + context?.ToString() + ")");
		return 0f;
	}

	public static object GetChildWithClass(GameObject gameObject, string className)
	{
		Type type = Type.GetType(className);
		if (type == null)
		{
			Debug.Log("Type is null: " + className);
			return null;
		}
		return gameObject.GetComponentInChildren(type);
	}

	public static object GetChildWithClass(MonoBehaviour monoBehaviour, string className)
	{
		Type type = Type.GetType(className);
		if (type == null)
		{
			Debug.Log("Type is null: " + className);
			return null;
		}
		return monoBehaviour.GetComponentInChildren(type);
	}

	public static bool HasChildWithClass(GameObject gameObject, string className)
	{
		Type type = Type.GetType(className);
		return gameObject.GetComponentInChildren(type) != null;
	}

	public static void GetAllValueStacks(GameObject root, ref List<string> attributePaths, bool traverseChildren = true, bool includeHidden = false)
	{
		MonoBehaviour[] array = ((!traverseChildren) ? root.GetComponents<MonoBehaviour>() : root.GetComponentsInChildren<MonoBehaviour>());
		MonoBehaviour[] array2 = array;
		foreach (MonoBehaviour monoBehaviour in array2)
		{
			string name = monoBehaviour.GetType().Name;
			FieldInfo[] fields = monoBehaviour.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(monoBehaviour);
				if (!includeHidden && value is CharacterAttribute && ((CharacterAttribute)value).IsHidden())
				{
					continue;
				}
				string text = name + "." + fieldInfo.Name;
				if (!(monoBehaviour is Ability))
				{
					Ability componentInParent = monoBehaviour.GetComponentInParent<Ability>();
					if ((bool)componentInParent)
					{
						text = componentInParent.GetType().Name + "." + text;
					}
				}
				if (value is ValueStackInt || value is ValueStackFloat || value is CharacterAttribute)
				{
					attributePaths.Add(text);
				}
				else if (value is DynamicCharacterAttribute)
				{
					attributePaths.Add(text);
					if (((DynamicCharacterAttribute)value).enableRegeneration.GetCurrentValue())
					{
						attributePaths.Add(text + ".regeneration");
					}
				}
			}
		}
	}

	public static void ShallowCopyAllValueStacks(object source, object target)
	{
		FieldInfo[] fields = source.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(source);
			FieldInfo field = target.GetType().GetField(fieldInfo.Name);
			if (field != null)
			{
				object value2 = field.GetValue(target);
				if (value is ValueStackFloat && value2 is ValueStackFloat)
				{
					ValueStackFloat obj = (ValueStackFloat)value2;
					obj.PopAll();
					obj.PushAllFrom((ValueStackFloat)value);
				}
				else if (value is ValueStackInt && value2 is ValueStackInt)
				{
					ValueStackInt obj2 = (ValueStackInt)value2;
					obj2.PopAll();
					obj2.PushAllFrom((ValueStackInt)value);
				}
				else if (value is ValueStackBool && value2 is ValueStackBool)
				{
					ValueStackBool obj3 = (ValueStackBool)value2;
					obj3.PopAll();
					obj3.PushAllFrom((ValueStackBool)value);
				}
				else if (value is CharacterAttribute && value2 is CharacterAttribute)
				{
					CharacterAttribute obj4 = (CharacterAttribute)value2;
					obj4.value.PopAll();
					obj4.value.PushAllFrom(((CharacterAttribute)value).value);
				}
			}
		}
	}

	public static StructuredAttributePaths GetAttributePathsStructure(List<string> attributePaths)
	{
		StructuredAttributePaths structuredAttributePaths = new StructuredAttributePaths("root");
		foreach (string attributePath in attributePaths)
		{
			string[] array = attributePath.Split(".");
			StructuredAttributePaths structuredAttributePaths2 = structuredAttributePaths;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + ((i > 0) ? "." : "") + array[i];
				bool flag = false;
				foreach (StructuredAttributePaths subAttribute in structuredAttributePaths2.subAttributes)
				{
					if (subAttribute.attribute == array[i])
					{
						structuredAttributePaths2 = subAttribute;
						flag = true;
					}
				}
				if (!flag)
				{
					StructuredAttributePaths structuredAttributePaths3 = new StructuredAttributePaths(array[i]);
					structuredAttributePaths3.fullAttributePath = text;
					structuredAttributePaths2.subAttributes.Add(structuredAttributePaths3);
					structuredAttributePaths2 = structuredAttributePaths3;
				}
			}
		}
		return structuredAttributePaths;
	}

	public static string GetValueText(string fullAttributePath)
	{
		string text = "";
		object obj = Get(Player.Get().gameObject, fullAttributePath);
		if (obj == null)
		{
			Debug.Log("NULL: " + fullAttributePath);
		}
		if (obj is ValueStackFloat)
		{
			text = (fullAttributePath.ToLowerInvariant().Contains("chance") ? (text + Mathf.RoundToInt(((ValueStackFloat)obj).GetCurrentValue() * 100f) + "%") : ((!fullAttributePath.ToLowerInvariant().Contains("regeneration")) ? (text + ((ValueStackFloat)obj).GetCurrentValue().ToString("0.##")) : (text + ((ValueStackFloat)obj).GetCurrentValue().ToString("0.##") + "<size=75%>/s</size>")));
		}
		else if (obj is ValueStackInt)
		{
			text += ((ValueStackInt)obj).GetCurrentValue();
		}
		else if (obj is ValueStackBool)
		{
			text += (((ValueStackBool)obj).GetCurrentValue() ? "Yes" : "No");
		}
		else if (obj is DynamicCharacterAttribute)
		{
			text += ((DynamicCharacterAttribute)obj).GetCurrentValue().ToString("0.##");
		}
		else if (obj is CharacterAttribute)
		{
			text = ((!fullAttributePath.ToLowerInvariant().Contains("chance")) ? (text + ((CharacterAttribute)obj).GetCurrentValue().ToString("0.##")) : (text + Mathf.RoundToInt(((CharacterAttribute)obj).GetCurrentValue() * 100f) + "%"));
		}
		return text;
	}
}
