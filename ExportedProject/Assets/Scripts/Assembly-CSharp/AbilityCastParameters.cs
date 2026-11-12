using UnityEngine;

public class AbilityCastParameters
{
	public CharacterAttributes caster;

	public Vector3 targetPosition;

	public AbilityCastParameters()
	{
	}

	public AbilityCastParameters(AbilityCastParameters copyFrom)
	{
		caster = copyFrom.caster;
		targetPosition = copyFrom.targetPosition;
	}
}
