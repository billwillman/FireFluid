using System;

[Serializable]
public class WFCMask
{
	public const char WILDCARD = '?';

	public string value = "???";

	public WFCMask(string value)
	{
		this.value = value;
	}

	public bool Fits(WFCMask other)
	{
		for (int i = 0; i < value.Length; i++)
		{
			if (value[i] != other.value[i] && value[i] != '?' && other.value[i] != '?')
			{
				return false;
			}
		}
		return true;
	}
}
