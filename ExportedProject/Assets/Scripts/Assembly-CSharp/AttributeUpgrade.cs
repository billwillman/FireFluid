using System.Collections.Generic;
using UnityEngine;

public class AttributeUpgrade : Upgrade
{
	public int maxLevel = 999;

	[Header("Modifications")]
	public List<AttributeModification> attributeModifications = new List<AttributeModification>();

	private const string VALUE_MODIFICATION_PREVIEW = "preview";

	public override void Awake()
	{
		base.Awake();
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			attributeModification.EvaluateModificationValue(upgradeEntry.level);
		}
	}

	public bool BaseRequirementsMet(Player player)
	{
		return base.RequirementsMet(player);
	}

	public override bool RequirementsMet(Player player)
	{
		bool flag = false;
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			if (ValueModification.Has(player.gameObject, attributeModification.attributePath))
			{
				flag = true;
				break;
			}
		}
		return base.RequirementsMet(player) && flag;
	}

	public override void Apply(Player player)
	{
		base.Apply(player);
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			attributeModification.ApplyModificationOnContext(player.gameObject, GetIdentifier());
		}
	}

	public override int GetMaxLevel()
	{
		return Mathf.Min(maxLevel, 5);
	}

	public string GetDescription(bool showAbilityName = false)
	{
		string text = base.GetDescription();
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			text.Replace("{" + attributeModification.attributePath + "}", attributeModification.GetDescription(upgradeEntry.level, IsInstance(), showAbilityName));
		}
		string text2 = "";
		foreach (AttributeModification attributeModification2 in attributeModifications)
		{
			if ((!IsInstance() || attributeModification2.HasAttributePath(Player.Get().gameObject)) && attributeModification2.type != ValueModifierType.Set)
			{
				text2 = text2 + attributeModification2.GetDescription(upgradeEntry.level, IsInstance(), showAbilityName) + "<br>";
			}
		}
		if (text2.Length > 0)
		{
			text = text + "<br><br>" + text2;
		}
		return text;
	}

	public override string GetHint()
	{
		string text = "";
		foreach (AttributeModification attributeModification in attributeModifications)
		{
			if (ValueModification.Has(Player.Get(), attributeModification.attributePath) && attributeModification.type != ValueModifierType.Set)
			{
				if (text != "")
				{
					text += "<br>";
				}
				string text2 = "<i><color=#888888>" + ValueModification.AttributePathToName(attributeModification.attributePath) + ": </color></i>";
				string valueText = ValueModification.GetValueText(attributeModification.attributePath);
				attributeModification.ApplyModificationOnContext(Player.Get(), "preview", fillDynamic: false);
				string valueText2 = ValueModification.GetValueText(attributeModification.attributePath);
				attributeModification.RemoveModificationFromContext(Player.Get(), "preview");
				valueText2 = (((attributeModification.type != ValueModifierType.Multiply || !(attributeModification.value < 1f)) && (attributeModification.type != ValueModifierType.Add || !(attributeModification.value < 0f))) ? ("<color=#88ff88>" + valueText2 + "</color>") : ("<color=#ff8888>" + valueText2 + "</color>"));
				text = text + text2 + " " + valueText + " -> " + valueText2;
			}
		}
		return text;
	}

	public override string GetDescription()
	{
		return GetDescription();
	}
}
