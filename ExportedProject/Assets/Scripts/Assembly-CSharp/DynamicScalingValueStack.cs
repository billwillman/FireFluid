using UnityEngine;

public class DynamicScalingValueStack
{
	public DynamicCharacterAttribute dynamicAttribute;

	public ValueStackFloat valueStack;

	public float minValue;

	public float maxValue;

	public string modificationId;

	public DynamicScalingAttributeUpgradeType scalingType;

	public ValueModifierType modifierType;

	public void UpdateValue()
	{
		float num = 0f;
		float value = Mathf.Lerp(t: (scalingType != DynamicScalingAttributeUpgradeType.PercentFull) ? (1f - Mathf.Clamp01(dynamicAttribute.GetDynamicValue() / dynamicAttribute.GetCurrentValue())) : Mathf.Clamp01(dynamicAttribute.GetDynamicValue() / dynamicAttribute.GetCurrentValue()), a: minValue, b: maxValue);
		valueStack.PopAll(modificationId);
		valueStack.Push(modificationId, value, modifierType);
	}
}
