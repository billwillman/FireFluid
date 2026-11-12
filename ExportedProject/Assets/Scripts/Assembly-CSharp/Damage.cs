using UnityEngine;

public static class Damage
{
	private static Collider[] colliders = new Collider[128];

	public static void DoAoEDamage(Vector3 position, float radius, float dmg, CharacterAttributes attacker, int attackLayer)
	{
		int num = Physics.OverlapSphereNonAlloc(position, radius, colliders);
		for (int i = 0; i < num; i++)
		{
			if (!(attacker != null) || !Physics.GetIgnoreLayerCollision(attackLayer, colliders[i].gameObject.layer))
			{
				colliders[i].GetComponentInParent<IDamageable>()?.ReceiveDamage(dmg, attacker, "AoEDamage");
			}
		}
	}
}
