using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
	public bool canBeFoundDuringRun = true;

	public bool unique = true;

	public bool unlockedFromTheStart;

	public bool lockedInDemo;

	public string identifier = "upgrade";

	public new string name = "Upgrade";

	public string description = "This upgrade does stuff.";

	public Sprite icon;

	[Header("Requirements")]
	public List<UpgradeRequirement> requirements = new List<UpgradeRequirement>();

	public List<CharacterMask> maskAffinities = new List<CharacterMask>();

	private bool isInstance;

	public virtual void Awake()
	{
		isInstance = true;
	}

	public bool IsInstance()
	{
		return isInstance;
	}

	public bool HasAffinity()
	{
		foreach (CharacterMask maskAffinity in maskAffinities)
		{
			if (maskAffinity.GetIdentifier() == PersistentDataComponent.GetDataInstance().currentMask)
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool RequirementsMet(Player player)
	{
		bool flag = true;
		int num = 0;
		if (PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier()).level < 1)
		{
			return false;
		}
		foreach (UpgradeRequirement requirement in requirements)
		{
			bool flag2 = false;
			switch (requirement.type)
			{
			case UpgradeRequirementType.HasUpgradeWithId:
				flag2 = UpgradeManager.Get().upgradeHistory.Contains(requirement.argument);
				break;
			case UpgradeRequirementType.ValueLessThen:
				flag2 = ValueModification.Get(player.gameObject, requirement.argument) != null && ValueModification.GetAsFloat(player.gameObject, requirement.argument) < requirement.value - 0.0001f;
				break;
			case UpgradeRequirementType.ValueGreaterThen:
				flag2 = ValueModification.Get(player.gameObject, requirement.argument) != null && ValueModification.GetAsFloat(player.gameObject, requirement.argument) > requirement.value + 0.0001f;
				break;
			case UpgradeRequirementType.AnyValueGreaterThen:
			{
				List<string> attributePaths = new List<string>();
				ValueModification.GetAllValueStacks(player.gameObject, ref attributePaths);
				foreach (string item in attributePaths)
				{
					if (item.Contains(requirement.argument) && ValueModification.Get(player.gameObject, item) != null && ValueModification.GetAsFloat(player.gameObject, item) > requirement.value)
					{
						flag2 = true;
						break;
					}
				}
				break;
			}
			}
			switch (requirement.logicOperator)
			{
			case UpgradeRequirementLogicOperator.And:
				flag = flag && flag2;
				break;
			case UpgradeRequirementLogicOperator.AndNot:
				flag = flag && !flag2;
				break;
			case UpgradeRequirementLogicOperator.Or:
				flag = flag || flag2;
				break;
			case UpgradeRequirementLogicOperator.OrNot:
				flag = flag || !flag2;
				break;
			}
			num++;
		}
		return flag;
	}

	public bool UnlockRequirementsMet()
	{
		bool flag = true;
		int num = 0;
		foreach (UpgradeRequirement requirement in requirements)
		{
			bool flag2 = true;
			switch (requirement.type)
			{
			case UpgradeRequirementType.HasUpgradeWithId:
				flag2 = PersistentDataComponent.GetDataInstance().UpgradeIsUnlocked(requirement.argument);
				if (!flag2)
				{
					Debug.Log("Upgrade:" + GetIdentifier() + " is missing required upgrade " + requirement.argument);
				}
				break;
			}
			if (requirement.logicOperator == UpgradeRequirementLogicOperator.And)
			{
				flag = flag && flag2;
			}
			num++;
		}
		foreach (CharacterMask maskAffinity in maskAffinities)
		{
			flag &= PersistentDataComponent.GetDataInstance().MaskIsUnlocked(maskAffinity.GetIdentifier());
			if (!PersistentDataComponent.GetDataInstance().MaskIsUnlocked(maskAffinity.GetIdentifier()))
			{
				Debug.Log("Upgrade:" + GetIdentifier() + " is missin requiremed mask " + maskAffinity.GetIdentifier());
			}
		}
		return flag;
	}

	public string ReplaceValue(string text, string valueIdentifier, float value)
	{
		return text.Replace(valueIdentifier, ((value >= 0f) ? "+" : "-") + value);
	}

	public int GetLevel()
	{
		return PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier()).level;
	}

	public virtual int GetMaxLevel()
	{
		return 1;
	}

	public bool CanLevelUp()
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(GetLevelUpCost()) && GetMaxLevel() > upgradeEntry.level)
		{
			return true;
		}
		return false;
	}

	public void LevelUp()
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		int levelUpCost = GetLevelUpCost();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(levelUpCost))
		{
			PersistentDataComponent.GetDataInstance().SpendCurrency(levelUpCost);
			upgradeEntry.level++;
		}
	}

	public bool CanActivateOrDeactivate()
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		if (upgradeEntry.level <= 0)
		{
			return false;
		}
		if (upgradeEntry.active && PersistentDataComponent.GetDataInstance().GetNumberOfInactiveUpgrades() >= PersistentDataComponent.GetDataInstance().GetMaxInactiveUpgrades())
		{
			return false;
		}
		if (!PersistentDataComponent.GetDataInstance().CanSpendCurrency(GetActivateDeactivateCost()))
		{
			return false;
		}
		return true;
	}

	public void ActivateOrDeactivate()
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		int activateDeactivateCost = GetActivateDeactivateCost();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(activateDeactivateCost))
		{
			PersistentDataComponent.GetDataInstance().SpendCurrency(activateDeactivateCost);
			upgradeEntry.active = !upgradeEntry.active;
		}
	}

	public int GetLevelUpCost()
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		if (upgradeEntry.level == 0)
		{
			return 10;
		}
		return Mathf.RoundToInt(5f * Mathf.Pow(2f, upgradeEntry.level - 1));
	}

	public bool MaxLevelReached()
	{
		return PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier()).level >= GetMaxLevel();
	}

	public UpgradeEntry GetEntry()
	{
		return PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
	}

	public int GetActivateDeactivateCost()
	{
		return 1;
	}

	public string GetIdentifier()
	{
		return identifier;
	}

	public virtual string GetName()
	{
		return StringLocalization.Parse(name);
	}

	public virtual string GetDescription()
	{
		return StringLocalization.Parse(description);
	}

	public virtual string GetHint()
	{
		return "";
	}

	public static string GetDescriptionAll(GameObject upgrades)
	{
		string text = "";
		Upgrade[] components = upgrades.GetComponents<Upgrade>();
		foreach (Upgrade upgrade in components)
		{
			text += upgrade.GetDescription();
		}
		return text;
	}

	public static string GetHintAll(GameObject upgrades)
	{
		string text = "";
		Upgrade[] components = upgrades.GetComponents<Upgrade>();
		foreach (Upgrade upgrade in components)
		{
			text += upgrade.GetHint();
		}
		return text;
	}

	public virtual string SubstituteVariables(string text)
	{
		string text2 = text;
		int num = 0;
		int num2 = 0;
		Debug.Log(GetType());
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(GetIdentifier());
		do
		{
			num = text2.IndexOf("{");
			if (num < 0)
			{
				continue;
			}
			num2 = text2.IndexOf("}", num);
			if (num2 < 0)
			{
				continue;
			}
			string text3 = text2.Substring(num + 1, num2 - num - 1);
			Debug.Log(text3);
			FieldInfo field = GetType().GetField(text3);
			if (field != null)
			{
				object obj = field.GetValue(this);
				string text4;
				if (obj is float)
				{
					if (text2.Length > num2 + 1 && text2[num2 + 1] == '%')
					{
						obj = (float)obj * 100f * (float)upgradeEntry.level;
					}
					text4 = (((float)obj >= 0f) ? "+" : "-") + obj;
				}
				else
				{
					text4 = obj.ToString();
				}
				text2 = text2.Substring(0, num) + text4 + text2.Substring(num2 + 1);
				continue;
			}
			Debug.Log("Field [" + text3 + "] not found. (" + GetIdentifier() + ")");
			break;
		}
		while (num >= 0 && num2 >= 0);
		return text2;
	}

	public virtual void OnDraw()
	{
	}

	public virtual void Apply(Player player)
	{
		if (!IsInstance())
		{
			Debug.LogError("Trying to apply a non-instance upgrade: " + GetIdentifier());
		}
		else if ((bool)UpgradeManager.Get() && GetIdentifier() != "")
		{
			UpgradeManager.Get().AddUpgradeToHistory(this);
		}
	}

	public static void ApplyAll(Player player, GameObject upgrades)
	{
		Upgrade[] components = upgrades.GetComponents<Upgrade>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Apply(player);
		}
	}
}
