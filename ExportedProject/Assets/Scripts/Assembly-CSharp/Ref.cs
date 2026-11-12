using System;

internal class Ref<T>
{
	private Func<T> getter;

	private Action<T> setter;

	public T Value
	{
		get
		{
			return getter();
		}
		set
		{
			setter(value);
		}
	}

	public Ref(Func<T> getter, Action<T> setter)
	{
		this.getter = getter;
		this.setter = setter;
	}
}
