public class TwitchCommandLevelUp : ITwitchCommand
{
	private int cost = 500;

	public void Execute(string user, string[] args)
	{
		TwitchCommandDefinitions.Get().LevelUp(user);
	}

	public string GetCommandName()
	{
		return "levelup";
	}

	public int GetCost(string[] args)
	{
		return cost;
	}

	public string GetDescription()
	{
		return $"Level up the player (cost {500} points)";
	}
}
