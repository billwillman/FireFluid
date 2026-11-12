public interface IFruit
{
	void UpdateGrowth(float growth);

	void Initialize(PlantData plantData, int fruitIndex);

	void CheckConsumed();
}
