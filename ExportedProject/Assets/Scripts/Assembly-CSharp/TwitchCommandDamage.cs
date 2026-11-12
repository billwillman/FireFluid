using UnityEngine;

public class TwitchCommandDamage : ITwitchCommand
{
	private int costPerHp = 10;

	public void Execute(string user, string[] args)
	{
		int result;
		int dmg = ((args.Length != 0 && int.TryParse(args[0], out result)) ? result : 10);
		TwitchCommandDefinitions.Get().DealDamage(user, dmg);
	}

	public string GetCommandName()
	{
		return "damage";
	}

	public int GetCost(string[] args)
	{
		int result;
		int num = Mathf.Max((args.Length != 0 && int.TryParse(args[0], out result)) ? result : 10, 0);
		return costPerHp * num;
	}

	public string GetDescription()
	{
		return $"Deals <amount> damage to the player. Usage: !damage <amount>. Cost: {costPerHp} per damage.";
	}
}
