using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PersistentData
{
	public int saveFileIdentifier = -1;

	public float playTime;

	public int totalCurrencyCollected;

	[NonSerialized]
	[XmlIgnore]
	public string saveFileName;

	[NonSerialized]
	[XmlIgnore]
	public Dictionary<string, object> dataMap = new Dictionary<string, object>();

	public List<DynamicDataItem> dynamicData = new List<DynamicDataItem>();

	public string currentMask = "impatienceandneglect";

	[XmlIgnore]
	public Dictionary<string, UpgradeEntry> upgradeEntriesMap = new Dictionary<string, UpgradeEntry>();

	public List<UpgradeEntry> upgradeEntries = new List<UpgradeEntry>();

	[XmlIgnore]
	public Dictionary<string, SacrificeEntry> sacrificeEntriesMap = new Dictionary<string, SacrificeEntry>();

	public List<SacrificeEntry> sacrificeEntries = new List<SacrificeEntry>();

	public List<AttributeModification> treeModifications = new List<AttributeModification>();

	public int currency;

	public int numberOfRuns;

	public int currencyTossedIntoWell;

	public int maxInactiveUpgrades = 7;

	public int maxSacrifices;

	[XmlIgnore]
	public int spendSacrifices;

	public int growthStepsTree;

	public int currentlySelectedLevel = 1;

	public List<int> unlockedLevels = new List<int> { 1 };

	public List<RunStats> runStats = new List<RunStats>();

	public void CollectCurrency(int currency)
	{
		this.currency += currency;
		totalCurrencyCollected += currency;
	}

	public bool CanSpendCurrency(int currency)
	{
		if (this.currency >= currency)
		{
			return true;
		}
		return false;
	}

	public void SpendCurrency(int currency)
	{
		this.currency -= currency;
	}

	public int GetNumberOfRuns()
	{
		return numberOfRuns;
	}

	public int GetNumberOfRunsWon()
	{
		int num = 0;
		foreach (RunStats runStat in runStats)
		{
			if (runStat.state == RunState.Won)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumberOfInactiveUpgrades()
	{
		int num = 0;
		foreach (UpgradeEntry value in upgradeEntriesMap.Values)
		{
			if (!value.active)
			{
				num++;
			}
		}
		return num;
	}

	public int GetMaxInactiveUpgrades()
	{
		return maxInactiveUpgrades + GetNumberOfUnlockedUpgrades() / 2;
	}

	public int GetNumberOfUnlockedUpgrades()
	{
		int num = 0;
		if (Encyclopedia.Get() == null)
		{
			return 0;
		}
		foreach (UpgradeEntry value in upgradeEntriesMap.Values)
		{
			Upgrade upgrade = Encyclopedia.Get().GetUpgrade(value.id);
			if (upgrade != null && !upgrade.unlockedFromTheStart && value.level > 0)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumberOfHeatAdded()
	{
		int num = 0;
		if (Encyclopedia.Get() == null)
		{
			return 0;
		}
		foreach (UpgradeEntry value in upgradeEntriesMap.Values)
		{
			if (value.level > 1)
			{
				num += value.level - 1;
			}
		}
		return num;
	}

	public void SaveRunStats(RunStats stats)
	{
		numberOfRuns++;
		runStats.Add(stats);
	}

	public bool UnspentSacrificeAvailable()
	{
		return spendSacrifices < maxSacrifices;
	}

	public bool MaskIsUnlocked(string maskId)
	{
		if (maskId == "impatienceandneglect")
		{
			return true;
		}
		return GetData(maskId) != null;
	}

	public static PersistentData CreateFallbackPersistentData()
	{
		return new PersistentData();
	}

	public UpgradeEntry GetUpgradeEntry(string upgradeId)
	{
		if (!upgradeEntriesMap.TryGetValue(upgradeId, out var value))
		{
			value = new UpgradeEntry();
			value.id = upgradeId;
			if ((bool)Encyclopedia.Get() && (bool)Encyclopedia.Get().GetUpgrade(upgradeId) && Encyclopedia.Get().GetUpgrade(upgradeId).unlockedFromTheStart)
			{
				value.level = 1;
			}
			upgradeEntriesMap[upgradeId] = value;
		}
		return value;
	}

	public bool UpgradeIsUnlocked(string upgradeId)
	{
		bool flag = GetUpgradeEntry(upgradeId).level > 0;
		bool flag2 = false;
		foreach (CharacterMask mask in Encyclopedia.Get().masks)
		{
			if (!MaskIsUnlocked(mask.GetIdentifier()))
			{
				continue;
			}
			foreach (Upgrade upgrade in mask.upgrades)
			{
				if (upgrade.GetIdentifier() == upgradeId)
				{
					flag2 = true;
				}
			}
		}
		return flag || flag2;
	}

	public SacrificeEntry GetSacrificeEntry(string sacrificeId)
	{
		if (!sacrificeEntriesMap.TryGetValue(sacrificeId, out var value))
		{
			value = new SacrificeEntry();
			value.id = sacrificeId;
			sacrificeEntriesMap[sacrificeId] = value;
		}
		return value;
	}

	public List<SacrificeEntry> GetAllActiveSacrificeEntries()
	{
		List<SacrificeEntry> list = new List<SacrificeEntry>();
		foreach (SacrificeEntry value in sacrificeEntriesMap.Values)
		{
			if (value.level > 0)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void ResetTreeData()
	{
		List<string> list = new List<string>();
		treeModifications.Clear();
		foreach (string key in dataMap.Keys)
		{
			if (key.Contains("Fruit_"))
			{
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			dataMap.Remove(item);
		}
		growthStepsTree = 0;
	}

	public void UnlockLevel(int level)
	{
		if (!unlockedLevels.Contains(level) && level <= 2 && level > 0)
		{
			unlockedLevels.Add(level);
		}
	}

	public int GetNumberOfUnlockedLevels()
	{
		return unlockedLevels.Count;
	}

	public bool IsLevelUnlocked(int level)
	{
		return unlockedLevels.Contains(level);
	}

	public RunStats GetLastRun()
	{
		if (runStats.Count == 0)
		{
			return null;
		}
		return runStats[runStats.Count - 1];
	}

	public int GetCompletionPercentage()
	{
		return 0;
	}

	public string GetFullSaveFilePath()
	{
		_ = Application.persistentDataPath;
		return Path.Combine(Application.dataPath + "/../", saveFileName);
	}

	public object GetDynamicData(Vector3 position, string key)
	{
		return GetData(position.GetSpatialHash() + "_" + key);
	}

	public object GetData(string key)
	{
		if (dataMap.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public void SetData(Vector3 position, string key, object data)
	{
		SetData(position.GetSpatialHash() + "_" + key, data);
	}

	public void SetData(string key, object data)
	{
		dataMap[key] = data;
	}

	public void RemoveData(string key)
	{
		dataMap.Remove(key);
	}
}
