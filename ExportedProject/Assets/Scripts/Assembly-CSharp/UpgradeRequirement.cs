using System;

[Serializable]
public class UpgradeRequirement
{
	public UpgradeRequirementLogicOperator logicOperator;

	public UpgradeRequirementType type = UpgradeRequirementType.HasClass;

	public string argument;

	public float value;
}
