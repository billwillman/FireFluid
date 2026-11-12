using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
	public float damage = 1f;

	public float sphereCastRadius = 0.2f;

	private Vector3 previousPosition;

	public CharacterAttributes owner;

	private List<GameObject> newlyHitObjects = new List<GameObject>();

	private List<GameObject> hitObjects = new List<GameObject>();

	public float lifeTimeAfterTermination = 2f;

	private bool terminated;

	public Vector4 element = new Vector4(1f, 0f, 0f, 0f);

	public const string CAUSE_OF_DAMAGE = "Claw";

	public void Initialize(CharacterAttributes owner)
	{
		this.owner = owner;
		previousPosition = base.transform.position;
		element = ElementHelper.InfuseElement(element, Player.Get().GetAttributes().GetElement());
		ElementHelper.SetElement(base.gameObject, element * 0.2f);
	}

	public void Terminate()
	{
		terminated = true;
	}

	public bool IsTerminated()
	{
		return terminated;
	}

	private void OnEnable()
	{
		previousPosition = base.transform.position;
	}

	private void Update()
	{
		Vector3 direction = base.transform.position - previousPosition;
		float magnitude = direction.magnitude;
		newlyHitObjects.Clear();
		if (!terminated)
		{
			RaycastHit[] array = Physics.SphereCastAll(previousPosition, sphereCastRadius, direction, magnitude);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, raycastHit.transform.gameObject.layer))
				{
					newlyHitObjects.Add(raycastHit.transform.gameObject);
					if (!hitObjects.Contains(raycastHit.transform.gameObject))
					{
						hitObjects.Add(raycastHit.transform.gameObject);
						raycastHit.transform.GetComponentInParent<IDamageable>()?.ReceiveDamage(damage, owner, "Claw");
					}
				}
			}
			for (int num = hitObjects.Count - 1; num >= 0; num--)
			{
				if (!newlyHitObjects.Contains(hitObjects[num]))
				{
					hitObjects.RemoveAt(num);
				}
			}
		}
		else
		{
			lifeTimeAfterTermination -= Time.deltaTime;
			if (lifeTimeAfterTermination < 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		previousPosition = base.transform.position;
	}
}
