using System;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindClaws : ClawAttack, IAnimatedAbility
{
	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public GameObject clawPrefab;

	private List<Claw> claws = new List<Claw>();

	public float spread = 2f;

	public float range = 4f;

	public float attackSpeedMultiplier = 2f;

	private float castingTime;

	private float angle;

	public Vector3 GetClawOffset(int index)
	{
		float f = angle;
		float num = range + (float)index * spread;
		return new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f)) * num;
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		castingTime = 0f;
		return base.Trigger(parameters);
	}

	public override bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		int num = Mathf.Min((int)(castingTime * 8f) + 1, numberOfClaws.GetCurrentValue());
		float num2 = attackSpeedMultiplier * owner.attackSpeed.GetCurrentValue();
		angle += num2 * 2f * MathF.PI * Time.deltaTime;
		SoundEffectManager.Play(triggerSound, base.transform.position);
		if (claws.Count < num)
		{
			Claw component = UnityEngine.Object.Instantiate(clawPrefab).GetComponent<Claw>();
			component.damage *= damageMultiplier.GetCurrentValue() * owner.damage.GetCurrentValue();
			component.transform.position = parameters.caster.transform.position + GetClawOffset(claws.Count);
			component.Initialize(owner);
			claws.Add(component);
		}
		int num3 = 0;
		foreach (Claw claw in claws)
		{
			claw.transform.position = parameters.caster.transform.position + GetClawOffset(num3);
			num3++;
		}
		castingTime += Time.deltaTime;
		return base.TriggerUpdateChanneling(parameters);
	}

	public override void TriggerEndChanneling()
	{
		foreach (Claw claw in claws)
		{
			claw.Terminate();
		}
		claws.Clear();
		base.TriggerEndChanneling();
	}

	public AnimationTakeOver Animate(Animator animator)
	{
		if (CurrentlyTriggering())
		{
			Vector3 clawOffset = GetClawOffset(0);
			float num = Mathf.Atan2(clawOffset.z, clawOffset.x);
			animator.transform.localRotation = Quaternion.Euler(0f, (0f - num) * 180f / MathF.PI - 90f, 0f);
			return AnimationTakeOver.Transform;
		}
		return AnimationTakeOver.None;
	}
}
