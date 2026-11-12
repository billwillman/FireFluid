using System.Collections.Generic;
using UnityEngine;

public class SummonClaws : ClawAttack
{
	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public ValueStackFloat clawSpeed = new ValueStackFloat(64f);

	public ValueStackFloat clawLength = new ValueStackFloat(8f);

	public float spread = 2f;

	public float range = 4f;

	public Claw clawPrefab;

	private int sliceCounter;

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (clawPrefab == null)
		{
			Debug.Log("slice prefab not set.");
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		Vector3 position = parameters.caster.transform.position;
		position += (parameters.targetPosition - parameters.caster.transform.position).normalized * range;
		Vector3 onUnitSphere = Random.onUnitSphere;
		onUnitSphere.y = 0f;
		Vector3 normalized = (parameters.caster.transform.position - parameters.targetPosition).normalized;
		Vector3 vector = new Vector3(normalized.z, 0f, 0f - normalized.x) + onUnitSphere * 0.5f;
		vector.Normalize();
		if (sliceCounter % 2 == 0)
		{
			vector *= -1f;
		}
		sliceCounter++;
		Vector3 vector2 = (parameters.targetPosition - parameters.caster.transform.position).normalized * spread;
		List<Claw> list = new List<Claw>();
		for (int i = 0; i < numberOfClaws.GetCurrentValue(); i++)
		{
			Vector3 vector3 = vector2 * i;
			Vector3 position2 = position - vector * clawLength.GetCurrentValue() * 0.5f + vector3;
			Vector3 targetPosition = position + vector * clawLength.GetCurrentValue() * 0.5f + vector3;
			Claw component = Object.Instantiate(clawPrefab.gameObject, position2, Quaternion.identity).GetComponent<Claw>();
			component.damage *= damageMultiplier.GetCurrentValue() * owner.damage.GetCurrentValue();
			component.Initialize(owner);
			component.GetComponent<ClawMovementStartEnd>()?.Initialize(targetPosition, clawSpeed.GetCurrentValue());
			list.Add(component);
		}
		return base.Trigger(parameters);
	}
}
