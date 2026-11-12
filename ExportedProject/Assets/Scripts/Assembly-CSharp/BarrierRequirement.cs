using System;

[Serializable]
public class BarrierRequirement
{
	public BarrierRequirementType type;

	public int value;

	public bool IsMet()
	{
		switch (type)
		{
		case BarrierRequirementType.NumberOfRunsGreaterOrEqual:
			if (PersistentDataComponent.GetDataInstance().GetNumberOfRuns() >= value)
			{
				return true;
			}
			break;
		case BarrierRequirementType.RunsWonGreaterOrEqual:
			if (PersistentDataComponent.GetDataInstance().GetNumberOfRunsWon() >= value)
			{
				return true;
			}
			break;
		case BarrierRequirementType.CurrencyGreaterOrEqual:
			if (PersistentDataComponent.GetDataInstance().currency >= value)
			{
				return true;
			}
			break;
		case BarrierRequirementType.MaxSacrificesGreaterOrEqual:
			if (PersistentDataComponent.GetDataInstance().maxSacrifices >= value)
			{
				return true;
			}
			break;
		}
		return false;
	}
}
