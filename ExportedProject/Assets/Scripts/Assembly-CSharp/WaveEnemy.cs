using UnityEngine;

public class WaveEnemy : Enemy
{
	public Vector3 direction;

	public override void UpdateMovementDirection()
	{
		movementDirection = direction;
	}
}
