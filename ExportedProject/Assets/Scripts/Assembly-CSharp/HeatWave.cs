using UnityEngine;

public class HeatWave : Ability
{
	public ValueStackFloat intensity = new ValueStackFloat(1f);

	public ValueStackFloat radius = new ValueStackFloat(2f);

	private FluidEmitter fluidEmitter;

	public override void Start()
	{
		base.Start();
		fluidEmitter = GetComponentInChildren<FluidEmitter>(includeInactive: true);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		if (fluidEmitter == null)
		{
			return false;
		}
		fluidEmitter.gameObject.SetActive(value: true);
		foreach (Enemy item in Enemy.all)
		{
			Vector3 vector = item.transform.position - base.transform.position;
			float magnitude = vector.magnitude;
			fluidEmitter.transform.localScale = Vector3.one * radius.GetCurrentValue() * 0.25f;
			if (magnitude < radius.GetCurrentValue())
			{
				item.transform.position += vector / magnitude * Mathf.Clamp01(1f - magnitude / radius.GetCurrentValue()) * Time.deltaTime * intensity.GetCurrentValue();
			}
		}
		return base.Trigger(parameters);
	}

	public override void TriggerEndChanneling()
	{
		fluidEmitter.gameObject.SetActive(value: false);
		base.TriggerEndChanneling();
	}

	public override void Update()
	{
		base.Update();
		base.transform.rotation = Quaternion.identity;
	}
}
