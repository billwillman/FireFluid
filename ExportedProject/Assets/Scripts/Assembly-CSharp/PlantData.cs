using System;
using System.Collections.Generic;

[Serializable]
public class PlantData
{
	public List<BranchNode> branches = new List<BranchNode>();

	public int growthStep;

	public int numberOfFruits;

	public int randomSeed;
}
