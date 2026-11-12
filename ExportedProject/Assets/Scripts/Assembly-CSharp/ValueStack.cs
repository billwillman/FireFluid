using System.Collections.Generic;
using UnityEngine;

public class ValueStack<T>
{
	public T defaultValue;

	private ValueStackAdd<T> addFunction;

	private ValueStackMultiply<T> multiplyFunction;

	private List<ValueModifier<T>> values = new List<ValueModifier<T>>();

	public ValueStack(ValueStack<T> copyFrom)
	{
		defaultValue = copyFrom.defaultValue;
		addFunction = copyFrom.addFunction;
		multiplyFunction = copyFrom.multiplyFunction;
		foreach (ValueModifier<T> value in copyFrom.values)
		{
			values.Add(new ValueModifier<T>(value));
		}
	}

	public void DebugLog()
	{
		int num = 0;
		foreach (ValueModifier<T> value in values)
		{
			Debug.Log(value.ToString());
			num++;
		}
	}

	public ValueStack(T defaultValue, ValueStackAdd<T> addFunction, ValueStackMultiply<T> multiplyFunction)
	{
		this.defaultValue = defaultValue;
		this.addFunction = addFunction;
		this.multiplyFunction = multiplyFunction;
	}

	public void Push(string callerId, T value, ValueModifierType type = ValueModifierType.Set)
	{
		values.Add(new ValueModifier<T>(callerId, value, type));
	}

	public void PushAllFrom(ValueStack<T> source)
	{
		foreach (ValueModifier<T> value in source.values)
		{
			Push(value.id, value.value, value.type);
		}
	}

	public ValueModifier<T> Get(string callerId)
	{
		for (int num = values.Count - 1; num >= 0; num--)
		{
			if (values[num].id == callerId)
			{
				return values[num];
			}
		}
		return null;
	}

	public bool Has(string callerId)
	{
		return Get(callerId) != null;
	}

	public void Pop(string callerId)
	{
		for (int num = values.Count - 1; num >= 0; num--)
		{
			if (values[num].id == callerId)
			{
				values.RemoveAt(num);
				break;
			}
		}
	}

	public void PopAll(string callerId)
	{
		for (int num = values.Count - 1; num >= 0; num--)
		{
			if (values[num].id == callerId)
			{
				values.RemoveAt(num);
			}
		}
	}

	public void PopAll()
	{
		values.Clear();
	}

	public T GetCurrentValue()
	{
		if (values.Count < 1)
		{
			return defaultValue;
		}
		if (values[values.Count - 1].type == ValueModifierType.Set)
		{
			return values[values.Count - 1].value;
		}
		T val = defaultValue;
		foreach (ValueModifier<T> value in values)
		{
			switch (value.type)
			{
			case ValueModifierType.Set:
				val = value.value;
				break;
			case ValueModifierType.Add:
				val = addFunction(val, value.value);
				break;
			case ValueModifierType.Multiply:
				val = multiplyFunction(val, value.value);
				break;
			}
		}
		return val;
	}

	public int GetNumberOfValues()
	{
		return values.Count;
	}
}
