using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TwitchClientIRCPubSub : ITwitchClient
{
	private readonly bool ircOnly;

	private readonly string username;

	private readonly string oauthToken;

	private readonly string channelName;

	private readonly string channelId;

	private TcpClient chatClient;

	private NetworkStream chatStream;

	private CancellationTokenSource chatCts;

	private ClientWebSocket pubSubClient;

	private CancellationTokenSource pubSubCts;

	public bool IsConnected { get; private set; }

	public event Action<string, string> OnChatMessage;

	public event Action<string, int> OnBitsReceived;

	public event Action<string, string> OnSubscription;

	public event Action<string, string> OnChannelPointRedeemed;

	public TwitchClientIRCPubSub(string username, string oauthToken, string channelName, string channelId, bool ircOnly = false)
	{
		this.username = username;
		this.oauthToken = (oauthToken.StartsWith("oauth:") ? oauthToken : ("oauth:" + oauthToken));
		this.channelName = (channelName.StartsWith("#") ? channelName : ("#" + channelName));
		this.channelId = channelId;
		this.ircOnly = ircOnly;
	}

	public void Connect()
	{
		Debug.Log("[TwitchClient] Starting connection...");
		ConnectChat();
		if (!ircOnly)
		{
			ConnectPubSub();
		}
		IsConnected = true;
	}

	public void Disconnect()
	{
		Debug.Log("[TwitchClient] Disconnecting...");
		try
		{
			chatCts?.Cancel();
			chatClient?.Close();
			if (!ircOnly)
			{
				pubSubCts?.Cancel();
				pubSubClient?.Abort();
			}
			IsConnected = false;
			Debug.Log("[TwitchClient] Disconnected.");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[TwitchClient] Disconnect error: " + ex.Message);
		}
	}

	private void ConnectChat()
	{
		chatCts = new CancellationTokenSource();
		Task.Run(async delegate
		{
			while (!chatCts.IsCancellationRequested)
			{
				try
				{
					Debug.Log("[TwitchChat] Connecting to IRC...");
					chatClient = new TcpClient();
					await chatClient.ConnectAsync("irc.chat.twitch.tv", 6667);
					chatStream = chatClient.GetStream();
					Debug.Log("[TwitchChat] Connected to IRC server.");
					await SendChatMessage("PASS " + oauthToken + "\r\n");
					await SendChatMessage("NICK " + username + "\r\n");
					await SendChatMessage("JOIN " + channelName + "\r\n");
					byte[] buffer = new byte[1024];
					while (!chatCts.IsCancellationRequested && chatClient.Connected)
					{
						int num = await chatStream.ReadAsync(buffer, 0, buffer.Length, chatCts.Token);
						if (num <= 0)
						{
							throw new Exception("IRC connection closed by server.");
						}
						string raw = Encoding.UTF8.GetString(buffer, 0, num);
						ProcessChatMessage(raw);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[TwitchChat] Connection lost: " + ex.Message + ". Reconnecting in 5s...");
					await Task.Delay(5000, chatCts.Token);
				}
			}
		}, chatCts.Token);
	}

	private async Task SendChatMessage(string msg)
	{
		try
		{
			if (chatStream != null && chatStream.CanWrite)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(msg);
				await chatStream.WriteAsync(bytes, 0, bytes.Length);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[TwitchChat] Send error: " + ex.Message);
		}
	}

	private void ProcessChatMessage(string raw)
	{
		string[] array = raw.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (text.StartsWith("PING"))
			{
				SendChatMessage("PONG :tmi.twitch.tv\r\n");
				Debug.Log("[TwitchChat] Responded to PING.");
			}
			else if (text.Contains("PRIVMSG"))
			{
				int num = text.IndexOf('!');
				int num2 = text.IndexOf(":", 1);
				if (num > 0 && num2 > 0)
				{
					string text2 = text.Substring(1, num - 1);
					string text3 = text.Substring(num2 + 1);
					Debug.Log("[TwitchChat] " + text2 + ": " + text3);
					this.OnChatMessage?.Invoke(text2, text3);
				}
			}
		}
	}

	public async void SendMessage(string message)
	{
		if (chatStream == null || !chatStream.CanWrite)
		{
			Debug.LogWarning("[TwitchChat] Tried to send message but chat stream not ready.");
			return;
		}
		if (!IsConnected)
		{
			Debug.LogWarning("[TwitchChat] Tried to send message while disconnected.");
			return;
		}
		try
		{
			string s = "PRIVMSG " + channelName + " :" + message + "\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			await chatStream.WriteAsync(bytes, 0, bytes.Length);
			Debug.Log("[TwitchChat] Sent message: " + message);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[TwitchChat] Failed to send message: " + ex.Message);
		}
	}

	private void ConnectPubSub()
	{
		pubSubCts = new CancellationTokenSource();
		Task.Run(async delegate
		{
			while (!pubSubCts.IsCancellationRequested)
			{
				try
				{
					Debug.Log("[TwitchPubSub] Connecting to PubSub...");
					pubSubClient = new ClientWebSocket();
					await pubSubClient.ConnectAsync(new Uri("wss://pubsub-edge.twitch.tv"), pubSubCts.Token);
					Debug.Log("[TwitchPubSub] Connected.");
					string s = JsonUtility.ToJson(new PubSubListen
					{
						nonce = Guid.NewGuid().ToString(),
						data = new PubSubListenData
						{
							topics = new string[3]
							{
								"channel-bits-events-v2." + channelId,
								"channel-subscribe-events-v1." + channelId,
								"channel-points-channel-v1." + channelId
							},
							auth_token = oauthToken.Replace("oauth:", "")
						}
					});
					byte[] bytes = Encoding.UTF8.GetBytes(s);
					await pubSubClient.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, pubSubCts.Token);
					Debug.Log("[TwitchPubSub] LISTEN message sent.");
					byte[] buffer = new byte[4096];
					while (!pubSubCts.IsCancellationRequested && pubSubClient.State == WebSocketState.Open)
					{
						WebSocketReceiveResult webSocketReceiveResult = await pubSubClient.ReceiveAsync(new ArraySegment<byte>(buffer), pubSubCts.Token);
						if (webSocketReceiveResult.Count <= 0)
						{
							throw new Exception("PubSub connection closed by server.");
						}
						string json = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
						ProcessPubSubMessage(json);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[TwitchPubSub] Connection lost: " + ex.Message + ". Reconnecting in 5s...");
					if (pubSubClient != null)
					{
						pubSubClient.Dispose();
					}
					await Task.Delay(5000, pubSubCts.Token);
				}
			}
		}, pubSubCts.Token);
	}

	private void ProcessPubSubMessage(string json)
	{
		try
		{
			if (json.Contains("bits_used"))
			{
				string text = ExtractJsonValue(json, "user_name");
				int num = int.Parse(ExtractJsonValue(json, "bits_used"));
				Debug.Log($"[TwitchPubSub] Bits received: {text} cheered {num}");
				this.OnBitsReceived?.Invoke(text, num);
			}
			else if (json.Contains("sub_plan"))
			{
				string text2 = ExtractJsonValue(json, "user_name");
				string text3 = ExtractJsonValue(json, "sub_plan_name");
				Debug.Log("[TwitchPubSub] Subscription: " + text2 + " subscribed " + text3);
				this.OnSubscription?.Invoke(text2, text3);
			}
			else if (json.Contains("redemption"))
			{
				string text4 = ExtractJsonValue(json, "display_name");
				string text5 = ExtractJsonValue(json, "title");
				Debug.Log("[TwitchPubSub] Channel Point redeemed: " + text4 + " -> " + text5);
				this.OnChannelPointRedeemed?.Invoke(text4, text5);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[TwitchPubSub] Failed to parse message: " + ex.Message);
		}
	}

	private string ExtractJsonValue(string json, string key)
	{
		string text = "\"" + key + "\":\"";
		int num = json.IndexOf(text);
		if (num < 0)
		{
			return "";
		}
		num += text.Length;
		int num2 = json.IndexOf("\"", num);
		if (num2 < 0)
		{
			return "";
		}
		return json.Substring(num, num2 - num);
	}
}
