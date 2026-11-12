using UnityEngine;

public interface IAbilityController
{
	bool AttackIntention();

	bool SecondaryActionIntention();

	Vector3 GetAimDirection();

	Vector3 GetMovementIntention();
}
