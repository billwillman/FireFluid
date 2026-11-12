using UnityEngine;

public class ChangeAbilityPrefabUpgrade : Upgrade
{
	public GameObject prefabToReplace;

	public GameObject newPrefab;

	public override void Apply(Player player)
	{
		base.Apply(player);
		SpawnAtEnemies[] componentsInChildren = player.GetComponentsInChildren<SpawnAtEnemies>();
		foreach (SpawnAtEnemies spawnAtEnemies in componentsInChildren)
		{
			if (spawnAtEnemies.spawnPrefab == prefabToReplace)
			{
				spawnAtEnemies.spawnPrefab = newPrefab;
				break;
			}
		}
		ShootProjectile[] componentsInChildren2 = player.GetComponentsInChildren<ShootProjectile>();
		foreach (ShootProjectile shootProjectile in componentsInChildren2)
		{
			if (shootProjectile.projectilePrefab.gameObject == prefabToReplace)
			{
				shootProjectile.projectilePrefab = newPrefab.GetComponent<Projectile>();
				break;
			}
		}
		Chains[] componentsInChildren3 = player.GetComponentsInChildren<Chains>();
		foreach (Chains chains in componentsInChildren3)
		{
			if (chains.chainPrefab.gameObject == prefabToReplace)
			{
				chains.chainPrefab = newPrefab;
				break;
			}
		}
	}
}
