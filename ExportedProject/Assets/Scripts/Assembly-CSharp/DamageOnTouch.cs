using UnityEngine;

public class DamageOnTouch : Ability
{
	public float damageMultiplier = 1f;

	private void OnTriggerEnter(Collider collider)
	{
		CharacterAttributes componentInParent = collider.gameObject.GetComponentInParent<CharacterAttributes>();
		if ((bool)componentInParent)
		{
			_ = collider.transform.position - base.transform.position;
			componentInParent.ReceiveDamage(owner.damage.GetCurrentValue() * damageMultiplier, owner, "damageOnTouch");
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		CharacterAttributes componentInParent = col.gameObject.GetComponentInParent<CharacterAttributes>();
		if ((bool)componentInParent)
		{
			componentInParent.ReceiveDamage(owner.damage.GetCurrentValue() * damageMultiplier, owner, "damageOnTouch");
		}
	}
}
