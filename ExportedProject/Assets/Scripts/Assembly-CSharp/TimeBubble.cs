using System.Collections.Generic;
using UnityEngine;

public class TimeBubble : MonoBehaviour
{
	public Transform model;

	public float movementSpeedMultiplier = 0.5f;

	public float attackSpeedMultiplier = 0.5f;

	public float radius = 8f;

	public float lifeTime = 10f;

	private float currentLifeTime;

	public float damage;

	public bool stunOnEnter;

	public bool explodeOnExpire;

	private HashSet<Collider> inside = new HashSet<Collider>();

	private HashSet<Collider> insideThisFrame = new HashSet<Collider>();

	public const string VALUE_MODIFIER_ID = "timeBubble";

	private static Collider[] sphereOverlapResults = new Collider[128];

	private int sphereOverlapLayerMask;

	private Vector3 originalPosition;

	private Material m;

	public GameObject explosionPrefab;

	private void Start()
	{
		sphereOverlapLayerMask = LayerHelper.GetOpponentObjectsLayerMask(base.gameObject.layer);
		originalPosition = base.transform.position;
	}

	private void Update()
	{
		currentLifeTime += Time.deltaTime;
		float num = 0.5f;
		if (currentLifeTime < num)
		{
			model.localScale = Vector3.one * currentLifeTime / num * radius;
		}
		else
		{
			if (explodeOnExpire)
			{
				num = 1f;
				if (lifeTime - currentLifeTime < num)
				{
					float num2 = currentLifeTime - lifeTime + num;
					model.localScale = Vector3.one * (radius + num2 * 0.5f);
					model.transform.position = originalPosition + Vector3.left * Mathf.Sin(num2 * 64f) * num2 * 0.5f;
				}
			}
			else if (lifeTime - currentLifeTime < num)
			{
				model.localScale = Vector3.one * (lifeTime - currentLifeTime) / num * radius;
			}
			if (currentLifeTime < lifeTime - num)
			{
				model.localScale = Vector3.one * radius;
			}
		}
		UpdateInside();
		if (currentLifeTime > lifeTime)
		{
			if (explodeOnExpire)
			{
				SoundEffectManager.Play("ability.pop", base.transform.position);
				ExplosionHelper.Spawn(explosionPrefab, base.transform.position, radius, Vector4.one, Player.Get().GetAttributes(), base.gameObject.layer, damage);
			}
			Object.Destroy(base.gameObject);
		}
	}

	private void UpdateInside()
	{
		insideThisFrame.Clear();
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, radius, sphereOverlapResults, sphereOverlapLayerMask);
		for (int i = 0; i < num; i++)
		{
			insideThisFrame.Add(sphereOverlapResults[i]);
		}
		foreach (Collider item in insideThisFrame)
		{
			if (!inside.Contains(item))
			{
				ApplyEffect(item);
				inside.Add(item);
			}
		}
		foreach (Collider item2 in inside)
		{
			if (item2 != null && !insideThisFrame.Contains(item2))
			{
				RemoveEffect(item2);
			}
		}
		inside.Clear();
		foreach (Collider item3 in insideThisFrame)
		{
			inside.Add(item3);
		}
	}

	private void ApplyEffect(Collider collider)
	{
		CharacterAttributes componentInParent = collider.GetComponentInParent<CharacterAttributes>();
		if ((bool)componentInParent)
		{
			componentInParent.movementSpeed.value.Push("timeBubble", movementSpeedMultiplier, ValueModifierType.Multiply);
			componentInParent.attackSpeed.value.Push("timeBubble", attackSpeedMultiplier, ValueModifierType.Multiply);
			if (stunOnEnter)
			{
				StatusEffectManager.Apply(StatusEffectManager.stunned, componentInParent, Player.Get().GetAttributes());
			}
		}
		Rigidbody componentInParent2 = collider.GetComponentInParent<Rigidbody>();
		if ((bool)componentInParent2)
		{
			componentInParent2.linearVelocity *= attackSpeedMultiplier;
		}
	}

	private void RemoveEffect(Collider collider)
	{
		CharacterAttributes componentInParent = collider.GetComponentInParent<CharacterAttributes>();
		if ((bool)componentInParent)
		{
			componentInParent.movementSpeed.value.PopAll("timeBubble");
			componentInParent.attackSpeed.value.PopAll("timeBubble");
		}
		Rigidbody componentInParent2 = collider.GetComponentInParent<Rigidbody>();
		if ((bool)componentInParent2)
		{
			componentInParent2.linearVelocity /= attackSpeedMultiplier;
		}
	}

	private void OnDestroy()
	{
		foreach (Collider item in inside)
		{
			if ((bool)item)
			{
				RemoveEffect(item);
			}
		}
	}
}
