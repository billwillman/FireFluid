using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAI : MonoBehaviour, IAbilityController
{
	protected CharacterAttributes attributes;

	protected bool hasTarget;

	protected bool hasVisionToTarget;

	protected CharacterAttributes target;

	protected float distanceToTarget = float.MaxValue;

	[Header("Movement")]
	public float movementAcceleration = 16f;

	public float movementInsideTargetRange = 1.9f;

	protected Vector3 movementDirection = Vector3.zero;

	private Rigidbody rigidBody;

	private Animator animator;

	protected NavMeshAgent agent;

	protected NavMeshPath path;

	protected Vector3[] corners = new Vector3[0];

	private AIState state = AIState.Move;

	[Header("Animation")]
	public float idleAnimationSpeed = 0.5f;

	public float walkAnimationSpeed = 0.7f;

	public float attackAnimationSpeed = 2f;

	public float secondaryActionAnimationSpeed = 2f;

	public float dyingAnimationSpeed = 0.5f;

	public TargetWatchMode targetWatchMode = TargetWatchMode.IdleAndCast;

	private bool abilityIsAnimating;

	[Header("Casting Abilities")]
	public float castRange = 2f;

	public float castCooldown = 1f;

	public float castDelay;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private Ability castAbility;

	private AbilityCastParameters abilityCastParameters = new AbilityCastParameters();

	private List<Ability> abilities = new List<Ability>();

	[Header("Mysc")]
	public bool boss;

	public bool boosted;

	[Header("Sound")]
	public string walkSound;

	private static readonly int WalkHash = Animator.StringToHash("Walk");

	private static readonly int IdleHash = Animator.StringToHash("Idle");

	private static readonly int DeathHash = Animator.StringToHash("Death");

	private static readonly int AttackHash = Animator.StringToHash("Attack");

	private static readonly int SecondaryActionHash = Animator.StringToHash("SecondaryAction");

	[HideInInspector]
	public bool positionIsDirty = true;

	public virtual void Start()
	{
		attributes = GetComponent<CharacterAttributes>();
		rigidBody = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();
		GetComponentsInChildren(abilities);
		abilityCastParameters.caster = attributes;
		agent = GetComponent<NavMeshAgent>();
		if ((bool)agent)
		{
			agent.updatePosition = false;
			agent.updateRotation = false;
			agent.updateUpAxis = false;
			path = new NavMeshPath();
		}
		if (boss)
		{
			Camera.main.GetComponent<CameraController>().Shake(2f);
		}
	}

	public float GetCastRange()
	{
		return castRange * base.transform.localScale.x;
	}

	public float GetInsideTargetRadius()
	{
		return movementInsideTargetRange * base.transform.localScale.x;
	}

	public CharacterAttributes GetAttributes()
	{
		return attributes;
	}

	public CharacterAttributes GetTarget()
	{
		return target;
	}

	public void PushAbilityCast(Ability ability)
	{
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (abilityIsAnimating || ability.IsChanneling())
			{
				return true;
			}
			if (ability.slot == AbilitySlot.Attack)
			{
				animator.Play(AttackHash);
			}
			else
			{
				animator.Play(SecondaryActionHash);
			}
			return true;
		}));
		bool animationFinished = false;
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (abilityIsAnimating || ability.IsChanneling())
			{
				return true;
			}
			animator.speed = ((ability.slot == AbilitySlot.Attack) ? attackAnimationSpeed : secondaryActionAnimationSpeed) * attributes.attackSpeed.GetCurrentValue();
			return animationFinished;
		}, blocking: false));
		float t = castDelay;
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			t -= Time.deltaTime * attributes.attackSpeed.GetCurrentValue();
			AimAtTarget();
			return t <= 0f;
		}));
		if (ability.IsChanneling())
		{
			bool castStarted = false;
			cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				if (!castStarted)
				{
					castStarted = true;
					ability.Trigger(abilityCastParameters);
					return false;
				}
				if (!attributes.mana.IsExhausted())
				{
					if (!abilityIsAnimating)
					{
						if (ability.slot == AbilitySlot.Attack)
						{
							animator.speed = attackAnimationSpeed * attributes.attackSpeed.GetCurrentValue();
							animator.Play(AttackHash);
						}
						else
						{
							animator.speed = secondaryActionAnimationSpeed * attributes.attackSpeed.GetCurrentValue();
							animator.Play(SecondaryActionHash);
						}
					}
					AimAtTarget();
					if (!ability.CanTrigger(abilityCastParameters))
					{
						ability.TriggerEndChanneling();
						return true;
					}
					ability.TriggerUpdateChanneling(abilityCastParameters);
					return false;
				}
				ability.TriggerEndChanneling();
				return true;
			}));
		}
		else
		{
			cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				ability.Trigger(abilityCastParameters);
				return true;
			}));
		}
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (abilityIsAnimating)
			{
				return true;
			}
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !animator.IsInTransition(0))
			{
				animator.Play(IdleHash);
				animator.speed = idleAnimationSpeed;
				animationFinished = true;
				return true;
			}
			return false;
		}, blocking: false));
		cinematicMachine.PushAction(new CinematicWait(castCooldown));
	}

	public virtual void UpdateTarget()
	{
	}

	private void AimAtTarget()
	{
		if (!(target == null) && !(castAbility == null))
		{
			if (!(castAbility is IEnemyAbility))
			{
				abilityCastParameters.targetPosition = Vector3.Lerp(abilityCastParameters.targetPosition, target.transform.position, Time.deltaTime * 8f);
			}
			bool flag = false;
			switch (state)
			{
			case AIState.Casting:
				flag = targetWatchMode == TargetWatchMode.Always || targetWatchMode == TargetWatchMode.Cast || targetWatchMode == TargetWatchMode.IdleAndCast;
				break;
			case AIState.Idle:
				flag = targetWatchMode == TargetWatchMode.Always || targetWatchMode == TargetWatchMode.IdleAndCast;
				break;
			}
			if (flag)
			{
				rigidBody.rotation = Quaternion.LookRotation(target.transform.position - base.transform.position);
			}
		}
	}

	protected bool OccasionalUpdate()
	{
		if (Time.frameCount % 2 == 0)
		{
			return (double)Random.value < 1.0 / (1.0 + (double)(distanceToTarget * 0.1f));
		}
		return false;
	}

	public virtual Vector3 GetMovementPathTarget()
	{
		return target.transform.position;
	}

	public virtual void UpdateMovementDirection()
	{
		if (hasTarget && (distanceToTarget > GetInsideTargetRadius() || !hasVisionToTarget))
		{
			if (agent == null || !agent.enabled)
			{
				movementDirection = GetMovementPathTarget() - base.transform.position;
				movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;
				return;
			}
			if (positionIsDirty)
			{
				agent.Warp(base.transform.position);
				positionIsDirty = false;
			}
			Vector3 position = base.transform.position;
			position.y = 0f;
			agent.nextPosition = position;
			if (OccasionalUpdate())
			{
				Vector3 movementPathTarget = GetMovementPathTarget();
				movementPathTarget.y = 0f;
				agent.CalculatePath(movementPathTarget, path);
				corners = path.corners;
			}
			if (corners.Length > 1)
			{
				Vector3 vector = corners[1];
				movementDirection = Vector3.Lerp(movementDirection, (vector - base.transform.position).normalized, Mathf.Clamp01(Time.deltaTime * 8f));
				movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;
			}
		}
		else if (Random.value < 0.001f)
		{
			if (Random.value < 0.2f)
			{
				movementDirection = Vector3.zero;
				return;
			}
			movementDirection = Random.onUnitSphere;
			movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;
		}
	}

	public void SetMovementDirection(Vector3 movementDirection)
	{
		this.movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;
	}

	private void FixedUpdate()
	{
		if (attributes.health.IsExhausted())
		{
			return;
		}
		Vector3 linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, movementDirection * attributes.movementSpeed.GetCurrentValue(), Mathf.Clamp01(Time.deltaTime * movementAcceleration));
		linearVelocity.y = rigidBody.linearVelocity.y;
		rigidBody.linearVelocity = linearVelocity;
		if (cinematicMachine.IsBusy())
		{
			if (target != null && castAbility != null && castAbility.IsChanneling())
			{
				abilityCastParameters.targetPosition = Vector3.Lerp(abilityCastParameters.targetPosition, target.transform.position, Time.deltaTime * 8f);
				if (targetWatchMode != TargetWatchMode.Never)
				{
					rigidBody.rotation = Quaternion.LookRotation(target.transform.position - base.transform.position);
				}
			}
			movementDirection = Vector3.zero;
			return;
		}
		UpdateTarget();
		UpdateMovementDirection();
		switch (state)
		{
		case AIState.Move:
			if (rigidBody.linearVelocity.sqrMagnitude > 0.01f)
			{
				rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(rigidBody.linearVelocity), Time.deltaTime * 8f);
			}
			break;
		case AIState.Casting:
			movementDirection = Vector3.zero;
			break;
		case AIState.Idle:
			break;
		}
	}

	public virtual void Update()
	{
		if (attributes.health.IsExhausted())
		{
			animator.speed = dyingAnimationSpeed;
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			if (animator.HasState(0, DeathHash))
			{
				animator.Play(DeathHash);
			}
			return;
		}
		abilityIsAnimating = false;
		foreach (Ability ability in abilities)
		{
			if (ability is IAnimatedAbility)
			{
				switch (((IAnimatedAbility)ability).Animate(animator))
				{
				case AnimationTakeOver.AnimationAndTransform:
					abilityIsAnimating = true;
					break;
				case AnimationTakeOver.Animation:
					abilityIsAnimating = true;
					break;
				}
			}
		}
		cinematicMachine.UpdateActions(Time.deltaTime);
		if (targetWatchMode == TargetWatchMode.Always)
		{
			rigidBody.rotation = Quaternion.LookRotation(target.transform.position - base.transform.position);
		}
		if (cinematicMachine.IsBusy() || ((bool)TimeManager.Get() && TimeManager.Get().paused.GetCurrentValue()))
		{
			return;
		}
		UpdateTarget();
		castAbility = null;
		bool flag = false;
		foreach (Ability ability2 in abilities)
		{
			if (ability2.CurrentlyBlocking())
			{
				flag = true;
				break;
			}
		}
		if (hasTarget && !flag)
		{
			if (OccasionalUpdate())
			{
				Vector3 direction = target.transform.position - base.transform.position;
				hasVisionToTarget = !Physics.Raycast(base.transform.position, direction, direction.magnitude, LayerHelper.LAYERMASK_ENVIRONMENT);
			}
			foreach (Ability ability3 in abilities)
			{
				if (ability3 is IEnemyAbility enemyAbility)
				{
					if ((!ability3.IsChanneling() || attributes.mana.IsFull()) && ability3.CanTrigger(abilityCastParameters) && enemyAbility.CastIntent())
					{
						abilityCastParameters.targetPosition = enemyAbility.GetPreferredTargetPosition();
						castAbility = ability3;
						break;
					}
				}
				else if (distanceToTarget < GetCastRange())
				{
					abilityCastParameters.targetPosition = target.transform.position;
					if ((!ability3.IsChanneling() || attributes.mana.IsFull()) && ability3.CanTrigger(abilityCastParameters) && hasVisionToTarget)
					{
						castAbility = ability3;
						break;
					}
				}
			}
			if ((bool)castAbility && !abilityIsAnimating)
			{
				state = AIState.Casting;
				PushAbilityCast(castAbility);
				return;
			}
		}
		float magnitude = rigidBody.linearVelocity.magnitude;
		switch (state)
		{
		case AIState.Idle:
			if (!abilityIsAnimating)
			{
				animator.Play(IdleHash);
				animator.speed = idleAnimationSpeed;
			}
			if ((double)magnitude > 0.1)
			{
				state = AIState.Move;
			}
			break;
		case AIState.Move:
			if (!abilityIsAnimating)
			{
				animator.Play(WalkHash);
				animator.speed = magnitude * walkAnimationSpeed / base.transform.localScale.x;
			}
			SoundEffectManager.Play(walkSound, base.transform.position, base.transform);
			if ((double)magnitude < 0.1)
			{
				state = AIState.Idle;
			}
			break;
		case AIState.Casting:
			state = AIState.Idle;
			break;
		}
	}

	public bool AttackIntention()
	{
		return state == AIState.Casting;
	}

	public bool SecondaryActionIntention()
	{
		return false;
	}

	public Vector3 GetAimDirection()
	{
		return target.transform.position - base.transform.position;
	}

	public Vector3 GetMovementIntention()
	{
		return target.transform.position - base.transform.position;
	}

	private void OnDrawGizmosSelected()
	{
		if ((bool)agent && agent.hasPath)
		{
			for (int i = 0; i < agent.path.corners.Length - 1; i++)
			{
				Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.green);
			}
		}
	}
}
