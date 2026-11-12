public class TwitchCommandHeal : ITwitchCommand
{
	private int costPerHp = 10;

	public void Execute(string user, string[] args)
	{
		int result;
		int amount = ((args.Length != 0 && int.TryParse(args[0], out result)) ? result : 10);
		TwitchCommandDefinitions.Get().Heal(user, amount);
	}

	public string GetCommandName()
	{
		return "heal";
	}

	public int GetCost(string[] args)
	{
		int result;
		int num = ((args.Length != 0 && int.TryParse(args[0], out result)) ? result : 10);
		return costPerHp * num;
	}

	public string GetDescription()
	{
		return $"Heal the player for <amount> HP. Usage: !heal <amount>. Cost: {costPerHp} per HP.";
	}
}
