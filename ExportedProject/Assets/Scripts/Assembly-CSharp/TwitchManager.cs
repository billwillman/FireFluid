using UnityEngine;

public class TwitchManager : MonoBehaviour
{
	[Header("Twitch Settings")]
	public bool TestMode;

	[Tooltip("If true, the mock Twitch client will automatically generate random events.")]
	public bool AutoDemoMode = true;

	private ITwitchClient client;

	private TwitchConfig config;

	private bool connected;

	public static TwitchManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public ITwitchClient GetClient()
	{
		return client;
	}

	private void Start()
	{
		if (TestMode)
		{
			Debug.Log("[TwitchManager] TestMode enabled. Using mock Twitch client.");
			SubscribeToEvents(client = new TwitchClientMock(this, AutoDemoMode));
			client.Connect();
			return;
		}
		config = TwitchConfig.Load();
		if (config == null)
		{
			Debug.LogWarning("[TwitchManager] No valid Twitch config found. Disabling TwitchManager.");
			base.gameObject.SetActive(value: false);
		}
		else
		{
			client = new TwitchClientIRCPubSub(config.username, config.oauthToken, config.channel, config.channel, ircOnly: true);
			SubscribeToEvents(client);
			client.Connect();
		}
	}

	private void Update()
	{
		if (!connected && client.IsConnected)
		{
			NotificationManager.PushNotification("Connected To Twitch!");
		}
		if (connected && !client.IsConnected)
		{
			NotificationManager.PushNotification("Connection To Twitch Lost...");
		}
		connected = client.IsConnected;
	}

	private void SubscribeToEvents(ITwitchClient c)
	{
		c.OnChatMessage += HandleChat;
		c.OnBitsReceived += HandleBits;
		c.OnSubscription += HandleSub;
		c.OnChannelPointRedeemed += HandleChannelPoint;
	}

	private void HandleChat(string user, string message)
	{
		Debug.Log("[CHAT]" + user + ":" + message);
	}

	private void HandleBits(string user, int amount)
	{
		Debug.Log($"[BITS]{user}: gifted {amount}");
	}

	private void HandleSub(string user, string tier)
	{
		Debug.Log("[SUB]" + user + ": gifted a " + tier + " sub");
	}

	private void HandleChannelPoint(string user, string reward)
	{
		Debug.Log("[CHANNELPOINTS]" + user + ": redeemed a " + reward);
	}

	private void OnApplicationQuit()
	{
		client?.Disconnect();
	}
}
