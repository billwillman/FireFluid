using UnityEngine;

[DefaultExecutionOrder(100)]
public class ProceduralPlantGrowth : MonoBehaviour
{
	private int growthSteps = 10;

	private ProceduralPlant plant;

	public static int GetInitialGrowSteps()
	{
		return 10;
	}

	public static int GetCostPerGrow()
	{
		return 1 + Mathf.Max(0, PersistentDataComponent.GetDataInstance().growthStepsTree / 3);
	}

	public int GetGrowthSteps()
	{
		return GetInitialGrowSteps() + PersistentDataComponent.GetDataInstance().growthStepsTree;
	}

	private void Start()
	{
		growthSteps = GetGrowthSteps();
		plant = GetComponent<ProceduralPlant>();
		for (int i = 0; i < growthSteps; i++)
		{
			plant.GrowStep();
		}
		ProceduralPlantRenderer component = GetComponent<ProceduralPlantRenderer>();
		for (int j = 0; j < growthSteps; j++)
		{
			component.AdvanceAnimation(100f);
		}
		Fruit[] componentsInChildren = GetComponentsInChildren<Fruit>();
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].TreeInitialized();
		}
	}

	private void Update()
	{
		if (growthSteps < GetGrowthSteps())
		{
			plant.GrowStep();
			growthSteps++;
		}
	}
}
