using UnityEngine;

public class TwitchCommandCast : ITwitchCommand
{
	private string commandName = "cast";

	private GameObject prefab;

	private int cost = 100;

	private string castName;

	public TwitchCommandCast(string commandName, GameObject prefab, int cost, string castName)
	{
		this.commandName = commandName;
		this.prefab = prefab;
		this.cost = cost;
		this.castName = castName;
	}

	public void Execute(string user, string[] args)
	{
		TwitchCommandDefinitions.Get().Cast(user, prefab);
	}

	public string GetCommandName()
	{
		return commandName;
	}

	public int GetCost(string[] args)
	{
		return cost;
	}

	public string GetDescription()
	{
		return $"Casts a {castName} (cost {cost} points)";
	}
}
