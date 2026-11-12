using UnityEngine;

public class ChangePosition : Ability, IAnimatedAbility, IEnemyAbility
{
	public bool fixedTravelRange;

	public float maxTravelRange = 8f;

	protected bool abilityTriggered;

	private Vector3 startPosition;

	protected Vector3 targetPosition;

	private CollisionController collisionController;

	private Rigidbody rigidBody;

	private BasicAI basicEnemy;

	public float movementSpeed = 16f;

	private ChangePositionAbilityState state;

	public float transitionTime = 0.5f;

	private float currentTransitionTime;

	private float abilityDuration;

	private float currentAbilityDuration;

	private float timeSinceTravelled = 99f;

	public string transitionAnimation = "Spawn";

	private const string VALUE_MODIFIER_ID = "ChangePosition";

	private Animator animator;

	private Vector3 animatorPosition;

	public ParticleSystem positionChangeParticles;

	public override void Start()
	{
		base.Start();
		rigidBody = GetComponentInParent<Rigidbody>();
		collisionController = owner.GetComponent<CollisionController>();
		animator = owner.GetComponentInChildren<Animator>();
		basicEnemy = owner.GetComponent<BasicAI>();
		animatorPosition = animator.transform.localPosition;
		if (collisionController == null)
		{
			collisionController = owner.gameObject.AddComponent<CollisionController>();
			collisionController.AutoFetchReferences();
			collisionController.mainEmitters.Clear();
		}
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		abilityTriggered = true;
		startPosition = owner.transform.position;
		targetPosition = parameters.targetPosition;
		if (fixedTravelRange || (startPosition - targetPosition).sqrMagnitude > maxTravelRange * maxTravelRange)
		{
			targetPosition = owner.transform.position + (parameters.targetPosition - owner.transform.position).normalized * maxTravelRange;
		}
		targetPosition = PositionProvider.Get().GetValidPositionNear(targetPosition);
		return base.Trigger(parameters);
	}

	public override bool CurrentlyBlocking()
	{
		return timeSinceTravelled < 1f;
	}

	public override void Update()
	{
		if (abilityTriggered)
		{
			CustomDebug.DrawSphere(targetPosition, 1f, Color.green);
			timeSinceTravelled = 0f;
			switch (state)
			{
			case ChangePositionAbilityState.ReadyToCast:
				state = ChangePositionAbilityState.TransitionIn;
				currentTransitionTime = transitionTime;
				break;
			case ChangePositionAbilityState.TransitionIn:
				currentTransitionTime -= Time.deltaTime;
				rigidBody.linearVelocity = Vector3.zero;
				rigidBody.position = startPosition;
				owner.transform.position = rigidBody.position;
				animator.transform.localPosition = animatorPosition + Mathf.Clamp01((transitionTime - currentTransitionTime) / transitionTime) * Vector3.down * 2f;
				if (currentTransitionTime < 0f)
				{
					currentAbilityDuration = 0f;
					abilityDuration = (targetPosition - owner.transform.position).magnitude / movementSpeed;
					collisionController.enableCollision.Push("ChangePosition", value: false, ValueModifierType.Multiply);
					if ((bool)positionChangeParticles)
					{
						ParticleSystem.EmissionModule emission = positionChangeParticles.emission;
						emission.enabled = true;
					}
					state = ChangePositionAbilityState.ChangingPosition;
				}
				break;
			case ChangePositionAbilityState.ChangingPosition:
			{
				currentAbilityDuration += Time.deltaTime;
				rigidBody.linearVelocity = Vector3.zero;
				float num = currentAbilityDuration / abilityDuration;
				if (num < 1f)
				{
					rigidBody.position = Vector3.Lerp(startPosition, targetPosition, num);
					owner.transform.position = rigidBody.position;
					break;
				}
				rigidBody.position = targetPosition;
				owner.transform.position = rigidBody.position;
				collisionController.enableCollision.PopAll("ChangePosition");
				if ((bool)positionChangeParticles)
				{
					ParticleSystem.EmissionModule emission2 = positionChangeParticles.emission;
					emission2.enabled = false;
				}
				state = ChangePositionAbilityState.TransitionOut;
				currentTransitionTime = transitionTime;
				break;
			}
			case ChangePositionAbilityState.TransitionOut:
				rigidBody.position = targetPosition;
				owner.transform.position = rigidBody.position;
				currentTransitionTime -= Time.deltaTime;
				animator.transform.localPosition = animatorPosition + Mathf.Clamp01(1f - (transitionTime - currentTransitionTime) / transitionTime) * Vector3.down * 2f;
				if (currentTransitionTime < 0f)
				{
					abilityTriggered = false;
					state = ChangePositionAbilityState.ReadyToCast;
				}
				break;
			}
		}
		else
		{
			timeSinceTravelled += Time.deltaTime;
		}
		base.Update();
	}

	public AnimationTakeOver Animate(Animator animator)
	{
		if (!abilityTriggered)
		{
			return AnimationTakeOver.None;
		}
		float num = 1f * (transitionTime - currentTransitionTime) / transitionTime;
		switch (state)
		{
		case ChangePositionAbilityState.TransitionIn:
			animator.speed = 0f;
			animator.Play(transitionAnimation, 0, 1f - num);
			break;
		case ChangePositionAbilityState.TransitionOut:
			animator.speed = 0f;
			animator.Play(transitionAnimation, 0, num);
			break;
		}
		return AnimationTakeOver.Animation;
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (state != ChangePositionAbilityState.ReadyToCast || abilityTriggered)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public bool CastIntent()
	{
		return true;
	}

	public Vector3 GetPreferredTargetPosition()
	{
		Vector3 position = Player.Get().transform.position;
		Vector2 vector = Random.insideUnitCircle * 16f;
		position.x += vector.x;
		position.y = 0f;
		position.z += vector.y;
		return PositionProvider.Get().GetValidPositionNear(position);
	}
}
