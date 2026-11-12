using System.Collections.Generic;
using UnityEngine;

public class RayAttack : Ability
{
	public ValueStackInt numberOfRays = new ValueStackInt(1);

	public ValueStackFloat damageMultiplier = new ValueStackFloat(2f);

	public float spreadAngle = 10f;

	[Range(0f, 1f)]
	public float spreadAngleAbsoluteFactor = 0.5f;

	public float bendStrength = 0.5f;

	public ValueStackFloat bendRadius = new ValueStackFloat(0f);

	public ValueStackFloat range = new ValueStackFloat(12f);

	public float rayHeight = 1.5f;

	public ValueStackBool piercing = new ValueStackBool(defaultValue: false);

	public float secondsPerTick = 0.5f;

	private int triggerFrames;

	public GameObject rayPrefab;

	private List<RayInstance> rays = new List<RayInstance>();

	public override void Start()
	{
		base.Start();
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		triggerFrames = 0;
		return base.Trigger(parameters);
	}

	public override bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		triggerFrames++;
		return base.TriggerUpdateChanneling(parameters);
	}

	public override void TriggerEndChanneling()
	{
		base.TriggerEndChanneling();
	}

	public override void Update()
	{
		if (rays.Count != numberOfRays.GetCurrentValue())
		{
			foreach (RayInstance ray in rays)
			{
				Object.Destroy(ray.gameObject);
			}
			rays.Clear();
			for (int i = 0; i < numberOfRays.GetCurrentValue(); i++)
			{
				RayInstance component = Object.Instantiate(rayPrefab, base.transform).GetComponent<RayInstance>();
				component.rayAttack = this;
				rays.Add(component);
			}
		}
		if (CurrentlyTriggering() && triggerFrames > 0)
		{
			int count = rays.Count;
			float num = Mathf.Lerp(spreadAngle, spreadAngle / (float)count, spreadAngleAbsoluteFactor);
			for (int j = 0; j < count; j++)
			{
				float angle = 0f;
				if (count > 1)
				{
					angle = (0f - num * (float)(count - 1)) * 0.5f + (float)j * num;
				}
				Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * owner.transform.forward;
				rays[j].UpdateState(base.transform.position + Vector3.up * rayHeight, dir);
				rays[j].currentlyActive = true;
			}
			if (Time.timeScale > 0.1f)
			{
				SoundEffectManager.Play("ability.ray", base.transform.position, base.transform, spatial: false);
			}
		}
		else
		{
			foreach (RayInstance ray2 in rays)
			{
				ray2.currentlyActive = false;
			}
			SoundEffectManager.Choke("ability.ray", base.transform);
		}
		base.Update();
	}
}
