using System.Collections.Generic;

public class BurdenRewardRandomAttributeWithRequirement : BurdenRewardRandomAttribute
{
	public string contains = "damageMultiplier";

	public override List<string> FilterAttributes(List<string> attributePaths)
	{
		attributePaths = base.FilterAttributes(attributePaths);
		List<string> list = new List<string>();
		foreach (string attributePath in attributePaths)
		{
			if (attributePath.Contains(contains))
			{
				list.Add(attributePath);
			}
		}
		return list;
	}
}
