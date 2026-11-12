using UnityEngine;

public class RammingAttack : Ability, IAnimatedAbility
{
	public float damageMultiplier = 1f;

	public float movementSpeedMultiplier = 3f;

	private bool currentlyActive;

	public GameObject trailEffect;

	public RammingAttackState state;

	public float chargeTime = 1f;

	private float currentChargeTime;

	public float rammingTime = 5f;

	private float currentRammingTime;

	public float recoverTime = 1f;

	private float currentRecoverTime;

	public GameObject onPlayerHitPrefab;

	private ValueStack<float> movementSpeedModifier;

	private Collider collider;

	private BasicAI basicAI;

	private Rigidbody rigidbody;

	private bool targetRangeReached;

	private Vector3 movementDirection;

	private float keepMovementRange = 12f;

	public override void Start()
	{
		base.Start();
		collider = GetComponent<Collider>();
		collider.enabled = false;
		rigidbody = GetComponentInParent<Rigidbody>();
		basicAI = GetComponentInParent<BasicAI>();
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		currentlyActive = true;
		return true;
	}

	public AnimationTakeOver Animate(Animator animator)
	{
		if (state == RammingAttackState.Inactive)
		{
			return AnimationTakeOver.None;
		}
		switch (state)
		{
		case RammingAttackState.Charge:
			animator.Play("Charge", 0, (chargeTime - currentChargeTime) / chargeTime);
			animator.speed = 0f;
			break;
		case RammingAttackState.Ram:
			animator.Play("Run");
			animator.speed = rigidbody.linearVelocity.magnitude * 0.08f;
			break;
		case RammingAttackState.Recover:
			animator.Play("Idle");
			animator.speed = 1f;
			break;
		}
		return AnimationTakeOver.Animation;
	}

	public override void Update()
	{
		base.Update();
		if (!currentlyActive)
		{
			return;
		}
		switch (state)
		{
		case RammingAttackState.Inactive:
			currentChargeTime = chargeTime;
			state = RammingAttackState.Charge;
			owner.movementSpeed.value.PopAll(identifier);
			owner.movementSpeed.value.Push(identifier, 0f, ValueModifierType.Multiply);
			break;
		case RammingAttackState.Charge:
			if ((bool)basicAI && (bool)basicAI.GetTarget() && (bool)rigidbody)
			{
				rigidbody.rotation = Quaternion.LookRotation(basicAI.GetTarget().transform.position - owner.transform.position);
			}
			currentChargeTime -= Time.deltaTime;
			if (currentChargeTime < 0f)
			{
				currentRammingTime = rammingTime;
				state = RammingAttackState.Ram;
				owner.movementSpeed.value.PopAll(identifier);
				owner.movementSpeed.value.Push(identifier, movementSpeedMultiplier, ValueModifierType.Multiply);
				trailEffect?.SetActive(value: true);
				collider.enabled = true;
				if ((bool)basicAI)
				{
					movementDirection = basicAI.GetTarget().transform.position - base.transform.position;
				}
				targetRangeReached = false;
			}
			break;
		case RammingAttackState.Ram:
			currentRammingTime -= Time.deltaTime;
			if (currentRammingTime < 0f)
			{
				currentRecoverTime = recoverTime;
				state = RammingAttackState.Recover;
				owner.movementSpeed.value.PopAll(identifier);
				owner.movementSpeed.value.Push(identifier, 0f, ValueModifierType.Multiply);
				trailEffect?.SetActive(value: false);
				collider.enabled = false;
			}
			break;
		case RammingAttackState.Recover:
			currentRecoverTime -= Time.deltaTime;
			if (currentRecoverTime < 0f)
			{
				state = RammingAttackState.Inactive;
				currentlyActive = false;
				owner.movementSpeed.value.PopAll(identifier);
				ExhaustCooldown();
			}
			break;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)basicAI && state == RammingAttackState.Ram)
		{
			if ((basicAI.GetTarget().transform.position - base.transform.position).sqrMagnitude < keepMovementRange * keepMovementRange)
			{
				targetRangeReached = true;
				basicAI.SetMovementDirection(movementDirection);
			}
			else if (basicAI.GetTarget() != null)
			{
				movementDirection = basicAI.GetTarget().transform.position - base.transform.position;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		CharacterAttributes componentInParent = other.GetComponentInParent<CharacterAttributes>();
		if (componentInParent == owner)
		{
			return;
		}
		if (targetRangeReached && ((1 << other.gameObject.layer) & LayerHelper.LAYERMASK_NONPASSABLEENVIRONMENT) > 0)
		{
			Camera.main.GetComponent<CameraController>().Shake(0.2f);
			currentRammingTime = -1f;
			return;
		}
		float num = 4f;
		float num2 = 8f;
		float num3 = 2f;
		Vector3 normalized = (other.transform.position - base.transform.position).normalized;
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent is CharacterAttributesPlayer)
		{
			float num4 = componentInParent.ReceiveDamage(owner.damage.GetCurrentValue() * damageMultiplier, owner, GetIdentifier());
			currentRammingTime = -1f;
			num2 *= 2f;
			num *= 1f;
			num3 *= 0.5f;
			if (num4 > 0.1f)
			{
				Camera.main.GetComponent<CameraController>().Shake(0.2f);
			}
			else
			{
				componentInParent = owner;
				normalized *= -1f;
			}
		}
		if (!componentInParent.GetComponent<KnockUp>())
		{
			if (componentInParent is CharacterAttributesPlayer)
			{
				ExplosionHelper.Spawn(onPlayerHitPrefab, componentInParent.transform.position, 2f, Vector4.zero);
			}
			KnockUp knockUp = componentInParent.gameObject.AddComponent<KnockUp>();
			knockUp.height = num;
			knockUp.offset = normalized * num2;
			knockUp.speed = num3;
		}
	}
}
