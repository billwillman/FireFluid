using UnityEngine;

internal static class ColorExtension
{
	public static Color FromHex(uint rgb)
	{
		return new Color((float)(int)(byte)((rgb & 0xFF0000) >> 16) / 256f, (float)(int)(byte)((rgb & 0xFF00) >> 8) / 256f, (float)(int)(byte)(rgb & 0xFF) / 256f);
	}
}
