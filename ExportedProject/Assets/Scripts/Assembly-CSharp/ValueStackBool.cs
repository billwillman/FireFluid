using System;

[Serializable]
public class ValueStackBool : ValueStack<bool>
{
	public ValueStackBool(bool defaultValue)
		: base(defaultValue, (ValueStackAdd<bool>)((bool a, bool b) => a || b), (ValueStackMultiply<bool>)((bool a, bool b) => a && b))
	{
	}
}
