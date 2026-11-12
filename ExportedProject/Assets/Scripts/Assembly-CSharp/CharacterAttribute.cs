using System;
using UnityEngine;

[Serializable]
public class CharacterAttribute
{
	private bool hidden;

	private float lowerBoundary;

	private float upperBoundary;

	public ValueStackFloat value;

	public CharacterAttribute(float defaultValue, bool hidden = false, float lowerBoundary = -9999999f, float upperBoundary = 9999999f)
	{
		value = new ValueStackFloat(defaultValue);
		this.lowerBoundary = lowerBoundary;
		this.upperBoundary = upperBoundary;
		this.hidden = hidden;
	}

	public float GetCurrentValue()
	{
		return Mathf.Clamp(value.GetCurrentValue(), lowerBoundary, upperBoundary);
	}

	public float GetCurrentBaseValue()
	{
		return value.defaultValue;
	}

	public bool IsHidden()
	{
		if (hidden)
		{
			return value.GetNumberOfValues() == 0;
		}
		return false;
	}
}
