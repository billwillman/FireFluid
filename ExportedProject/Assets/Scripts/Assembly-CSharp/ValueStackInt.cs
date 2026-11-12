using System;

[Serializable]
public class ValueStackInt : ValueStack<int>
{
	public ValueStackInt(int defaultValue)
		: base(defaultValue, (ValueStackAdd<int>)((int a, int b) => a + b), (ValueStackMultiply<int>)((int a, int b) => a * b))
	{
	}
}
