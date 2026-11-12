using System;

[Serializable]
public class PubSubListen
{
	public string type = "LISTEN";

	public string nonce;

	public PubSubListenData data;
}
