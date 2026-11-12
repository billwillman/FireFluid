using System.Collections.Generic;
using UnityEngine;

public class Burnable : Destructable
{
	private FluidEmitter emitter;

	public Vector3 element = new Vector3(1f, 0f, 0f);

	public float maxReaction = 0.5f;

	public float maxTemperature = 0.5f;

	public float maxNoiseForce = 0.5f;

	public float damageTillMinBurn = 50f;

	public float damageTillMaxBurn = 100f;

	private float damageTaken;

	public List<Transform> flameEffects = new List<Transform>();

	private List<Vector3> flameEffectScales = new List<Vector3>();

	public override void Start()
	{
		base.Start();
		emitter = GetComponentInChildren<FluidEmitter>();
		emitter.reaction = Vector3.zero;
		emitter.temperature = 0f;
		emitter.noiseForce = 0f;
		element = element.normalized;
		foreach (Transform flameEffect in flameEffects)
		{
			flameEffectScales.Add(flameEffect.localScale);
			flameEffect.localScale = Vector3.zero;
		}
	}

	private void Update()
	{
		float num = Mathf.Clamp01((damageTaken - damageTillMinBurn) / (damageTillMaxBurn - damageTillMinBurn));
		if (num > 0.01f)
		{
			num = num * 0.7f + 0.3f;
		}
		emitter.reaction = Vector3.Lerp(emitter.reaction, element * maxReaction * num, Time.deltaTime);
		emitter.temperature = Mathf.Lerp(emitter.temperature, maxTemperature * num, Time.deltaTime);
		emitter.noiseForce = Mathf.Lerp(emitter.noiseForce, maxNoiseForce * num, Time.deltaTime);
		int num2 = 0;
		foreach (Transform flameEffect in flameEffects)
		{
			flameEffect.localScale = Vector3.Lerp(flameEffect.localScale, flameEffectScales[num2] * num, Time.deltaTime);
			num2++;
		}
	}

	public override float ReceiveDamage(float dmg, CharacterAttributes attacker, string causeOfDamage)
	{
		float num = base.ReceiveDamage(dmg, attacker, causeOfDamage);
		damageTaken += num;
		return num;
	}
}
