using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public bool endOnGroundImpact;

	public float lifeTime = 2f;

	public CharacterAttributes owner;

	public ProjectileType type = ProjectileType.AoE;

	public float impactRadius = 1f;

	public float aoeDamage = 1f;

	public float hitDamage = 0.5f;

	public GameObject onHitPrefab;

	public Vector4 element = new Vector4(1f, 0f, 0f, 0f);

	[HideInInspector]
	public float targetSeekingRadius;

	private LayerMask targetSeekingMask;

	public float turnSpeed = 1f;

	protected Rigidbody rigidbody;

	private HashSet<IDamageable> hits = new HashSet<IDamageable>();

	private static Collider[] colliders = new Collider[64];

	public virtual void Update()
	{
		lifeTime -= Time.deltaTime;
		if ((bool)rigidbody && (double)rigidbody.linearVelocity.sqrMagnitude > 0.0001)
		{
			base.transform.rotation = Quaternion.LookRotation(rigidbody.linearVelocity);
		}
		if (lifeTime < 0f)
		{
			End();
		}
		if (endOnGroundImpact && base.transform.position.y < 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			End();
		}
		UpdateTargetSeeking();
	}

	public virtual void UpdateTargetSeeking()
	{
		rigidbody.linearVelocity = GetTargetSeekingVelocity(rigidbody.linearVelocity);
	}

	protected Vector3 GetTargetSeekingVelocity(Vector3 velocity)
	{
		if (targetSeekingRadius > 0.1f)
		{
			Transform transform = null;
			float num = float.MaxValue;
			int num2 = Physics.OverlapSphereNonAlloc(base.transform.position, targetSeekingRadius, colliders, targetSeekingMask.value);
			for (int i = 0; i < num2; i++)
			{
				IDamageable componentInParent = colliders[i].GetComponentInParent<IDamageable>();
				if (componentInParent != null && (type != ProjectileType.Piercing || !hits.Contains(componentInParent)))
				{
					float sqrMagnitude = (base.transform.position - colliders[i].gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						transform = colliders[i].gameObject.transform;
					}
				}
			}
			if (transform != null)
			{
				Vector2 vector = new Vector2(transform.position.x - base.transform.position.x, transform.position.z - base.transform.position.z);
				float num3 = Mathf.Atan2(vector.y, vector.x);
				float num4 = Mathf.Atan2(velocity.z, velocity.x);
				if (num4 - num3 > MathF.PI)
				{
					num4 -= MathF.PI * 2f;
				}
				if (num4 - num3 < -MathF.PI)
				{
					num4 += MathF.PI * 2f;
				}
				float num5 = ((!(num4 - num3 < 0f)) ? (MathF.PI * Time.deltaTime) : (-MathF.PI * Time.deltaTime));
				num5 *= turnSpeed;
				if (Mathf.Abs(num3 - num4) < Mathf.Abs(num5))
				{
					return new Vector3(vector.x, 0f, vector.y).normalized * velocity.magnitude;
				}
				return Quaternion.Euler(0f, num5 * 57.29578f, 0f) * velocity;
			}
		}
		return velocity;
	}

	public virtual void Initialize(CharacterAttributes owner, Vector3 velocity)
	{
		this.owner = owner;
		rigidbody = GetComponent<Rigidbody>();
		if ((bool)rigidbody)
		{
			rigidbody.linearVelocity = velocity;
		}
		ElementHelper.InfuseElement(this, Player.Get().GetAttributes().GetElement());
		targetSeekingMask = LayerHelper.GetAttackTargetLayerMask(base.gameObject.layer);
	}

	private void OnCollisionEnter(Collision collision)
	{
		IDamageable componentInParent = collision.gameObject.GetComponentInParent<IDamageable>();
		OnHit(componentInParent);
	}

	public void OnTriggerEnter(Collider other)
	{
		IDamageable componentInParent = other.gameObject.GetComponentInParent<IDamageable>();
		OnHit(componentInParent);
	}

	private void OnHit(IDamageable damageable)
	{
		switch (type)
		{
		case ProjectileType.SingleTarget:
			damageable?.ReceiveDamage(hitDamage, owner, "projectile");
			End();
			break;
		case ProjectileType.Piercing:
			if (damageable != null && !hits.Contains(damageable))
			{
				damageable.ReceiveDamage(hitDamage, owner, "projectile");
				hits.Add(damageable);
			}
			break;
		case ProjectileType.AoE:
			End();
			break;
		}
	}

	public virtual void End()
	{
		ExplosionHelper.Spawn(onHitPrefab, base.transform.position, impactRadius * base.transform.localScale.x, element, owner, base.gameObject.layer, aoeDamage);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Push(Vector3 v)
	{
		if ((bool)rigidbody)
		{
			rigidbody.linearVelocity += v;
		}
	}
}
