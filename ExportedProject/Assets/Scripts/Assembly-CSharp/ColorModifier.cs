using System;
using UnityEngine;

[Serializable]
public struct ColorModifier
{
	public string id;

	public Color color;

	public ColorBlendMode blendMode;

	public ColorModifier(string id, Color color, ColorBlendMode blendMode)
	{
		this.id = id;
		this.color = color;
		this.blendMode = blendMode;
	}
}
