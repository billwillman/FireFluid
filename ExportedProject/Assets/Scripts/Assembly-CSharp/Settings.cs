using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-1)]
[SerializeField]
public class Settings : MonoBehaviour
{
	private const string SETTINGSFILENAME = "settings.txt";

	public static string language = "EN";

	public static float musicVolume = 1f;

	public static float fxVolume = 1f;

	public static float brightness = 0.5f;

	public static bool fullScreenMode = true;

	public static ResolutionSetting resolution;

	public static FPSCap fpsCap = FPSCap._Unlimited;

	public static float overlayUIScaling = 1f;

	public static bool showInputUI = true;

	public static bool showNumbers = true;

	public static MouseCursorStyleType cursorStyle = MouseCursorStyleType.HighContrast;

	public static float cursorSize = 1f;

	public static bool enableScreenShakes = true;

	public static bool showFps = false;

	public static bool performanceMode = false;

	public static LevelUpMethod levelUpMethod = LevelUpMethod.ImmediateDraw;

	public static bool autoAttack = false;

	public static bool enableSendingFeedback = true;

	public ColorAdjustments colorAdjustments;

	private static Settings instance;

	public static Settings GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 60;
		Cursor.visible = false;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SetRegionSpecificDefaults();
		SetResolutionDefault();
		Load();
		StringLocalization.currentLanguage = language;
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
	}

	private void Start()
	{
		GameObject obj = new GameObject("GlobalBrightnessVolume");
		obj.transform.SetParent(base.transform);
		Volume volume = obj.AddComponent<Volume>();
		volume.isGlobal = true;
		volume.priority = 1000f;
		volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
		colorAdjustments = volume.profile.Add<ColorAdjustments>(overrides: true);
	}

	public void SetRegionSpecificDefaults()
	{
		string text = language;
		switch (text)
		{
		case "CHI (SIM)":
			return;
		case "CHI (TRA)":
			return;
		}
		_ = text == "KOR";
	}

	public void SetResolutionDefault()
	{
		resolution = new ResolutionSetting(Screen.width, Screen.height);
	}

	public static void SetLanguage(string id)
	{
		language = id;
		StringLocalization.currentLanguage = language;
		Debug.Log("language changed to: " + id);
	}

	private void Update()
	{
		if (fullScreenMode != Screen.fullScreen)
		{
			Resolution[] resolutions = Screen.resolutions;
			int width = resolutions[^1].width;
			int width2 = resolutions[^1].width;
			Screen.SetResolution(width, width2, fullScreenMode);
		}
		Screen.fullScreen = fullScreenMode;
		if (Input.GetKeyDown(KeyCode.F1))
		{
			showFps = !showFps;
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			showNumbers = !showNumbers;
		}
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Return))
		{
			fullScreenMode = !fullScreenMode;
		}
		if (Screen.currentResolution.width != resolution.width || Screen.currentResolution.height != resolution.height)
		{
			Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
		}
		switch (fpsCap)
		{
		case FPSCap._15:
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 15;
			break;
		case FPSCap._30:
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
			break;
		case FPSCap._60:
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			break;
		default:
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -1;
			break;
		}
		if (colorAdjustments != null)
		{
			colorAdjustments.postExposure.value = Mathf.Lerp(-2f, 2f, brightness);
		}
	}

	public static string GetFullFilePath()
	{
		_ = Application.persistentDataPath;
		return Path.Combine(Application.dataPath + "/../", "settings.txt");
	}

	public static void Save()
	{
		Debug.Log("saveing settings to disk [" + GetFullFilePath() + "]...");
		try
		{
			List<Tuple<string, object>> list = new List<Tuple<string, object>>();
			FieldInfo[] fields = typeof(Settings).GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsPublic && fields[i].IsStatic && !fields[i].IsNotSerialized)
				{
					list.Add(new Tuple<string, object>(fields[i].Name, fields[i].GetValue(instance)));
				}
			}
			XmlSerializer xmlSerializer = new XmlSerializer(list.GetType());
			using MemoryStream memoryStream = new MemoryStream();
			xmlSerializer.Serialize(memoryStream, list);
			memoryStream.Position = 0L;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(memoryStream);
			xmlDocument.Save(GetFullFilePath());
		}
		catch (Exception ex)
		{
			Debug.Log("Exception while saving settings: " + ex.Message);
		}
	}

	public static void Load()
	{
		List<Tuple<string, object>> list = new List<Tuple<string, object>>();
		if (instance == null)
		{
			return;
		}
		string fullFilePath = GetFullFilePath();
		Debug.Log("loading settings from disk...");
		try
		{
			if (File.Exists(fullFilePath))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fullFilePath);
				using StringReader input = new StringReader(xmlDocument.OuterXml);
				XmlSerializer xmlSerializer = new XmlSerializer(list.GetType());
				using XmlReader xmlReader = new XmlTextReader(input);
				list = (List<Tuple<string, object>>)xmlSerializer.Deserialize(xmlReader);
			}
			Type typeFromHandle = typeof(Settings);
			foreach (Tuple<string, object> item in list)
			{
				typeFromHandle.GetField(item.first).SetValue(instance, item.second);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("an exception accured while trying to load settings: " + ex);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			Save();
		}
	}
}
