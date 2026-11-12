using System.Collections.Generic;
using UnityEngine;

public class SpawnAtTarget : Ability
{
	public Vector3 offset;

	public Vector3 velocity;

	public GameObject spawnPrefab;

	public float scatterRadius;

	public float channelingCoolDown = 1f;

	private float currentChannelCooldown = -1f;

	public virtual GameObject Spawn(AbilityCastParameters parameters)
	{
		Vector3 targetPosition = parameters.targetPosition;
		Vector2 vector = Random.insideUnitCircle * scatterRadius;
		GameObject gameObject = Object.Instantiate(spawnPrefab, targetPosition + new Vector3(vector.x, 0f, vector.y), Quaternion.identity);
		Projectile component = gameObject.GetComponent<Projectile>();
		if ((bool)component)
		{
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
		return gameObject;
	}

	public override void DeductCooldown(float time)
	{
		base.DeductCooldown(time);
		currentChannelCooldown -= time;
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		if (!IsChanneling() || currentChannelCooldown < 0f)
		{
			Spawn(parameters);
			currentChannelCooldown = channelingCoolDown;
		}
		return base.Trigger(parameters);
	}

	public override bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		if (IsChanneling() && currentChannelCooldown < 0f)
		{
			Spawn(parameters);
			currentChannelCooldown = channelingCoolDown;
		}
		return base.TriggerUpdateChanneling(parameters);
	}

	public override void Update()
	{
		currentChannelCooldown -= Time.deltaTime;
		base.Update();
	}
}
