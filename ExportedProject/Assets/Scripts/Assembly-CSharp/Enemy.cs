using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicAI
{
	public string id = "enemy";

	public static List<Enemy> all = new List<Enemy>();

	public string GetIdentifier()
	{
		return id;
	}

	public override void Start()
	{
		base.Start();
		all.Add(this);
	}

	public override void UpdateTarget()
	{
		base.UpdateTarget();
		distanceToTarget = float.MaxValue;
		target = Player.GetTarget(base.transform.position, target);
		hasTarget = target != null && !target.health.IsExhausted();
		if (hasTarget)
		{
			distanceToTarget = Vector3.ProjectOnPlane(target.transform.position - base.transform.position, Vector3.up).magnitude;
		}
	}

	public static List<Enemy> GetAllInArea(Vector3 position, float radius)
	{
		List<Enemy> list = new List<Enemy>();
		float num = radius * radius;
		foreach (Enemy item in all)
		{
			if ((item.transform.position - position).sqrMagnitude < num)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static void KillAll(CharacterAttributes attacker, string sourceOfDamage)
	{
		foreach (Enemy item in all)
		{
			item.GetComponent<IDamageable>()?.ReceiveDamage(1000000f, attacker, sourceOfDamage);
		}
	}

	public static Enemy GetNearest(Vector3 p)
	{
		Enemy result = null;
		float num = float.MaxValue;
		foreach (Enemy item in all)
		{
			float sqrMagnitude = (p - item.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = item;
			}
		}
		return result;
	}

	public static Enemy GetNearestInVision(Vector3 p)
	{
		Enemy result = null;
		float num = float.MaxValue;
		foreach (Enemy item in all)
		{
			Vector3 direction = item.transform.position - p;
			direction.y = 0f;
			float sqrMagnitude = direction.sqrMagnitude;
			if (sqrMagnitude < num && !Physics.Raycast(p, direction, Mathf.Sqrt(sqrMagnitude), LayerHelper.LAYERMASK_ENVIRONMENT))
			{
				num = sqrMagnitude;
				result = item;
			}
		}
		return result;
	}

	public virtual void OnDestroy()
	{
		all.Remove(this);
	}
}
