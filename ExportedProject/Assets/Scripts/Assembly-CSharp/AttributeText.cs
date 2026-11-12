using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeText : MonoBehaviour
{
	private List<string> attributePaths = new List<string>();

	public StructuredAttributePaths root;

	private TextMeshProUGUI text;

	private bool dirty = true;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private string GetFormattedText(string fullAttributePath)
	{
		return "<i><color=#888888>" + ValueModification.AttributePathToName(fullAttributePath) + ": </color></i>" + ValueModification.GetValueText(fullAttributePath);
	}

	private void TraverseStructureAttributePath(StructuredAttributePaths structuredPaths, ref string currentString, int level = 0)
	{
		currentString += new string('\t', Mathf.Max(level - 1, 0));
		if (structuredPaths.subAttributes.Count == 0)
		{
			ValueModification.Get(Player.Get().gameObject, structuredPaths.fullAttributePath);
			currentString = currentString + GetFormattedText(structuredPaths.fullAttributePath) + "<br>";
			return;
		}
		if (level != 0)
		{
			currentString = currentString + "<b>" + ValueModification.AttributePathToName(structuredPaths.attribute) + "</b><br>";
		}
		foreach (StructuredAttributePaths subAttribute in structuredPaths.subAttributes)
		{
			TraverseStructureAttributePath(subAttribute, ref currentString, level + 1);
		}
	}

	private void Rebuild()
	{
		string text = "";
		attributePaths.Clear();
		ValueModification.GetAllValueStacks(Player.Get().gameObject, ref attributePaths);
		text = text + "<b>" + StringLocalization.Get("class_characterAttributesPlayer") + "</b><br>";
		foreach (string attributePath in attributePaths)
		{
			if (attributePath.Contains("CharacterAttributesPlayer"))
			{
				text = ((!(ValueModification.Get(Player.Get().gameObject, attributePath) is DynamicCharacterAttribute)) ? (text + "\t" + GetFormattedText(attributePath) + "<br>") : (text + "\t" + GetFormattedText(attributePath) + " +" + ValueModification.GetValueText(attributePath + ".regeneration") + "<br>"));
			}
		}
		foreach (Transform item in Player.Get().transform)
		{
			Ability component = item.GetComponent<Ability>();
			if (component == null)
			{
				continue;
			}
			attributePaths.Clear();
			ValueModification.GetAllValueStacks(item.gameObject, ref attributePaths);
			if (attributePaths.Count <= 0)
			{
				continue;
			}
			text = text + "<b>" + component.GetDisplayName() + "</b><br>";
			foreach (string attributePath2 in attributePaths)
			{
				object obj = ValueModification.Get(Player.Get().gameObject, attributePath2);
				if ((!(obj is CharacterAttribute) || !(Mathf.Abs(((CharacterAttribute)obj).GetCurrentValue()) <= 0.0001f)) && (!(obj is ValueStackFloat) || !(Mathf.Abs(((ValueStackFloat)obj).GetCurrentValue()) <= 0.0001f)) && (!(obj is ValueStackInt) || ((ValueStackInt)obj).GetCurrentValue() != 0))
				{
					text = text + "\t" + GetFormattedText(attributePath2) + "<br>";
				}
			}
		}
		this.text.text = text;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}

	private void Update()
	{
		if (dirty)
		{
			Rebuild();
			dirty = false;
		}
	}

	private void OnEnable()
	{
		dirty = true;
	}
}
