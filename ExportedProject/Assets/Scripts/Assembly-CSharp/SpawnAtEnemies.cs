using System.Collections.Generic;
using UnityEngine;

public class SpawnAtEnemies : Ability
{
	public Vector3 offset;

	public Vector3 velocity;

	public float enemySearchRadius = 64f;

	public GameObject spawnPrefab;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		if (spawnPrefab != null)
		{
			Spawn(spawnPrefab, parameters.targetPosition);
		}
		return base.Trigger(parameters);
	}

	public void Spawn(GameObject prefab, Vector3 areaCenter)
	{
		List<Enemy> allInArea = Enemy.GetAllInArea(areaCenter, enemySearchRadius);
		if (allInArea.Count <= 0)
		{
			return;
		}
		Vector3 position = allInArea[Random.Range(0, allInArea.Count)].transform.position + offset;
		Projectile component = Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<Projectile>();
		if (!component)
		{
			return;
		}
		component.impactRadius *= Mathf.Sqrt(owner.area.GetCurrentValue());
		component.aoeDamage *= owner.damage.GetCurrentValue();
		component.hitDamage *= owner.damage.GetCurrentValue();
		component.Initialize(owner, velocity);
		List<IProjectileModification> list = new List<IProjectileModification>();
		GetComponentsInChildren(list);
		foreach (IProjectileModification item in list)
		{
			item.OnInstantiateProjectile(component);
		}
	}
}
