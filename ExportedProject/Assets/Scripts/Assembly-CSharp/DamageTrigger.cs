using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
	public CharacterAttributes owner;

	public float damage;

	public string damageSource = "damagetrigger";

	public Color highlightColor = Color.red;

	private void OnTriggerEnter(Collider other)
	{
		CharacterAttributes componentInParent = other.GetComponentInParent<CharacterAttributes>();
		if ((bool)componentInParent)
		{
			RendererEffects.Highlight(componentInParent.gameObject, damageSource, highlightColor, 0.075f);
			componentInParent.ReceiveDamage(damage, owner, damageSource);
		}
	}
}
