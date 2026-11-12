using UnityEngine;

public class Dash : Ability
{
	private DashState state;

	public ValueStackBool canRedirectDuringDash = new ValueStackBool(defaultValue: false);

	public ValueStackFloat dashDuration = new ValueStackFloat(0.1f);

	private FluidEmitter fluidEmitter;

	public ValueStackFloat dashSpeed = new ValueStackFloat(128f);

	private float currentDashTime;

	private Vector3 dashDirection = Vector3.zero;

	private bool casting;

	private Rigidbody characterBody;

	private IAbilityController abilityController;

	private ParticleSystem particleSystem;

	public ValueStackFloat movementSpeedMultiplierRunning = new ValueStackFloat(1f);

	public ValueStackBool canRun = new ValueStackBool(defaultValue: false);

	public float manaCostDash = 10f;

	public float manaCostRunning = 15f;

	public TrailRenderer trail;

	public override void Start()
	{
		base.Start();
		fluidEmitter = GetComponentInChildren<FluidEmitter>();
		characterBody = GetComponentInParent<Rigidbody>();
		abilityController = GetComponentInParent<IAbilityController>();
	}

	public bool IsDashing()
	{
		return state == DashState.Dashing;
	}

	public bool IsRunning()
	{
		return state == DashState.Running;
	}

	public override bool CanSpendMana(bool indicateFailedSpendAttempt)
	{
		if (owner == null || owner.mana.CanSpend(manaCostDash, indicateFailedSpendAttempt))
		{
			return true;
		}
		return false;
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (fluidEmitter == null)
		{
			Debug.Log("Dash ability is missing a fluid emitter");
			return false;
		}
		if (IsDashing())
		{
			return false;
		}
		if ((parameters.targetPosition - parameters.caster.transform.position).magnitude < 0.1f)
		{
			return false;
		}
		if (!IsRunning() && !parameters.caster.mana.CanSpend(manaCostDash, indicateFailedSpendAttempt: true))
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		casting = true;
		currentDashTime = dashDuration.GetCurrentValue();
		Vector3 vector = parameters.targetPosition - parameters.caster.transform.position;
		if (vector.magnitude > 0.1f)
		{
			dashDirection = vector;
		}
		return base.Trigger(parameters);
	}

	public override void TriggerEndChanneling()
	{
		casting = false;
		base.TriggerEndChanneling();
	}

	private void EnableEmitter(bool enable)
	{
		if (enable)
		{
			fluidEmitter.temperature = 0.2f;
			fluidEmitter.inheritEmitterVelocity = true;
		}
		else
		{
			fluidEmitter.temperature = 0f;
			fluidEmitter.inheritEmitterVelocity = false;
		}
	}

	public override void Update()
	{
		base.Update();
		currentDashTime -= Time.deltaTime;
		owner.movementSpeed.value.Pop(GetIdentifier());
		switch (state)
		{
		case DashState.Inactive:
			trail.time = Mathf.Lerp(trail.time, 0f, Time.deltaTime * 8f);
			trail.enabled = trail.time > 0.01f;
			if (casting)
			{
				state = DashState.Dashing;
				currentDashTime = dashDuration.GetCurrentValue();
				StartDashing();
			}
			break;
		case DashState.Dashing:
			if (currentDashTime < 0f)
			{
				state = DashState.Running;
				EndDashing();
				StartRunning();
				break;
			}
			if (canRedirectDuringDash.GetCurrentValue())
			{
				dashDirection = abilityController.GetMovementIntention();
			}
			characterBody.linearVelocity = dashDirection * dashSpeed.GetCurrentValue();
			break;
		case DashState.Running:
			if (!casting || !canRun.GetCurrentValue())
			{
				casting = false;
				state = DashState.Inactive;
				EndRunning();
			}
			else
			{
				owner.movementSpeed.value.Push(GetIdentifier(), movementSpeedMultiplierRunning.GetCurrentValue(), ValueModifierType.Multiply);
				owner.mana.Spend(manaCostRunning * Time.deltaTime, DynamicCharacterAttributeValueChangedType.gradual);
			}
			break;
		}
	}

	private void StartDashing()
	{
		EnableEmitter(enable: true);
		trail.time = 0.5f;
		trail.enabled = true;
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IDashModification[] componentsInChildren = GetComponentsInChildren<IDashModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnStartDashing();
			}
		}
		owner.mana.Spend(manaCostDash);
	}

	private void EndDashing()
	{
		characterBody.linearVelocity = Vector3.zero;
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IDashModification[] componentsInChildren = GetComponentsInChildren<IDashModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEndDashing();
			}
		}
	}

	private void StartRunning()
	{
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IDashModification[] componentsInChildren = GetComponentsInChildren<IDashModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnStartRunning();
			}
		}
	}

	private void EndRunning()
	{
		ExhaustCooldown();
		EnableEmitter(enable: false);
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IDashModification[] componentsInChildren = GetComponentsInChildren<IDashModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEndRunning();
			}
		}
	}
}
