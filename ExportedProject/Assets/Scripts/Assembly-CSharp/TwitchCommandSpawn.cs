public class TwitchCommandSpawn : ITwitchCommand
{
	public void Execute(string user, string[] args)
	{
		string text = ((args.Length != 0) ? args[0] : "skeleton1");
		int result;
		int count = ((args.Length > 1 && int.TryParse(args[1], out result)) ? result : 5);
		if (!Encyclopedia.Get().GetEnemy(text))
		{
			TwitchManager.Instance.GetClient().SendMessage("@" + user + ", enemy with unknown id: " + text);
		}
		else
		{
			TwitchCommandDefinitions.Get().SpawnEnemies(user, text, count);
		}
	}

	public string GetCommandName()
	{
		return "spawn";
	}

	public int GetCost(string[] args)
	{
		string enemyId = ((args.Length != 0) ? args[0] : "skeleton1");
		int result;
		int num = ((args.Length > 1 && int.TryParse(args[1], out result)) ? result : 5);
		return TwitchCommandDefinitions.Get().GetEnemyCost(enemyId) * num;
	}

	public string GetDescription()
	{
		string text = "";
		foreach (Enemy enemy in Encyclopedia.Get().enemies)
		{
			text = text + enemy.GetIdentifier() + ",";
		}
		return "Spawn enemies! Valid enemyIds: " + text + " Usage: !spawn <enemyId> <count>";
	}
}
