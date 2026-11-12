using System;
using System.IO;
using UnityEngine;

[Serializable]
public class TwitchConfig
{
	public string username;

	public string oauthToken;

	public string channel;

	public string clientId;

	public bool usePubSub;

	public static TwitchConfig Load()
	{
		string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "twitch_config.json");
		if (!File.Exists(text))
		{
			Debug.LogWarning("Twitch config not found at: " + text);
			return null;
		}
		try
		{
			TwitchConfig twitchConfig = JsonUtility.FromJson<TwitchConfig>(File.ReadAllText(text));
			if (string.IsNullOrEmpty(twitchConfig.username) || string.IsNullOrEmpty(twitchConfig.oauthToken) || string.IsNullOrEmpty(twitchConfig.channel))
			{
				Debug.LogError("Twitch config missing required fields (username, oauthToken, channel).");
				return null;
			}
			return twitchConfig;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load Twitch config: " + ex.Message);
			return null;
		}
	}
}
