using System;
using UnityEngine;

[Serializable]
public class DynamicCharacterAttribute : CharacterAttribute
{
	public float exhaustedRecoveryThreshold = 10f;

	private bool exhausted;

	[SerializeField]
	private float dynamicValue;

	private float dynamicValueSmoothed;

	public ValueStackBool enableRegeneration = new ValueStackBool(defaultValue: true);

	public ValueStackFloat regeneration = new ValueStackFloat(0f);

	private bool failedSpendAttempt;

	private bool failedCooldownAttempt;

	public DynamicCharacterAttributeValueChangedEvent onDynamicValueChanged = new DynamicCharacterAttributeValueChangedEvent();

	public bool IsFull()
	{
		return Mathf.Abs(dynamicValue - value.GetCurrentValue()) < 0.001f;
	}

	public void IndicateFailedSpendAttempt()
	{
		failedSpendAttempt = true;
	}

	public bool HasFailedSpendAttempt()
	{
		return failedSpendAttempt;
	}

	public DynamicCharacterAttribute(float defaultValue, bool hidden = false)
		: base(defaultValue, hidden)
	{
		dynamicValue = GetCurrentValue();
	}

	public bool CanSpend(float amount, bool indicateFailedSpendAttempt = false)
	{
		if (exhausted)
		{
			if (indicateFailedSpendAttempt)
			{
				IndicateFailedSpendAttempt();
			}
			return false;
		}
		if (dynamicValue >= amount)
		{
			return true;
		}
		if (indicateFailedSpendAttempt)
		{
			IndicateFailedSpendAttempt();
		}
		return false;
	}

	public void Spend(float amount, DynamicCharacterAttributeValueChangedType changeType = DynamicCharacterAttributeValueChangedType.instant)
	{
		float num = dynamicValue;
		dynamicValue -= amount;
		if (dynamicValue <= 0f)
		{
			exhausted = true;
		}
		onDynamicValueChanged?.Invoke(dynamicValue - num, changeType);
	}

	public void Fill()
	{
		Fill(99999f);
	}

	public void Fill(float amount, DynamicCharacterAttributeValueChangedType changeType = DynamicCharacterAttributeValueChangedType.instant)
	{
		float num = dynamicValue;
		dynamicValue += amount;
		dynamicValue = Mathf.Clamp(dynamicValue, 0f, GetCurrentValue());
		if (dynamicValue > exhaustedRecoveryThreshold)
		{
			exhausted = false;
		}
		onDynamicValueChanged?.Invoke(dynamicValue - num, changeType);
	}

	public float GetDynamicValue()
	{
		return dynamicValue;
	}

	public void SetDynamicValue(float value)
	{
		dynamicValue = value;
		exhausted = false;
	}

	public float GetSmoothedDynamicValue()
	{
		return dynamicValueSmoothed;
	}

	public bool IsExhausted()
	{
		return exhausted;
	}

	public void Regenerate()
	{
		Fill(regeneration.GetCurrentValue() * Time.deltaTime, DynamicCharacterAttributeValueChangedType.gradual);
	}

	public void Update()
	{
		if (enableRegeneration.GetCurrentValue())
		{
			Regenerate();
		}
		dynamicValueSmoothed = Mathf.Max(dynamicValue, Mathf.Lerp(dynamicValueSmoothed, dynamicValue, Time.deltaTime));
		failedSpendAttempt = false;
	}
}
