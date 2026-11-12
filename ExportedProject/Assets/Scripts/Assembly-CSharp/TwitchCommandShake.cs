public class TwitchCommandShake : ITwitchCommand
{
	private int cost = 20;

	public void Execute(string user, string[] args)
	{
		TwitchCommandDefinitions.Get().Shake(user);
	}

	public string GetCommandName()
	{
		return "shake";
	}

	public int GetCost(string[] args)
	{
		return cost;
	}

	public string GetDescription()
	{
		return $"Shake the camera (cost {cost} points)";
	}
}
