using System;
using System.Collections;
using UnityEngine;

public class TwitchClientMock : ITwitchClient
{
	private readonly MonoBehaviour coroutineHost;

	private readonly bool autoDemoEnabled;

	private Coroutine demoCoroutine;

	public bool IsConnected => true;

	public event Action<string, string> OnChatMessage;

	public event Action<string, int> OnBitsReceived;

	public event Action<string, string> OnSubscription;

	public event Action<string, string> OnChannelPointRedeemed;

	public TwitchClientMock(MonoBehaviour host, bool autoDemo)
	{
		coroutineHost = host;
		autoDemoEnabled = autoDemo;
	}

	public void Connect()
	{
		Debug.Log("[TwitchClientMock] Connected (TestMode).");
		if (autoDemoEnabled && coroutineHost != null)
		{
			demoCoroutine = coroutineHost.StartCoroutine(AutoDemoLoop());
		}
	}

	public void Disconnect()
	{
		if (demoCoroutine != null)
		{
			coroutineHost.StopCoroutine(demoCoroutine);
		}
		Debug.Log("[TwitchClientMock] Disconnected (TestMode).");
	}

	public void SimulateChat(string user, string message)
	{
		this.OnChatMessage?.Invoke(user, message);
	}

	public void SimulateBits(string user, int amount)
	{
		this.OnBitsReceived?.Invoke(user, amount);
	}

	public void SimulateSub(string user, string tier)
	{
		this.OnSubscription?.Invoke(user, tier);
	}

	public void SimulateChannelPointReward(string user, string reward)
	{
		this.OnChannelPointRedeemed?.Invoke(user, reward);
	}

	private IEnumerator AutoDemoLoop()
	{
		string[] exampleUsers = new string[4] { "Alice", "Bob", "Charlie", "Daisy" };
		string[] exampleMessages = new string[5] { "Hello!", "Good luck!", "Nice move!", "Wow!", "Haha" };
		string[] exampleSubTiers = new string[3] { "Tier 1", "Tier 2", "Tier 3" };
		Debug.Log("[TwitchClientMock] Auto demo mode started.");
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 4f));
			int num = UnityEngine.Random.Range(0, 4);
			string user = exampleUsers[UnityEngine.Random.Range(0, exampleUsers.Length)];
			switch (num)
			{
			case 0:
				SimulateChat(user, exampleMessages[UnityEngine.Random.Range(0, exampleMessages.Length)]);
				break;
			case 1:
				SimulateBits(user, UnityEngine.Random.Range(100, 1000));
				break;
			case 2:
				SimulateSub(user, exampleSubTiers[UnityEngine.Random.Range(0, exampleSubTiers.Length)]);
				break;
			}
		}
	}

	public void SendMessage(string message)
	{
	}
}
