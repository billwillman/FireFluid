using System;

[Serializable]
public class ValueStackFloat : ValueStack<float>
{
	public ValueStackFloat(float defaultValue)
		: base(defaultValue, (ValueStackAdd<float>)((float a, float b) => a + b), (ValueStackMultiply<float>)((float a, float b) => a * b))
	{
	}
}
