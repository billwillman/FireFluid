using System;

public interface ITwitchClient
{
	bool IsConnected { get; }

	event Action<string, string> OnChatMessage;

	event Action<string, int> OnBitsReceived;

	event Action<string, string> OnSubscription;

	event Action<string, string> OnChannelPointRedeemed;

	void Connect();

	void Disconnect();

	void SendMessage(string message);
}
