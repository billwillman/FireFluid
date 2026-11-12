using System;

[Serializable]
public class ResolutionSetting
{
	public int width;

	public int height;

	public ResolutionSetting(int width, int height)
	{
		this.width = width;
		this.height = height;
	}

	public string GetDescription()
	{
		return width + "x" + height;
	}
}
