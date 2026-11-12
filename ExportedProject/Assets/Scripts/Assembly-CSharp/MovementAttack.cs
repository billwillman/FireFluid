using UnityEngine;

public class MovementAttack : MeleeAttack
{
	public float timeOfMovement;

	private Vector3 startPosition;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		startPosition = owner.transform.position;
		return base.Trigger(parameters);
	}

	public override void Update()
	{
		if (attackTriggered)
		{
			float value = Mathf.Clamp01((progress - timeOfMovement) / (timeOfImpact - timeOfMovement));
			owner.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.Clamp01(value));
		}
		base.Update();
	}
}
