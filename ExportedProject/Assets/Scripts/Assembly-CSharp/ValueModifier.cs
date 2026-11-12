public class ValueModifier<T>
{
	public string id;

	public T value;

	public ValueModifierType type;

	public ValueModifier(ValueModifier<T> copyFrom)
		: this(copyFrom.id, copyFrom.value, copyFrom.type)
	{
	}

	public ValueModifier(string id, T value, ValueModifierType type = ValueModifierType.Set)
	{
		this.id = id;
		this.value = value;
		this.type = type;
	}

	public override string ToString()
	{
		return "[" + id + ":" + type.ToString() + "]" + value.ToString();
	}
}
