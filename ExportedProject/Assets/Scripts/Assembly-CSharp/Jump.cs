using UnityEngine;
using UnityEngine.AI;

public class Jump : Ability, IAnimatedAbility
{
	public string animation = "FrontFlip";

	private float currentJumpTime;

	public float jumpDuration = 0.7f;

	public float jumpHeight = 6f;

	public float jumpRange = 16f;

	public ValueStackInt numberOfJumps = new ValueStackInt(1);

	public ValueStackBool teleport = new ValueStackBool(defaultValue: false);

	public GameObject teleportEffectPrefab;

	public float impactRadius = 1f;

	public float impactDamageMultiplier = 1f;

	public GameObject onHitPrefab;

	private Vector3 jumpStartPosition;

	private Vector3 jumpTargetPosition;

	private Rigidbody characterBody;

	private CollisionController collisionController;

	private CharacterController characterController;

	private RendererEffects rendererEffects;

	public override void Start()
	{
		base.Start();
		characterBody = GetComponentInParent<Rigidbody>();
		collisionController = GetComponentInParent<CollisionController>();
		characterController = GetComponentInParent<CharacterController>();
		rendererEffects = GetComponentInParent<RendererEffects>();
	}

	public override void Update()
	{
		base.Update();
		cooldownStacks = numberOfJumps.GetCurrentValue();
		bool num = IsJumping();
		if (num)
		{
			float num2 = 1f - currentJumpTime / jumpDuration;
			characterBody.linearVelocity = Vector3.zero;
			if (teleport.GetCurrentValue())
			{
				if (num2 < 0.4f)
				{
					characterBody.position = jumpStartPosition;
				}
				else if (num2 < 0.55f)
				{
					if ((bool)rendererEffects && rendererEffects.visible)
					{
						rendererEffects.visible = false;
						if ((bool)teleportEffectPrefab)
						{
							Object.Instantiate(teleportEffectPrefab, jumpStartPosition + Vector3.up * jumpHeight, Quaternion.identity);
						}
						SoundEffectManager.Play("ability.teleport");
					}
					characterBody.position = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, (num2 - 0.4f) / 0.15f);
				}
				else
				{
					if ((bool)rendererEffects && !rendererEffects.visible)
					{
						rendererEffects.visible = true;
						rendererEffects.PlayEffect(GetIdentifier(), Color.white * 8f, 0.2f, EffectMode.Solid);
					}
					characterBody.position = jumpTargetPosition;
				}
			}
			else
			{
				characterBody.position = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, num2);
			}
		}
		currentJumpTime -= Time.deltaTime;
		if (num && !IsJumping())
		{
			characterBody.position = jumpTargetPosition;
			collisionController?.enableCollision.PopAll("Jump");
			if (impactDamageMultiplier > 0.01f)
			{
				Damage.DoAoEDamage(base.transform.position, impactRadius * base.transform.localScale.x, impactDamageMultiplier * owner.damage.GetCurrentValue(), owner, base.gameObject.layer);
			}
			if (onHitPrefab != null)
			{
				GameObject obj = Object.Instantiate(onHitPrefab, base.transform.position, Quaternion.Euler(90f, 0f, 0f));
				Vector3 localScale = obj.transform.localScale * impactRadius;
				localScale.x *= base.transform.localScale.x;
				localScale.y *= base.transform.localScale.y;
				localScale.z *= base.transform.localScale.z;
				obj.transform.localScale = localScale;
			}
			ExhaustCooldown();
			OnEndJump();
		}
	}

	public override bool CurrentlyBlocking()
	{
		return IsJumping();
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (IsJumping())
		{
			return false;
		}
		if ((parameters.targetPosition - parameters.caster.transform.position).magnitude < 0.1f)
		{
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		currentJumpTime = jumpDuration;
		jumpStartPosition = characterBody.position;
		jumpTargetPosition = characterBody.position + (parameters.targetPosition - characterBody.position).normalized * jumpRange;
		if (teleport.GetCurrentValue())
		{
			if (NavMesh.SamplePosition(jumpTargetPosition, out var hit, (jumpTargetPosition - jumpStartPosition).magnitude * 1.5f, -1))
			{
				jumpTargetPosition = hit.position;
			}
		}
		else
		{
			int layerMask = 1 << LayerMask.NameToLayer("Environment");
			if (Physics.SphereCast(base.transform.position, 0.45f, jumpTargetPosition - jumpStartPosition, out var hitInfo, (jumpTargetPosition - jumpStartPosition).magnitude, layerMask))
			{
				jumpTargetPosition = hitInfo.point;
			}
		}
		collisionController?.enableCollision.Push("Jump", value: false);
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IJumpModification[] componentsInChildren = GetComponentsInChildren<IJumpModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnStartJump();
			}
		}
		return base.Trigger(parameters);
	}

	private bool IsJumping()
	{
		return currentJumpTime > 0f;
	}

	private void OnEndJump()
	{
		if (owner.damage.GetCurrentValue() > 0.0001f)
		{
			IJumpModification[] componentsInChildren = GetComponentsInChildren<IJumpModification>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEndJump();
			}
		}
	}

	public AnimationTakeOver Animate(Animator animator)
	{
		if (!IsJumping() || animation == "")
		{
			return AnimationTakeOver.None;
		}
		animator.SetLayerWeight(0, 1f);
		animator.SetLayerWeight(1, 0f);
		animator.speed = 0f;
		float num = 1f - currentJumpTime / jumpDuration;
		animator.Play(animation, 0, num);
		float p = 2f;
		if ((double)num > 0.5)
		{
			p = 3f;
		}
		float num2 = (1f - Mathf.Abs(Mathf.Pow(num * 2f - 1f, p))) * jumpHeight;
		animator.transform.localPosition = Vector3.up * num2;
		return AnimationTakeOver.AnimationAndTransform;
	}
}
