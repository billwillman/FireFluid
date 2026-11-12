public class TwitchCommandBurden : ITwitchCommand
{
	private int cost = 500;

	public void Execute(string user, string[] args)
	{
		TwitchCommandDefinitions.Get().InflictRandomBurden(user);
	}

	public string GetCommandName()
	{
		return "burden";
	}

	public int GetCost(string[] args)
	{
		if ((bool)Player.Get().GetComponentInChildren<Burden>())
		{
			return 0;
		}
		return cost;
	}

	public string GetDescription()
	{
		return $"Inflicts a random burden on the player (cost {cost} points)";
	}
}
