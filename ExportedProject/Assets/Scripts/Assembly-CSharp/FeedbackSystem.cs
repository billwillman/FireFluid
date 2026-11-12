using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FeedbackSystem : MonoBehaviour
{
	public const string APPLICATION_IDENTIFIER = "Ignitement";

	private const string FEEDBACK_URL = "https://soerb.games/submit.php";

	private const string API_KEY = "0g9f4h9vs0984b40ajdkf093409w943zld0176490paf";

	private static FeedbackSystem _instance;

	public static FeedbackSystem Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindFirstObjectByType<FeedbackSystem>();
				if (_instance == null)
				{
					GameObject obj = new GameObject("FeedbackSystem");
					_instance = obj.AddComponent<FeedbackSystem>();
					Object.DontDestroyOnLoad(obj);
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void Send(string message)
	{
		List<string> list = new List<string>();
		if ((bool)UpgradeManager.Get() && SceneManager.Get().IsInUnderworld())
		{
			foreach (string item in UpgradeManager.Get().upgradeHistory)
			{
				list.Add(item);
			}
		}
		Feedback feedback = new Feedback
		{
			applicationIdentifier = "Ignitement",
			version = BuildInformation.GetVersionString(),
			saveFileIdentifier = PersistentDataComponent.GetDataInstance().saveFileIdentifier,
			message = message,
			playTime = PersistentDataComponent.GetDataInstance().playTime,
			unlockedLevels = PersistentDataComponent.GetDataInstance().unlockedLevels.Count,
			numberOfRunsPlayed = PersistentDataComponent.GetDataInstance().GetNumberOfRuns(),
			numberOfRunsWon = PersistentDataComponent.GetDataInstance().GetNumberOfRunsWon(),
			totalCurrencyCollected = PersistentDataComponent.GetDataInstance().totalCurrencyCollected,
			totalCurrencySpent = PersistentDataComponent.GetDataInstance().totalCurrencyCollected - PersistentDataComponent.GetDataInstance().currency,
			upgradesUnlocked = PersistentDataComponent.GetDataInstance().GetNumberOfUnlockedUpgrades(),
			heatAdded = PersistentDataComponent.GetDataInstance().GetNumberOfHeatAdded(),
			upgradesInactive = PersistentDataComponent.GetDataInstance().GetNumberOfInactiveUpgrades(),
			currencyTossedInTheWell = PersistentDataComponent.GetDataInstance().currencyTossedIntoWell,
			currentScene = SceneManager.Get().currentlyLoadedScene.ToString(),
			currentPlayerLevel = (Player.Get() ? Player.Get().GetAttributes().level : 0),
			currentRunTime = ((SceneManager.Get().IsInUnderworld() && (bool)TimeManager.Get()) ? TimeManager.Get().time : 0f),
			currentMask = PersistentDataComponent.GetDataInstance().currentMask,
			currentUpgrades = list
		};
		Instance.StartCoroutine(Instance.SendFeedbackCoroutine(feedback));
	}

	private IEnumerator SendFeedbackCoroutine(Feedback feedback)
	{
		string s = JsonUtility.ToJson(feedback);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		using UnityWebRequest request = new UnityWebRequest("https://soerb.games/submit.php", "POST");
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("X-API-KEY", "0g9f4h9vs0984b40ajdkf093409w943zld0176490paf");
		yield return request.SendWebRequest();
		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("Feedback send failed: " + request.error);
			NotificationManager.PushNotification("There was an error sending this feedback: <br>" + request.error);
		}
		else
		{
			Debug.Log("Feedback sent successfully: " + request.downloadHandler.text);
			NotificationManager.PushNotification("Thank you for making Ignitement a better game :)");
		}
	}
}
