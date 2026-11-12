using UnityEngine;

public interface IEnemyAbility
{
	bool CastIntent();

	Vector3 GetPreferredTargetPosition();
}
