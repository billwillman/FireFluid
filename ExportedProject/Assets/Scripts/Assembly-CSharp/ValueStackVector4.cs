using System;
using UnityEngine;

[Serializable]
public class ValueStackVector4 : ValueStack<Vector4>
{
	public ValueStackVector4(Vector4 defaultValue)
		: base(defaultValue, (ValueStackAdd<Vector4>)((Vector4 a, Vector4 b) => a + b), (ValueStackMultiply<Vector4>)((Vector4 a, Vector4 b) => new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w)))
	{
	}
}
