using System;

[Serializable]
public class DynamicDataItem
{
	public string key;

	public object data;

	public DynamicDataItem()
	{
	}

	public DynamicDataItem(string key, object data)
	{
		this.key = key;
		this.data = data;
	}
}
