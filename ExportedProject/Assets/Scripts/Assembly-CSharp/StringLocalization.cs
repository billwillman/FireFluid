using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class StringLocalization
{
	public const bool debugMode = false;

	public const bool caseSensitivity = false;

	public static string csvFile = Path.Combine(Application.streamingAssetsPath, "IgnitementLocalization - Main.csv");

	public static string[] header;

	public static string fallbackLanguage = "EN";

	public static string currentLanguage = "EN";

	private static float tableLoadedProgress = 0f;

	private static bool tableLoaded = false;

	public const string parseCmdStart = "localize(";

	public const string parseCmdStartShort = "loc(";

	public const string parseCmdEnd = ")";

	public static Dictionary<string, Dictionary<string, string>> localizationTable;

	public static Dictionary<string, Vector2> characterSizeFactor = new Dictionary<string, Vector2>
	{
		{
			"JP",
			new Vector2(1.8f, 1f)
		},
		{
			"CHI (TRA)",
			new Vector2(2f, 1f)
		},
		{
			"CHI (SIM)",
			new Vector2(2f, 1f)
		},
		{
			"KOR",
			new Vector2(1.8f, 1f)
		},
		{
			"RU",
			new Vector2(1.1f, 1f)
		},
		{
			"UKR",
			new Vector2(1.1f, 1f)
		}
	};

	public static Dictionary<SystemLanguage, string> systemLanguageIdentifier = new Dictionary<SystemLanguage, string>
	{
		{
			SystemLanguage.English,
			"EN"
		},
		{
			SystemLanguage.German,
			"DE"
		},
		{
			SystemLanguage.Japanese,
			"JP"
		},
		{
			SystemLanguage.Dutch,
			"NL"
		},
		{
			SystemLanguage.Spanish,
			"SPA (EU)"
		},
		{
			SystemLanguage.Chinese,
			"CHI (SIM)"
		},
		{
			SystemLanguage.ChineseSimplified,
			"CHI (SIM)"
		},
		{
			SystemLanguage.ChineseTraditional,
			"CHI (TRA)"
		},
		{
			SystemLanguage.Korean,
			"KOR"
		},
		{
			SystemLanguage.Portuguese,
			"POR (EU)"
		},
		{
			SystemLanguage.Russian,
			"RU"
		},
		{
			SystemLanguage.Ukrainian,
			"UKR"
		},
		{
			SystemLanguage.Italian,
			"ITA"
		},
		{
			SystemLanguage.Polish,
			"PL"
		}
	};

	public static bool IsLoaded()
	{
		return tableLoaded;
	}

	public static float GetLoadingProgress()
	{
		return tableLoadedProgress;
	}

	public static string GetSystemLanguageIdentifier()
	{
		ValidateTable();
		SystemLanguage systemLanguage = Application.systemLanguage;
		string value = "";
		if (systemLanguageIdentifier.TryGetValue(systemLanguage, out value) && localizationTable.ContainsKey(value))
		{
			return value;
		}
		return fallbackLanguage;
	}

	private static IEnumerator LoadStringsFromTableWebRequest()
	{
		localizationTable = new Dictionary<string, Dictionary<string, string>>();
		Debug.Log("->WebRequest: " + csvFile);
		UnityWebRequest loadingRequest = UnityWebRequest.Get(csvFile);
		yield return loadingRequest.SendWebRequest();
		if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
		{
			Debug.LogError("WebRequest could not be loaded : " + loadingRequest.error);
		}
		else
		{
			LoadLocalizationLines(loadingRequest.downloadHandler.text.Split('\n'));
			tableLoaded = true;
			tableLoadedProgress = 1f;
			TextFieldLocalization.UpdateAll();
		}
		CoroutineController.Stop("LoadStringsFromTableWebRequest");
	}

	private static bool LoadStringsFromTable()
	{
		try
		{
			localizationTable = new Dictionary<string, Dictionary<string, string>>();
			LoadLocalizationLines(File.ReadAllLines(csvFile));
			tableLoaded = true;
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError("Could not load localization table: " + ex);
			return false;
		}
	}

	private static void LoadLocalizationLines(string[] lines)
	{
		int num = 0;
		foreach (string text in lines)
		{
			tableLoadedProgress = 1f * (float)num / (float)lines.Length;
			_ = new char[text.Length];
			char[] array = text.ToCharArray();
			List<string> list = new List<string>();
			int num2 = 0;
			int num3 = 0;
			bool flag = true;
			char[] array2 = array;
			foreach (char c in array2)
			{
				if (c == '"')
				{
					flag = !flag;
				}
				else if (flag && c == ',')
				{
					list.Add(text.Substring(num2, num3 - num2).Replace("\"", "").Replace("\\n", "\n"));
					num2 = num3 + 1;
				}
				num3++;
			}
			list.Add(text.Substring(num2).Replace("\"", "").Replace("\\n", "\n"));
			if (num == 0)
			{
				header = list.ToArray();
			}
			else if (list.Count > 0 && list[0] != "")
			{
				string text2 = list[0];
				text2 = text2.ToLowerInvariant();
				for (int k = 1; k < header.Length; k++)
				{
					string text3 = header[k];
					string text4 = "";
					if (list.Count > k)
					{
						text4 = list[k];
					}
					if (text3 != "" && text4 != "")
					{
						if (!localizationTable.ContainsKey(text3))
						{
							localizationTable.Add(text3, new Dictionary<string, string>());
						}
						if (text4[0] == '\'')
						{
							text4 = text4.Substring(1);
						}
						localizationTable[text3].Add(text2, text4);
					}
				}
			}
			num++;
		}
	}

	public static string Parse(string s)
	{
		int num = s.IndexOf("localize(");
		if (num >= 0)
		{
			int num2 = s.IndexOf(")");
			if (num2 < 0)
			{
				Debug.Log("GetLocalizedString parsing error.");
				return s;
			}
			s = s.Substring(0, num) + Get(s.Substring(num + "localize(".Length, num2 - num - "localize(".Length)) + s.Substring(num2 + 1);
		}
		num = s.IndexOf("loc(");
		if (num >= 0)
		{
			int num3 = s.IndexOf(")");
			if (num3 < 0)
			{
				Debug.Log("GetLocalizedString parsing error.");
				return s;
			}
			s = s.Substring(0, num) + Get(s.Substring(num + "loc(".Length, num3 - num - "loc(".Length)) + s.Substring(num3 + 1);
		}
		return s;
	}

	public static bool Has(string stringID, string language)
	{
		stringID = stringID.ToLowerInvariant();
		ValidateTable();
		if (!localizationTable.TryGetValue(currentLanguage, out var value))
		{
			return false;
		}
		if (!value.TryGetValue(stringID, out var value2))
		{
			if (localizationTable.TryGetValue(fallbackLanguage, out value) && value.TryGetValue(stringID, out value2))
			{
				return true;
			}
			return false;
		}
		if (value2 == "")
		{
			return false;
		}
		return true;
	}

	public static bool Has(string stringID)
	{
		return Has(stringID, currentLanguage);
	}

	public static string Get(string stringID, string language)
	{
		stringID = stringID.ToLowerInvariant();
		ValidateTable();
		if (!localizationTable.TryGetValue(currentLanguage, out var value))
		{
			return "missing lang. [" + currentLanguage + "]";
		}
		string value2 = "";
		value2 = ((!value.TryGetValue(stringID, out value2)) ? "" : value2.Trim());
		if (value2 == "")
		{
			if (localizationTable.TryGetValue(fallbackLanguage, out value) && value.TryGetValue(stringID, out value2))
			{
				return value2.Trim();
			}
			return currentLanguage + ":[" + stringID + "]";
		}
		return value2;
	}

	public static string Get(string stringID)
	{
		return Get(stringID, currentLanguage);
	}

	public static Vector2 GetCharSizeFactor()
	{
		if (characterSizeFactor.TryGetValue(currentLanguage, out var value))
		{
			return value;
		}
		return Vector2.one;
	}

	public static void DebugTable()
	{
		ValidateTable();
		foreach (string key in localizationTable.Keys)
		{
			Debug.Log(key);
			foreach (KeyValuePair<string, string> item in localizationTable[key])
			{
				Debug.Log(item.Key + ":" + item.Value);
			}
		}
	}

	public static void ValidateTable()
	{
		if (!tableLoaded)
		{
			LoadStringsFromTable();
		}
	}
}
