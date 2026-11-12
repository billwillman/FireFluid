using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class PersistentDataComponent : MonoBehaviour
{
	public const int HIGHEST_UNLOCKABLE_LEVEL = 2;

	public bool enableSaving = true;

	public float autoSaveTimeIntervall = 60f;

	private static float lastSaveTime;

	private static PersistentDataComponent instance = null;

	public PersistentData defaultStartData = new PersistentData();

	private PersistentData data;

	private static PersistentData fallbackData = PersistentData.CreateFallbackPersistentData();

	public static PersistentData GetDataInstance()
	{
		if (instance == null || instance.data == null)
		{
			return fallbackData;
		}
		return instance.data;
	}

	public static PersistentDataComponent GetComponentInstance()
	{
		if (instance == null)
		{
			return null;
		}
		return instance;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			GetComponentInstance().LoadData("save.sav");
			lastSaveTime = Time.time;
			if (base.transform.parent == null)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}
		else
		{
			data = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static void SaveAndSetNull()
	{
		if (!(instance == null))
		{
			if (instance.data != null)
			{
				instance.SaveData(instance.data);
			}
			instance.data = null;
		}
	}

	public float GetLastSaveTime()
	{
		return lastSaveTime;
	}

	public void Save()
	{
		lastSaveTime = Time.time;
		if (data != null)
		{
			SaveData(data);
		}
	}

	private void _Save()
	{
		SaveData(data);
	}

	public void SaveDataThreaded(PersistentData dataToSave)
	{
		if (dataToSave == null)
		{
			return;
		}
		lastSaveTime = Time.time;
		string filePath = dataToSave.GetFullSaveFilePath();
		if (data != null)
		{
			new Thread((ThreadStart)delegate
			{
				SaveWithFilePath(dataToSave, filePath);
			}).Start();
		}
	}

	public void SaveData(PersistentData dataToSave)
	{
		SaveWithFilePath(dataToSave, dataToSave.GetFullSaveFilePath());
	}

	public void SaveWithFilePath(PersistentData dataToSave, string filePath)
	{
		if (!enableSaving)
		{
			Debug.Log("saving disabled.");
			return;
		}
		Debug.Log("saveing [" + filePath + "] to disk");
		data.upgradeEntries.Clear();
		foreach (UpgradeEntry value in data.upgradeEntriesMap.Values)
		{
			data.upgradeEntries.Add(value);
		}
		data.sacrificeEntries.Clear();
		data.spendSacrifices = 0;
		foreach (SacrificeEntry value2 in data.sacrificeEntriesMap.Values)
		{
			data.sacrificeEntries.Add(value2);
			data.spendSacrifices += value2.level;
		}
		dataToSave.dynamicData.Clear();
		foreach (string key in dataToSave.dataMap.Keys)
		{
			dataToSave.dynamicData.Add(new DynamicDataItem(key, dataToSave.dataMap[key]));
		}
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(dataToSave.GetType());
			using MemoryStream memoryStream = new MemoryStream();
			xmlSerializer.Serialize(memoryStream, dataToSave);
			memoryStream.Position = 0L;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(memoryStream);
			xmlDocument.Save(filePath);
		}
		catch (Exception ex)
		{
			Debug.Log("an exception accured while trying to save the savegame: " + ex);
		}
	}

	public PersistentData LoadData(string saveFile, bool setAsCurrentData = true)
	{
		PersistentData persistentData = (PersistentData)defaultStartData.DeepClone();
		persistentData.saveFileName = saveFile;
		string fullSaveFilePath = persistentData.GetFullSaveFilePath();
		Debug.Log("loading [" + fullSaveFilePath + "] from disk");
		try
		{
			if (File.Exists(fullSaveFilePath))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fullSaveFilePath);
				using StringReader input = new StringReader(xmlDocument.OuterXml);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PersistentData));
				using XmlReader xmlReader = new XmlTextReader(input);
				persistentData = (PersistentData)xmlSerializer.Deserialize(xmlReader);
			}
			else
			{
				Debug.Log("file [" + fullSaveFilePath + "] doesn't exist.");
			}
		}
		catch (Exception ex)
		{
			Debug.Log("an exception accured while trying to load the savegame: " + ex);
		}
		persistentData.saveFileName = saveFile;
		if (persistentData.saveFileIdentifier < 0)
		{
			persistentData.saveFileIdentifier = UnityEngine.Random.Range(0, int.MaxValue);
		}
		persistentData.upgradeEntriesMap.Clear();
		foreach (UpgradeEntry upgradeEntry in persistentData.upgradeEntries)
		{
			persistentData.upgradeEntriesMap[upgradeEntry.id] = upgradeEntry;
		}
		persistentData.sacrificeEntriesMap.Clear();
		persistentData.spendSacrifices = 0;
		foreach (SacrificeEntry sacrificeEntry in persistentData.sacrificeEntries)
		{
			persistentData.sacrificeEntriesMap.Add(sacrificeEntry.id, sacrificeEntry);
			persistentData.spendSacrifices += sacrificeEntry.level;
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int unlockedLevel in persistentData.unlockedLevels)
		{
			hashSet.Add(unlockedLevel);
		}
		persistentData.unlockedLevels.Clear();
		foreach (int item in hashSet)
		{
			persistentData.unlockedLevels.Add(item);
		}
		foreach (AttributeModification treeModification in persistentData.treeModifications)
		{
			treeModification.EvaluateModificationValue();
		}
		foreach (DynamicDataItem dynamicDatum in persistentData.dynamicData)
		{
			persistentData.dataMap.Add(dynamicDatum.key, dynamicDatum.data);
		}
		if (setAsCurrentData)
		{
			data = persistentData;
		}
		return persistentData;
	}

	public void StartSaveGame(PersistentData data)
	{
	}

	public void OnDestroy()
	{
		if (data != null)
		{
			SaveData(data);
		}
	}

	public void ActStarted(int actIndex)
	{
	}

	public void ActFinished(int actIndex)
	{
	}

	public void RunStarted()
	{
	}

	public void RunRecovered()
	{
	}

	private void Update()
	{
		if (data != null)
		{
			data.playTime += Time.unscaledDeltaTime;
		}
		if (Time.time - GetLastSaveTime() > autoSaveTimeIntervall && data != null)
		{
			SaveDataThreaded(data);
		}
	}
}
