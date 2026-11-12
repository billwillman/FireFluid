using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BurdensText : MonoBehaviour
{
	private TextMeshProUGUI text;

	private bool dirty = true;

	private List<Burden> burdens = new List<Burden>();

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (dirty)
		{
			Rebuild();
			dirty = false;
		}
	}

	private void Rebuild()
	{
		Player.Get().GetComponentsInChildren(burdens);
		string text = "";
		if (burdens.Count > 0)
		{
			text += "<br>";
			foreach (Burden burden in burdens)
			{
				text += burden.GetShortDescription();
			}
			text += "<br>";
		}
		this.text.text = text;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}

	private void OnEnable()
	{
		dirty = true;
	}
}
