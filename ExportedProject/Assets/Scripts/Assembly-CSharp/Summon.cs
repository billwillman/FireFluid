using System.Collections.Generic;
using UnityEngine;

public class Summon : Ability
{
	public GameObject summonEffectPrefab;

	public List<GameObject> summonPrefabs = new List<GameObject>();

	public int numberOfSummons = 8;

	public int summonLimit = 128;

	public SummonPosition summonPosition;

	public float scatterRadius = 16f;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private List<GameObject> currentInstances = new List<GameObject>();

	public override bool Trigger(AbilityCastParameters parameters)
	{
		for (int i = 0; i < numberOfSummons; i++)
		{
			cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				Vector2 vector = Random.insideUnitCircle * scatterRadius;
				Vector3 near = ((summonPosition != SummonPosition.ScatteredAroundCaster) ? (parameters.caster.transform.position + (parameters.targetPosition - parameters.caster.transform.position) * 8f + new Vector3(vector.x, 0f, vector.y)) : (parameters.caster.transform.position + new Vector3(vector.x, 0f, vector.y)));
				near = PositionProvider.Get().GetValidPositionNear(near);
				GameObject item = Object.Instantiate(summonPrefabs[Random.Range(0, summonPrefabs.Count)], near, Quaternion.identity);
				currentInstances.Add(item);
				Object.Instantiate(summonEffectPrefab, near, Quaternion.identity);
				return true;
			}));
			cinematicMachine.PushAction(new CinematicWait(0.3f));
		}
		return base.Trigger(parameters);
	}

	public override void Update()
	{
		for (int num = currentInstances.Count - 1; num >= 0; num--)
		{
			if (currentInstances[num] == null)
			{
				currentInstances.RemoveAt(num);
			}
		}
		cinematicMachine.UpdateActions(Time.deltaTime);
		base.Update();
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (currentInstances.Count >= summonLimit)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}
}
