using UnityEngine;

public class EarlyMeleeAttack : MeleeAttack
{
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
		targetPosition = parameters.targetPosition;
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
