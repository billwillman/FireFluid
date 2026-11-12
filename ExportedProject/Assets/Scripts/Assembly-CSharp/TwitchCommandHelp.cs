using System.Collections.Generic;

public class TwitchCommandHelp : ITwitchCommand
{
	private Dictionary<string, ITwitchCommand> commands;

	public TwitchCommandHelp(Dictionary<string, ITwitchCommand> commands)
	{
		this.commands = commands;
	}

	public void Execute(string user, string[] args)
	{
		string text = "";
		foreach (string key in commands.Keys)
		{
			text = text + key + ",";
		}
		TwitchManager.Instance.GetClient().SendMessage("@" + user + ", write !<command> to execute the command and ?<command> to show details for that specific command. commands: " + text);
	}

	public string GetCommandName()
	{
		return "help";
	}

	public int GetCost(string[] args)
	{
		return 0;
	}

	public string GetDescription()
	{
		return "lists all available commands";
	}
}
