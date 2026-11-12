using UnityEngine;

public class SpawnAtEnemiesPrimarySecondary : SpawnAtEnemies
{
	public GameObject mainPrefab;

	public GameObject secondaryPrefab;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	public ValueStackInt numberOfSecondarySpawns = new ValueStackInt(0);

	public float timeBetweenSpawnsMin = 0.2f;

	public float timeBetweenSpawnsMax = 0.7f;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		Transform caster = parameters.caster.transform;
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			Spawn(mainPrefab, caster.position);
			return true;
		}));
		for (int num = 0; num < numberOfSecondarySpawns.GetCurrentValue(); num++)
		{
			cinematicMachine.PushAction(new CinematicWait(Random.Range(timeBetweenSpawnsMin, timeBetweenSpawnsMax)));
			cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				Spawn(secondaryPrefab, caster.position);
				return true;
			}));
		}
		return base.Trigger(parameters);
	}

	public override void Update()
	{
		base.Update();
		cinematicMachine.UpdateActions(Time.deltaTime * owner.attackSpeed.GetCurrentValue());
	}
}
