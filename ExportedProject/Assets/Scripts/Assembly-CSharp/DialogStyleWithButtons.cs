using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogStyleWithButtons : DialogStyle
{
	public bool finished;

	public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

	public void Finish()
	{
		finished = true;
	}

	public override bool IsFinished()
	{
		return finished;
	}

	public override void Update()
	{
		base.Update();
		foreach (TextMeshProUGUI text in texts)
		{
			Color color = text.color;
			color.a = blend;
			text.color = color;
		}
		foreach (Image image in images)
		{
			Color color2 = image.color;
			color2.a = blend;
			image.color = color2;
		}
	}
}
