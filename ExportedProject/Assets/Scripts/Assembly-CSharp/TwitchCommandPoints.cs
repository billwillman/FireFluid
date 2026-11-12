public class TwitchCommandPoints : ITwitchCommand
{
	private TwitchPointsManager pointsManager;

	public TwitchCommandPoints(TwitchPointsManager pointsManager)
	{
		this.pointsManager = pointsManager;
	}

	public void Execute(string user, string[] args)
	{
		int points = pointsManager.GetPoints(user);
		TwitchManager.Instance.GetClient().SendMessage($"@{user}, you currently have {points} points.");
	}

	public string GetCommandName()
	{
		return "points";
	}

	public int GetCost(string[] args)
	{
		return 0;
	}

	public string GetDescription()
	{
		return "Check how many points you have";
	}
}
