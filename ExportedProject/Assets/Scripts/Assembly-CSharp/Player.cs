using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static List<CharacterAttributes> decoys = new List<CharacterAttributes>();

	public static List<Player> all = new List<Player>();

	private CharacterAttributesPlayer attributes;

	private CharacterController characterController;

	private void Awake()
	{
		attributes = GetComponent<CharacterAttributesPlayer>();
		characterController = GetComponent<CharacterController>();
		all.Add(this);
	}

	private void Start()
	{
	}

	public void Update()
	{
	}

	public static Player Get()
	{
		if (all.Count == 0)
		{
			return null;
		}
		return all[0];
	}

	public static Player GetRandomPlayer()
	{
		if (all.Count == 0)
		{
			return null;
		}
		return all[Random.Range(0, all.Count)];
	}

	public static Player GetNearestPlayer(Vector3 worldPos)
	{
		return Get();
	}

	public static CharacterAttributes GetTarget(Vector3 position, CharacterAttributes previousTarget)
	{
		if (decoys.Count > 0)
		{
			if (previousTarget == null || (double)Random.value < 0.1)
			{
				float num = (Get().transform.position - position).sqrMagnitude;
				CharacterAttributes result = Get().GetAttributes();
				{
					foreach (CharacterAttributes decoy in decoys)
					{
						float sqrMagnitude = (decoy.transform.position - position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							result = decoy;
						}
					}
					return result;
				}
			}
			return previousTarget;
		}
		return Get().GetAttributes();
	}

	public CharacterAttributesPlayer GetAttributes()
	{
		return attributes;
	}

	public CharacterController GetCharacterController()
	{
		return characterController;
	}

	public void SetToPosition(Vector3 p)
	{
		SetToPosition(p, Quaternion.identity);
	}

	public void SetToPosition(Vector3 p, Quaternion q)
	{
		base.transform.position = p;
		Rigidbody component = GetComponent<Rigidbody>();
		component.position = p;
		component.rotation = q;
		component.linearVelocity = Vector3.zero;
		component.angularVelocity = Vector3.zero;
	}

	public void ResetToDefault()
	{
		List<Ability> list = new List<Ability>();
		GetComponentsInChildren(list);
		foreach (Ability item in list)
		{
			Object.Destroy(item.gameObject);
		}
		GetComponent<GameObjectSetSwitch>().ActivateSetEntry("masks", "impatienceandneglect");
	}

	private void OnDestroy()
	{
		all.Remove(this);
	}
}
