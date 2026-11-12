using UnityEngine;

public class MeleeAttack : Ability
{
	public float damageFactor = 1f;

	public float range = 2f;

	public float impactRadius = 2f;

	public float timeOfImpact = 0.4f;

	public float maxImpactDistance = 999f;

	protected float progress;

	protected bool attackTriggered;

	protected Vector3 targetPosition;

	public GameObject onHitPrefab;

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if ((parameters.targetPosition - base.transform.position).sqrMagnitude > range * range)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		attackTriggered = true;
		Vector3 vector = parameters.targetPosition - parameters.caster.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude > maxImpactDistance)
		{
			vector = vector / magnitude * maxImpactDistance;
		}
		targetPosition = parameters.caster.transform.position + vector;
		progress = 0f;
		return base.Trigger(parameters);
	}

	public override void Update()
	{
		if (attackTriggered)
		{
			CustomDebug.DrawSphere(targetPosition, impactRadius, Color.red);
			progress += Time.deltaTime * owner.attackSpeed.GetCurrentValue();
			if (progress > timeOfImpact)
			{
				attackTriggered = false;
				float radius = impactRadius * Mathf.Sqrt(owner.area.GetCurrentValue());
				Damage.DoAoEDamage(targetPosition, radius, owner.damage.GetCurrentValue() * damageFactor, owner, base.gameObject.layer);
				if (onHitPrefab != null)
				{
					ExplosionHelper.Spawn(onHitPrefab, targetPosition, radius, Player.Get().GetAttributes().GetElement());
				}
			}
		}
		base.Update();
	}
}
