using UnityEngine;

public class RingOfFire : Ability
{
	public ValueStackFloat size = new ValueStackFloat(8f);

	private FluidEmitter emitter;

	public float burnLength = 5f;

	public float currentBurnLength;

	private float currentSize;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		currentBurnLength = 0f;
		return base.Trigger(parameters);
	}

	public override void Start()
	{
		base.Start();
		emitter = GetComponentInChildren<FluidEmitter>();
	}

	public override void Update()
	{
		base.Update();
		currentBurnLength += Time.deltaTime * (0.5f + Player.Get().GetAttributes().attackSpeed.GetCurrentValue() * 0.5f);
		emitter.noiseForce = Mathf.Lerp(emitter.noiseForce, 0.2f + size.GetCurrentValue() * 0.001f, Time.deltaTime * 2f);
		emitter.reaction = Player.Get().GetAttributes().GetElement() * 0.22f;
		if (currentBurnLength < burnLength)
		{
			emitter.gameObject.SetActive(value: true);
			currentCoolDown = GetCooldown();
		}
		else
		{
			emitter.gameObject.SetActive(value: false);
			currentSize = 0f;
		}
		currentSize = Mathf.Lerp(currentSize, size.GetCurrentValue(), Time.deltaTime);
		emitter.transform.localScale = Vector3.one * currentSize;
	}
}
