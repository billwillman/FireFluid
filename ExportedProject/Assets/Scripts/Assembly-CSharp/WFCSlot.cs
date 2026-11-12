using System.Collections.Generic;

public class WFCSlot
{
	public List<WFCModule> possibleModules;

	public WFCSlot()
	{
	}

	public WFCSlot(List<WFCModule> allModules)
	{
		possibleModules = new List<WFCModule>(allModules);
	}

	public bool IsCollapsed()
	{
		return possibleModules.Count == 1;
	}
}
