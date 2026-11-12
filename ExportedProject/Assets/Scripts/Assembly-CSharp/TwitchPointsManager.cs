using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TwitchPointsManager : MonoBehaviour
{
	private Dictionary<string, UserData> viewers = new Dictionary<string, UserData>();

	private float rewardInterval = 5f;

	private float currentRewardInterval;

	private int rewardAmount = 10;

	private float inactivityTimeout = 300f;

	private Dictionary<string, ITwitchCommand> commands;

	public bool enableCommands = true;

	public bool enableGatheringPoints = true;

	private void Awake()
	{
		commands = new Dictionary<string, ITwitchCommand>(StringComparer.OrdinalIgnoreCase);
		AddCommand(new TwitchCommandPoints(this));
		AddCommand(new TwitchCommandHelp(commands));
	}

	private void Start()
	{
		ITwitchClient client = TwitchManager.Instance.GetClient();
		client.OnChatMessage += HandleChatMessage;
		client.OnBitsReceived += HandleBits;
		client.OnChannelPointRedeemed += HandleChannelPoints;
		client.OnSubscription += HandleSubsciption;
	}

	private void Update()
	{
		UpdateViewerPoints();
	}

	public void AddCommand(ITwitchCommand command)
	{
		commands[command.GetCommandName()] = command;
	}

	public void UpdateViewerPoints()
	{
		DateTime utcNow = DateTime.UtcNow;
		currentRewardInterval -= Time.unscaledDeltaTime;
		if (!enableGatheringPoints || !(currentRewardInterval < 0f))
		{
			return;
		}
		currentRewardInterval = rewardInterval;
		foreach (KeyValuePair<string, UserData> viewer in viewers)
		{
			UserData value = viewer.Value;
			if ((utcNow - value.lastActive).TotalSeconds < (double)inactivityTimeout)
			{
				value.points += rewardAmount;
			}
		}
	}

	public bool TrySpendPoints(string username, int cost)
	{
		if (cost < 0)
		{
			return false;
		}
		if (viewers.ContainsKey(username) && viewers[username].points >= cost)
		{
			viewers[username].points -= cost;
			return true;
		}
		return false;
	}

	public int GetPoints(string username)
	{
		if (viewers.TryGetValue(username, out var value))
		{
			return value.points;
		}
		return 0;
	}

	public void HandleChatMessage(string user, string msg)
	{
		if (!viewers.TryGetValue(user, out var value))
		{
			value = new UserData();
			viewers[user] = value;
		}
		value.lastActive = DateTime.UtcNow;
		if (msg.StartsWith("!"))
		{
			if (enableCommands)
			{
				string[] array = msg.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0)
				{
					return;
				}
				string text = array[0];
				string[] args = ((array.Length > 1) ? array[1..] : Array.Empty<string>());
				if (commands.TryGetValue(text, out var cmd))
				{
					int cost = cmd.GetCost(args);
					if (!TrySpendPoints(user, cost))
					{
						int points = GetPoints(user);
						int num = cost - points;
						TwitchManager.Instance.GetClient().SendMessage($"@{user}, not enough points! You need {cost}, but you only have {points} ({num} short). Type !points to check your balance.");
						return;
					}
					UnityThread.Enqueue(delegate
					{
						try
						{
							cmd.Execute(user, args);
						}
						catch
						{
							TwitchManager.Instance.GetClient().SendMessage("@" + user + ", something went wrong executing your command.");
						}
					});
				}
				else
				{
					TwitchManager.Instance.GetClient().SendMessage("@" + user + ", unknown command: " + text);
				}
			}
			else
			{
				TwitchManager.Instance.GetClient().SendMessage("@" + user + ", commands are currently disabled.");
			}
		}
		else
		{
			if (!msg.StartsWith("?"))
			{
				return;
			}
			string[] array2 = msg.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 0)
			{
				string text2 = array2[0];
				if (commands.TryGetValue(text2, out var value2))
				{
					TwitchManager.Instance.GetClient().SendMessage("@" + user + ", " + value2.GetDescription());
				}
				else
				{
					TwitchManager.Instance.GetClient().SendMessage("@" + user + ", unknown command: " + text2);
				}
			}
		}
	}

	public void HandleBits(string user, int amount)
	{
		NotificationManager.PushNotification("<color=#00FFFF><b>[BITS]</b></color> <b><color=#FFD700>" + user + "</color></b> " + string.Format("cheered <b><color={0}>{1:N0}</color></b> bits!<br>", "#00FFFF", amount) + "<color=#AAAAAA><i>*Thanks for the support!*</i></color>");
	}

	public void HandleChannelPoints(string user, string reward)
	{
	}

	public void HandleSubsciption(string user, string tier)
	{
		NotificationManager.PushNotification("<color=#FFB347><b>[SUB]</b></color> <b><color=#FFD700>" + user + "</color></b> just subscribed at <color=#FFD700>" + tier + "</color>!<br><color=#FF69B4><i>*Welcome to the club!* <3</i></color>");
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.fontSize = 24;
		gUIStyle.normal.textColor = Color.white;
		float num = 20f;
		float num2 = 20f;
		float num3 = 32f;
		List<KeyValuePair<string, UserData>> list = new List<KeyValuePair<string, UserData>>(viewers);
		list.Sort((KeyValuePair<string, UserData> a, KeyValuePair<string, UserData> b) => b.Value.points.CompareTo(a.Value.points));
		int num4 = 0;
		foreach (KeyValuePair<string, UserData> item in list)
		{
			string text = item.Key + ": " + item.Value.points;
			Vector2 vector = gUIStyle.CalcSize(new GUIContent(text));
			float x = (float)Screen.width - vector.x - num;
			float y = num2 + (float)num4 * num3;
			GUI.Label(new Rect(x, y, vector.x + 32f, vector.y), text, gUIStyle);
			num4++;
		}
	}
}
