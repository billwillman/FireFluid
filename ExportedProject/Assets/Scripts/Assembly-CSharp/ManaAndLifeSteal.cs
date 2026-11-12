using UnityEngine;

public class ManaAndLifeSteal : Ability
{
	public float manaDamageMultiplier = 1f;

	public float lifeStealDamageMultiplier;

	public float lifeStealHealAmount = 1f;

	private float lifeStealTickTimer = 0.5f;

	private float currentLifeStealTickTimer;

	private CharacterAttributes target;

	private GhostLine bondLine;

	public float breakRange = 12f;

	private float channelingTime;

	public override void Start()
	{
		base.Start();
		bondLine = base.gameObject.AddComponent<GhostLine>();
		bondLine.startColor = Color.Lerp(new Color(1f, 0.5f, 0.5f, 0f), new Color(0.5f, 0.5f, 1f, 0f), manaDamageMultiplier / (manaDamageMultiplier + lifeStealDamageMultiplier));
		bondLine.endColor = bondLine.startColor;
		bondLine.Show(show: false);
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (target != null && (owner.transform.position - target.transform.position).magnitude > breakRange)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		float num = float.MaxValue;
		target = null;
		foreach (CharacterAttributes item in CharacterAttributes.all)
		{
			if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, item.gameObject.layer))
			{
				float sqrMagnitude = (item.gameObject.transform.position - parameters.targetPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					target = item;
				}
			}
		}
		return base.Trigger(parameters);
	}

	public override bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		if (target != null && manaDamageMultiplier > 0.001f)
		{
			target.mana.Spend(parameters.caster.damage.GetCurrentValue() * manaDamageMultiplier * Time.deltaTime, DynamicCharacterAttributeValueChangedType.gradual);
		}
		if (target != null && lifeStealDamageMultiplier > 0.001f && currentLifeStealTickTimer < 0f && channelingTime > 0.5f)
		{
			currentLifeStealTickTimer = lifeStealTickTimer;
			float dmg = parameters.caster.damage.GetCurrentValue() * lifeStealDamageMultiplier * lifeStealTickTimer;
			float num = target.ReceiveDamage(dmg, owner, "lifesteal");
			owner.health.Fill(num * lifeStealHealAmount);
		}
		return base.TriggerUpdateChanneling(parameters);
	}

	public void SetBondPositions()
	{
		if (!(target == null))
		{
			bondLine.end = owner.transform.position + Vector3.up * 3f;
			bondLine.start = target.transform.position + Vector3.up;
		}
	}

	public override void Update()
	{
		currentLifeStealTickTimer -= Time.deltaTime;
		if (CurrentlyTriggering())
		{
			bondLine.Show();
			SetBondPositions();
			bondLine.endColor.a = Mathf.Lerp(bondLine.endColor.a, 1f, Time.deltaTime * 4f);
			channelingTime += Time.deltaTime;
		}
		else
		{
			if (bondLine.endColor.a < 0.01f)
			{
				bondLine.Show(show: false);
				bondLine.endColor.a = 0f;
			}
			else
			{
				SetBondPositions();
				bondLine.endColor.a = Mathf.Lerp(bondLine.endColor.a, 0f, Time.deltaTime * 4f);
			}
			channelingTime = 0f;
		}
		base.Update();
	}
}
