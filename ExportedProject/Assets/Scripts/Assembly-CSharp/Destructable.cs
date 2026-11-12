using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
	public float health = 10f;

	public string id = "destructable";

	public static List<Destructable> all = new List<Destructable>();

	public string GetIdentifier()
	{
		return id;
	}

	public virtual void Start()
	{
		all.Add(this);
	}

	public virtual void OnDestroy()
	{
		all.Remove(this);
	}

	public bool IsDestroyed()
	{
		return health <= 0f;
	}

	public virtual float ReceiveDamage(float dmg, CharacterAttributes attacker, string causeOfDamage)
	{
		health -= dmg;
		if (IsDestroyed())
		{
			IKillable[] componentsInChildren = GetComponentsInChildren<IKillable>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnKill();
			}
		}
		return dmg;
	}

	public static Destructable GetNearest(Vector3 p)
	{
		Destructable result = null;
		float num = float.MaxValue;
		foreach (Destructable item in all)
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
}
