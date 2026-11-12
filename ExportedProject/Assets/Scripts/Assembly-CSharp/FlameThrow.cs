using UnityEngine;

public class FlameThrow : Ability
{
	public ValueStackFloat flameSize = new ValueStackFloat(1f);

	public float velocity = 0.2f;

	public float reaction = 0.3f;

	public float temperature = 0.2f;

	private FluidEmitter fluidEmitter;

	private ParticleSystem particleSystem;

	public ValueStackBool enablePushBack = new ValueStackBool(defaultValue: false);

	public ValueStackBool enableLightning = new ValueStackBool(defaultValue: false);

	public LightningDamageHit lightningEffectPrefab;

	public GameObject lightningGraphic;

	private CooldownTracker<IDamageable> lightningCooldownTracker = new CooldownTracker<IDamageable>(1f);

	private Collider[] colliders = new Collider[128];

	public override void Start()
	{
		base.Start();
		fluidEmitter = GetComponentInChildren<FluidEmitter>();
		particleSystem = GetComponentInChildren<ParticleSystem>();
		fluidEmitter.reaction = Vector3.zero;
		fluidEmitter.temperature = 0f;
		fluidEmitter.velocityX = 0f;
		fluidEmitter.velocityY = 0f;
		if ((bool)particleSystem)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
	}

	public override void Update()
	{
		lightningCooldownTracker.Update(Time.deltaTime * owner.attackSpeed.GetCurrentValue());
		if (enableLightning.GetCurrentValue() && CurrentlyTriggering())
		{
			lightningGraphic.SetActive(value: true);
			lightningGraphic.transform.localScale = Vector3.one * flameSize.GetCurrentValue();
			if (!SoundEffectManager.IsPlaying("ability.electricity", base.transform) && Time.timeScale > 0.1f)
			{
				SoundEffectManager.Play("ability.electricity", base.transform.position, base.transform, spatial: false);
			}
		}
		else
		{
			SoundEffectManager.Choke("ability.electricity", base.transform);
			lightningGraphic.SetActive(value: false);
		}
		base.Update();
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (!fluidEmitter)
		{
			Debug.Log("Flame Throw is missing a fluid emitter.");
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		Vector3 vector = Player.Get().GetAttributes().GetElement();
		float num = flameSize.GetCurrentValue() * 2f;
		Vector3 vector2 = vector * reaction;
		fluidEmitter.reaction = vector2 * (0.9f + num * 0.025f);
		fluidEmitter.temperature = temperature * (0.6f + num * 0.4f);
		Vector3 normalized = (parameters.targetPosition - parameters.caster.transform.position).normalized;
		Vector2 vector3 = default(Vector2);
		vector3.x = Vector3.Dot(Vector3.right, normalized);
		vector3.y = Vector3.Dot(Vector3.forward, normalized);
		fluidEmitter.velocityX = vector3.x * velocity * (0.5f + num * 0.5f);
		fluidEmitter.velocityY = vector3.y * velocity * (0.5f + num * 0.5f);
		fluidEmitter.transform.localScale = Vector3.one * 2.5f * (0.8f + num * 0.2f);
		if ((bool)particleSystem)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = true;
			ParticleSystem.MainModule main = particleSystem.main;
			main.startSpeed = new ParticleSystem.MinMaxCurve(8f * (0.7f + num * 0.3f), 16f * (0.7f + num * 0.3f));
			main.startSize = new ParticleSystem.MinMaxCurve(1f * (0.8f + num * 0.2f), 2f * (0.8f + num * 0.2f));
			particleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial.SetVector("_ReactionScale", new Vector4(vector.x, vector.y, vector.z, 0f) * 0.05f);
		}
		bool currentValue = enablePushBack.GetCurrentValue();
		bool currentValue2 = enableLightning.GetCurrentValue();
		if (currentValue || currentValue2)
		{
			float num2 = 16f * flameSize.GetCurrentValue();
			int num3 = Physics.OverlapSphereNonAlloc(base.transform.position, num2, colliders, LayerHelper.LAYERMASK_ENEMYOBJECTS);
			float num4 = 8f;
			float num5 = 64f;
			float num6 = 8f * flameSize.GetCurrentValue();
			for (int i = 0; i < num3; i++)
			{
				Vector3 vector4 = colliders[i].gameObject.transform.position - base.transform.position;
				vector4.y = 0f;
				float magnitude = vector4.magnitude;
				if (!(Vector3.Dot(vector4 / magnitude, owner.transform.forward) > 0.8f))
				{
					continue;
				}
				if (currentValue)
				{
					Enemy componentInParent = colliders[i].GetComponentInParent<Enemy>();
					if ((bool)componentInParent)
					{
						componentInParent.transform.position += vector4 / (magnitude + 0.01f) * Mathf.Clamp01(1f - magnitude / num2) * Time.deltaTime * num4;
					}
					else
					{
						Projectile componentInParent2 = colliders[i].GetComponentInParent<Projectile>();
						if ((bool)componentInParent2)
						{
							componentInParent2.Push(vector4 / (magnitude + 0.01f) * Mathf.Clamp01(1f - magnitude / num2) * Time.deltaTime * num5);
						}
					}
				}
				if (currentValue2 && magnitude < num6)
				{
					IDamageable componentInParent3 = colliders[i].GetComponentInParent<IDamageable>();
					if (componentInParent3 != null && !lightningCooldownTracker.IsOnCooldown(componentInParent3))
					{
						Object.Instantiate(lightningEffectPrefab, colliders[i].transform.position, Quaternion.identity);
						componentInParent3.ReceiveDamage(owner.damage.GetCurrentValue() * 2f, owner, "flamelightning");
						lightningCooldownTracker.ExhaustCooldown(componentInParent3);
					}
				}
			}
		}
		return base.TriggerUpdateChanneling(parameters);
	}

	public override void TriggerEndChanneling()
	{
		fluidEmitter.reaction = Vector3.zero;
		fluidEmitter.temperature = 0f;
		fluidEmitter.velocityX = 0f;
		fluidEmitter.velocityY = 0f;
		if ((bool)particleSystem)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
		base.TriggerEndChanneling();
	}
}
