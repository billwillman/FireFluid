using System;
using System.Collections.Generic;

public class ViewerPointsManager
{
	private class ViewerData
	{
		public int Points;

		public DateTime LastActive;
	}

	private Dictionary<string, ViewerData> viewers = new Dictionary<string, ViewerData>();

	private float rewardInterval = 60f;

	private int rewardAmount = 10;

	private float inactivityTimeout = 300f;

	public void RegisterChatActivity(string username)
	{
		if (!viewers.ContainsKey(username))
		{
			viewers[username] = new ViewerData();
		}
		viewers[username].LastActive = DateTime.UtcNow;
	}

	public void Update()
	{
		DateTime utcNow = DateTime.UtcNow;
		foreach (KeyValuePair<string, ViewerData> viewer in viewers)
		{
			ViewerData value = viewer.Value;
			if ((utcNow - value.LastActive).TotalSeconds < (double)inactivityTimeout)
			{
				value.Points += rewardAmount;
			}
		}
	}

	public bool TrySpendPoints(string username, int cost)
	{
		if (viewers.ContainsKey(username) && viewers[username].Points >= cost)
		{
			viewers[username].Points -= cost;
			return true;
		}
		return false;
	}

	public int GetPoints(string username)
	{
		if (viewers.TryGetValue(username, out var value))
		{
			return value.Points;
		}
		return 0;
	}
}
