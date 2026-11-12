using System;
using System.Collections;
using System.Reflection;

public static class SerializationExtension
{
	public static object DeepClone(this object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj.GetType().IsValueType)
		{
			return obj;
		}
		if (obj is string)
		{
			return string.Copy((string)obj);
		}
		object obj2 = Activator.CreateInstance(obj.GetType());
		if (obj is IList)
		{
			IList obj3 = (IList)obj;
			IList list = (IList)obj2;
			{
				foreach (object item in obj3)
				{
					list.Add(item.DeepClone());
				}
				return list;
			}
		}
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsNotSerialized)
			{
				fieldInfo.SetValue(obj2, fieldInfo.GetValue(obj).DeepClone());
			}
		}
		return obj2;
	}
}
