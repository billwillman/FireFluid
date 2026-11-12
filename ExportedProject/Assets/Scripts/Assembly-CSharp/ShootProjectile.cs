using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Ability
{
	public ValueStackInt numberOfProjectiles = new ValueStackInt(1);

	public ValueStackFloat projectileTargetSeekingRadius = new ValueStackFloat(0f);

	public ValueStackFloat projectileScale = new ValueStackFloat(1f);

	[Range(0f, 1f)]
	public float projectileSpreadAngleAbsoluteFactor = 0.5f;

	public float projectileSpreadAngle = 12f;

	public Projectile projectilePrefab;

	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public ValueStackFloat projectileSpeed = new ValueStackFloat(16f);

	public float projectileShootHight = 1f;

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (projectilePrefab == null)
		{
			Debug.Log("projectile prefab not set.");
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		Vector3 forward = parameters.targetPosition - parameters.caster.transform.position;
		float num = Mathf.Lerp(projectileSpreadAngle, projectileSpreadAngle / (float)numberOfProjectiles.GetCurrentValue(), projectileSpreadAngleAbsoluteFactor);
		List<Projectile> list = new List<Projectile>();
		for (int i = 0; i < numberOfProjectiles.GetCurrentValue(); i++)
		{
			Quaternion quaternion = Quaternion.Euler(0f, num * ((float)i - (float)(numberOfProjectiles.GetCurrentValue() - 1) * 0.5f), 0f);
			Quaternion quaternion2 = Quaternion.LookRotation(forward);
			Projectile component = Object.Instantiate(projectilePrefab.gameObject, base.transform.position + Vector3.up * projectileShootHight, quaternion * quaternion2).GetComponent<Projectile>();
			component.transform.localScale *= projectileScale.GetCurrentValue();
			component.aoeDamage *= damageMultiplier.GetCurrentValue() * owner.damage.GetCurrentValue();
			component.hitDamage *= damageMultiplier.GetCurrentValue() * owner.damage.GetCurrentValue();
			component.targetSeekingRadius = projectileTargetSeekingRadius.GetCurrentValue();
			component.impactRadius *= Mathf.Sqrt(owner.area.GetCurrentValue());
			component.Initialize(owner, quaternion * forward.normalized * projectileSpeed.GetCurrentValue());
			list.Add(component);
		}
		List<IProjectileModification> list2 = new List<IProjectileModification>();
		GetComponentsInChildren(list2);
		foreach (IProjectileModification item in list2)
		{
			foreach (Projectile item2 in list)
			{
				item.OnInstantiateProjectile(item2);
			}
		}
		return base.Trigger(parameters);
	}
}
