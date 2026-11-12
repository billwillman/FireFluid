using System.Collections.Generic;
using UnityEngine;

public class ChainLightningProjectile : MonoBehaviour
{
	[HideInInspector]
	public CharacterAttributes owner;

	public float damage = 1f;

	public LightningStrike lightningPrefab;

	private IDamageable currentTarget;

	public float chainCooldown = 0.5f;

	private float currentChainCooldown;

	public float chainJumpRadius = 8f;

	public int numberOfChainJumps = 4;

	private int currentNumberOfChainJumps;

	private static Collider[] colliders = new Collider[64];

	private LayerMask targetMask;

	private Queue<IDamageable> hits = new Queue<IDamageable>();

	public const string DAMAGE_SOURCE = "chainlightning";

	private void Start()
	{
		if (((1 << base.gameObject.layer) & LayerHelper.LAYERMASK_PLAYERATTACKS) > 0)
		{
			targetMask = LayerHelper.LAYERMASK_ENEMY | LayerHelper.LAYERMASK_ENVIRONMENTDESTRUCTABLE;
		}
		else
		{
			targetMask = LayerHelper.LAYERMASK_PLAYER | LayerHelper.LAYERMASK_PLAYERSUMMON;
		}
		currentChainCooldown = chainCooldown;
	}

	private void Update()
	{
		currentChainCooldown -= Time.deltaTime;
		if (!(currentChainCooldown < 0f))
		{
			return;
		}
		currentChainCooldown = chainCooldown;
		currentNumberOfChainJumps++;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, chainJumpRadius, colliders, targetMask.value);
		Transform transform = null;
		IDamageable item = null;
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IDamageable componentInParent = colliders[i].GetComponentInParent<IDamageable>();
			if (componentInParent != null && !hits.Contains(componentInParent))
			{
				float sqrMagnitude = (base.transform.position - colliders[i].gameObject.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					transform = colliders[i].gameObject.transform;
					item = componentInParent;
				}
			}
		}
		if (transform != null)
		{
			hits.Enqueue(item);
			if (hits.Count > 3)
			{
				hits.Dequeue();
			}
			LightningStrike component = Object.Instantiate(lightningPrefab, base.transform.position, Quaternion.identity).GetComponent<LightningStrike>();
			component.start = base.transform.position + Vector3.up;
			component.end = transform.transform.position + Vector3.up;
			currentTarget = item;
			base.transform.position = transform.transform.position;
		}
		else
		{
			currentTarget = null;
		}
		if (currentTarget != null)
		{
			currentTarget.ReceiveDamage(damage, owner, "chainlightning");
		}
		if (currentNumberOfChainJumps >= numberOfChainJumps)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
