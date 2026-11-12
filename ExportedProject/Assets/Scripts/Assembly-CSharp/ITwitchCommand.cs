public interface ITwitchCommand
{
	string GetCommandName();

	string GetDescription();

	int GetCost(string[] args);

	void Execute(string user, string[] args);
}
