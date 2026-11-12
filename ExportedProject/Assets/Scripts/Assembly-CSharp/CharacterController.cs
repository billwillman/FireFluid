using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour, IAbilityController
{
	public ValueStackBool enableControlls = new ValueStackBool(defaultValue: true);

	public ValueStackBool enableAttacks = new ValueStackBool(defaultValue: true);

	public ValueStackBool animate = new ValueStackBool(defaultValue: true);

	public Camera camera;

	public float movementSpeedWhileAttackingMultiplier = 0.5f;

	public float movementAcceleration = 16f;

	public CharacterAttributes attributes;

	private List<Ability> abilities = new List<Ability>();

	private bool abilityIsBlocking;

	private Rigidbody rigidBody;

	private Animator animator;

	protected Vector3 worldMoveDirection;

	protected Vector3 worldAimDirection;

	protected int intent_attack_state;

	protected int intent_secondary_state;

	private bool healthPreviouslyExhausted;

	private float deathAnimationTime;

	private Vector3 cameraOffset;

	private float cameraSize;

	private bool wonPreviously;

	private Vector3 positionLastFrame;

	private float footStepCooldown;

	private Vector2 currentVelocity;

	private float currentSpeed;

	private AnimationTakeOver animationTakeOver;

	private TextLabel feedbackLabel;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<CharacterAttributes>();
		cameraOffset = Camera.main.GetComponent<CameraController>().offset;
		cameraSize = Camera.main.orthographicSize;
		positionLastFrame = base.transform.position;
	}

	public bool AttackIntention()
	{
		if (intent_attack_state != 1)
		{
			return intent_attack_state == 2;
		}
		return true;
	}

	public bool SecondaryActionIntention()
	{
		if (intent_secondary_state != 1)
		{
			return intent_secondary_state == 2;
		}
		return true;
	}

	public Vector3 ScreenDirToWorldDir(Vector2 screenDir)
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		Vector3 pos = Camera.main.WorldToScreenPoint(position);
		pos.z = Camera.main.nearClipPlane;
		pos.x += screenDir.x;
		pos.y += screenDir.y;
		Ray ray = Camera.main.ScreenPointToRay(pos);
		float num = (0f - ray.origin.y) / ray.direction.y;
		Vector3 vector = ray.origin + ray.direction * num;
		Vector3 vector2 = default(Vector3);
		vector2.x = vector.x - position.x;
		vector2.y = 0f;
		vector2.z = vector.z - position.z;
		return vector2.normalized;
	}

	protected virtual void PullInputs()
	{
		bool flag = false;
		bool flag2 = false;
		if (!attributes.health.IsExhausted() && enableControlls.GetCurrentValue() && (TimeManager.Get() == null || !TimeManager.Get().paused.GetCurrentValue()) && !UILayer.AnyLayerOpen())
		{
			if (!attributes.health.IsExhausted())
			{
				worldMoveDirection = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized * InputManager.MoveVertical + Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized * InputManager.MoveHorizontal;
				worldMoveDirection.Normalize();
			}
			Vector2 screenDir = new Vector2(InputManager.AimHorizontal, InputManager.AimVertical);
			worldAimDirection = ScreenDirToWorldDir(screenDir);
			if (!flag2 && screenDir.magnitude < 0.1f && Settings.autoAttack && !attributes.mana.IsExhausted() && attributes.damage.GetCurrentValue() > 0.001f)
			{
				bool flag3 = false;
				foreach (Ability ability in abilities)
				{
					if (ability.slot == AbilitySlot.Attack && ability.CanSpendMana(indicateFailedSpendAttempt: false))
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					Enemy nearestInVision = Enemy.GetNearestInVision(base.transform.position + Vector3.up);
					if ((bool)nearestInVision)
					{
						Vector3 vector = nearestInVision.transform.position - base.transform.position;
						if (vector.magnitude < 24f)
						{
							worldAimDirection = vector;
							worldAimDirection.y = 0f;
							flag = true;
						}
					}
				}
			}
			if (!abilityIsBlocking && (double)screenDir.sqrMagnitude > 0.1 && enableAttacks.GetCurrentValue())
			{
				flag = true;
			}
			if (!abilityIsBlocking && enableAttacks.GetCurrentValue() && (InputManager.SecondaryAction == ButtonState.Down || InputManager.SecondaryAction == ButtonState.Pressed))
			{
				flag2 = true;
			}
		}
		else
		{
			worldAimDirection = Vector3.zero;
			worldMoveDirection = Vector3.zero;
		}
		if (flag2)
		{
			if (intent_secondary_state == 0 || intent_secondary_state == 3)
			{
				intent_secondary_state = 1;
			}
			else
			{
				intent_secondary_state = 2;
			}
		}
		else if (intent_secondary_state != 3)
		{
			intent_secondary_state = 3;
		}
		else
		{
			intent_secondary_state = 0;
		}
		if (flag)
		{
			if (intent_attack_state == 0 || intent_attack_state == 3)
			{
				intent_attack_state = 1;
			}
			else
			{
				intent_attack_state = 2;
			}
		}
		else if (intent_attack_state != 3)
		{
			intent_attack_state = 3;
		}
		else
		{
			intent_attack_state = 0;
		}
	}

	public void SetAnimationTakeOver(AnimationTakeOver animationTakeOver)
	{
		this.animationTakeOver |= animationTakeOver;
	}

	protected void UpdateAnimation()
	{
		if (!animate.GetCurrentValue())
		{
			return;
		}
		if (RunManager.Get().state == RunState.Won && Player.Get() == GetComponent<Player>())
		{
			CameraController component = Camera.main.GetComponent<CameraController>();
			if (!wonPreviously)
			{
				deathAnimationTime = 0f;
				component.Shake(0.2f);
				TimeManager.Get().Freeze(1f);
				SoundTrackTransitionMode transitionMode = SoundTrack.Get().transitionMode;
				SoundTrack.Get().transitionMode = SoundTrackTransitionMode.fadeOutFadeIn;
				SoundTrack.Get().SetSoundTrack(SoundTrack.Get().Wind);
				SoundTrack.Get().transitionMode = transitionMode;
				SoundEffectManager.Play("player.win", Player.Get().transform.position);
			}
			component.Shake(0.02f);
			deathAnimationTime = Mathf.Clamp01(deathAnimationTime + Time.unscaledDeltaTime * 0.1f);
			animator.Play("Death", 0, deathAnimationTime - Mathf.PerlinNoise1D(Time.time * 0.2f) * 0.5f);
			animator.SetLayerWeight(0, 1f);
			animator.SetLayerWeight(1, 0f);
			animator.speed = 0f;
			RendererEffects.Highlight(base.gameObject, "Win", new Color(4f, 2.4f, 0.4f, 1f), 9999999f, EffectMode.SlowFade);
			component.offset = Vector3.Lerp(component.offset, cameraOffset + Vector3.up * 4f, Time.deltaTime);
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSize * 0.7f, Time.deltaTime);
		}
		wonPreviously = RunManager.Get().state == RunState.Won;
		if (wonPreviously)
		{
			return;
		}
		if (attributes.health.IsExhausted())
		{
			CameraController component2 = Camera.main.GetComponent<CameraController>();
			if (!healthPreviouslyExhausted)
			{
				deathAnimationTime = 0f;
				component2.Shake(0.2f);
				TimeManager.Get().Freeze(1f);
				SoundTrackTransitionMode transitionMode2 = SoundTrack.Get().transitionMode;
				SoundTrack.Get().transitionMode = SoundTrackTransitionMode.cut;
				SoundTrack.Get().SetSoundTrack(SoundTrack.Get().Wind);
				SoundTrack.Get().transitionMode = transitionMode2;
				SoundEffectManager.Play("player.death", Player.Get().transform.position);
			}
			component2.Shake(0.02f);
			deathAnimationTime = Mathf.Lerp(deathAnimationTime, 1f, Time.deltaTime * 1f);
			animator.Play("Death", 0, deathAnimationTime - Mathf.PerlinNoise1D(Time.time) * 0.1f);
			animator.SetLayerWeight(0, 1f);
			animator.SetLayerWeight(1, 0f);
			animator.speed = 0f;
			RendererEffects.Highlight(base.gameObject, "Death", Color.red, 9999f);
			component2.offset = Vector3.Lerp(component2.offset, cameraOffset + Vector3.up * 4f, Time.deltaTime);
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSize * 0.7f, Time.deltaTime);
		}
		healthPreviouslyExhausted = attributes.health.IsExhausted();
		if (healthPreviouslyExhausted)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (Ability ability in abilities)
		{
			if (ability is IAnimatedAbility)
			{
				animationTakeOver |= ((IAnimatedAbility)ability).Animate(animator);
			}
		}
		switch (animationTakeOver)
		{
		case AnimationTakeOver.AnimationAndTransform:
			flag = true;
			flag2 = true;
			break;
		case AnimationTakeOver.Animation:
			flag2 = true;
			break;
		case AnimationTakeOver.Transform:
			flag = true;
			break;
		}
		animationTakeOver = AnimationTakeOver.None;
		if (!flag)
		{
			animator.transform.localRotation = Quaternion.identity;
			animator.transform.localPosition = Vector3.zero;
		}
		if (flag2)
		{
			return;
		}
		if (AttackIntention())
		{
			animator.Play("Walk");
			animator.speed = currentSpeed * 0.15f;
		}
		else if ((double)currentSpeed > 0.1)
		{
			animator.Play("Run");
			animator.speed = currentSpeed * 0.15f;
		}
		else
		{
			animator.Play("Idle");
			animator.speed = 1f;
		}
		footStepCooldown -= Time.deltaTime * currentSpeed;
		if ((double)currentSpeed > 0.1)
		{
			if (footStepCooldown < 0f)
			{
				SoundEffectManager.Play("player.footstep", base.transform.position, base.transform, spatial: false);
				footStepCooldown = 2.8f;
			}
		}
		else
		{
			footStepCooldown = 0f;
		}
		if (AttackIntention())
		{
			Vector2 normalized = new Vector2(Vector3.Dot(rigidBody.linearVelocity, base.transform.right), Vector3.Dot(rigidBody.linearVelocity, base.transform.forward)).normalized;
			animator.Play("Attack", 1);
			animator.SetLayerWeight(1, 0.9f);
			animator.SetFloat("WalkX", normalized.x);
			animator.SetFloat("WalkY", normalized.y);
			animator.speed = currentSpeed * 0.8f;
		}
		else
		{
			animator.SetLayerWeight(1, 0f);
		}
	}

	public void UpdateAbilities()
	{
		abilities.Clear();
		GetComponentsInChildren(abilities);
		bool flag = false;
		AbilityCastParameters abilityCastParameters = new AbilityCastParameters();
		abilityCastParameters.caster = attributes;
		foreach (Ability ability in abilities)
		{
			switch (ability.slot)
			{
			case AbilitySlot.Attack:
				flag = AttackIntention();
				abilityCastParameters.targetPosition = base.transform.position + GetAimDirection();
				break;
			case AbilitySlot.SecondaryAction:
				flag = SecondaryActionIntention();
				abilityCastParameters.targetPosition = base.transform.position + GetMovementIntention();
				break;
			case AbilitySlot.Passive:
				flag = RunManager.Get().state == RunState.Ongoing;
				abilityCastParameters.targetPosition = base.transform.position;
				break;
			}
			if (attributes.damage.GetCurrentValue() < 0.0001f)
			{
				if (!(ability is Dash) && !(ability is Shield) && !(ability is Jump) && !(ability is SummonTimeBubble))
				{
					flag = false;
				}
				if (ability.slot == AbilitySlot.Passive)
				{
					flag = false;
				}
			}
			if ((ability.slot == AbilitySlot.Attack && intent_attack_state == 1) || (ability.slot == AbilitySlot.SecondaryAction && intent_secondary_state == 1))
			{
				if (ability.CooldownIsExhausted())
				{
					ShowFeedbackLabel("<color=#ffffff>" + StringLocalization.Get("general_cooldownnotready") + "</color>");
				}
				else if (!ability.CanSpendMana(indicateFailedSpendAttempt: false))
				{
					ShowFeedbackLabel("<color=#5555ff>" + StringLocalization.Get("general_notenoughmana") + "</color>");
				}
			}
			if (flag && ability.CanTrigger(abilityCastParameters))
			{
				if (ability.IsChanneling() && ability.CurrentlyTriggering())
				{
					ability.TriggerUpdateChanneling(abilityCastParameters);
				}
				else
				{
					ability.Trigger(abilityCastParameters);
				}
			}
			else if ((!flag || !ability.CanTrigger(abilityCastParameters)) && ability.IsChanneling() && ability.CurrentlyTriggering())
			{
				ability.TriggerEndChanneling();
			}
		}
	}

	public void ShowFeedbackLabel(string text)
	{
		if (feedbackLabel != null)
		{
			Object.Destroy(feedbackLabel.gameObject);
		}
		feedbackLabel = TextParticleManager.Label(text, base.transform, Color.black, 8f);
		Object.Destroy(feedbackLabel.gameObject, 2f);
	}

	public List<Ability> GetAbilities()
	{
		return abilities;
	}

	private void Update()
	{
		if (camera == null)
		{
			camera = Camera.main;
		}
		if (Time.timeScale < 0.01f)
		{
			return;
		}
		abilityIsBlocking = false;
		foreach (Ability ability in abilities)
		{
			if (ability.CurrentlyBlocking())
			{
				abilityIsBlocking = true;
				break;
			}
		}
		currentVelocity = new Vector2(base.transform.position.x - positionLastFrame.x, base.transform.position.z - positionLastFrame.z);
		if (Time.deltaTime > 0.0001f)
		{
			currentVelocity /= Time.deltaTime;
			currentSpeed = currentVelocity.magnitude;
		}
		else
		{
			currentVelocity = Vector2.zero;
			currentSpeed = 0f;
		}
		PullInputs();
		UpdateAbilities();
		UpdateAnimation();
		positionLastFrame = base.transform.position;
	}

	public void OffsetPosition(Vector3 offset)
	{
		float magnitude = offset.magnitude;
		if (!(magnitude < 0.0001f))
		{
			Vector3 vector = offset / magnitude;
			if (!Physics.SphereCast(rigidBody.position - vector, 1f, vector, out var _, magnitude + 1f, LayerHelper.LAYERMASK_NONPASSABLEENVIRONMENT))
			{
				rigidBody.position += offset;
				rigidBody.linearVelocity += offset * Time.deltaTime;
				positionLastFrame += offset;
			}
		}
	}

	public Vector3 GetAimDirection()
	{
		return worldAimDirection;
	}

	public Vector3 GetMovementIntention()
	{
		return worldMoveDirection;
	}

	private void FixedUpdate()
	{
		Vector3 b = worldMoveDirection;
		if (AttackIntention())
		{
			b *= attributes.movementSpeed.GetCurrentValue() * movementSpeedWhileAttackingMultiplier;
		}
		else
		{
			b *= attributes.movementSpeed.GetCurrentValue();
		}
		Vector3 linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, b, Mathf.Clamp01(Time.deltaTime * movementAcceleration));
		linearVelocity.y = rigidBody.linearVelocity.y;
		rigidBody.linearVelocity = linearVelocity;
		linearVelocity.y = 0f;
		if (attributes.health.IsExhausted() || animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
		{
			rigidBody.rotation *= Quaternion.Euler(0f, Time.deltaTime * 24f, 0f);
		}
		else if (AttackIntention())
		{
			rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(worldAimDirection), Time.deltaTime * 8f);
		}
		else if (currentSpeed > 1f)
		{
			rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, Quaternion.LookRotation(new Vector3(currentVelocity.x, 0f, currentVelocity.y)), Time.deltaTime * 16f);
		}
	}
}
