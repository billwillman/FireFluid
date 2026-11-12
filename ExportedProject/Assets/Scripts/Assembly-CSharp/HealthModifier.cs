using UnityEngine;

public class HealthModifier : MonoBehaviour
{
	private CharacterAttributes attributes;

	public float chanceToModify = 0.5f;

	public float modifyMult = 1f;

	public float modifyAdd = 1f;

	public float sizeMult = 1.5f;

	private void Awake()
	{
		if (Random.value < chanceToModify)
		{
			attributes = GetComponent<CharacterAttributes>();
			attributes.health.value.Push("healthMod", modifyMult, ValueModifierType.Multiply);
			attributes.health.value.Push("healthMod", modifyAdd, ValueModifierType.Add);
			base.transform.localScale *= sizeMult;
			GetComponent<BasicAI>().boosted = true;
		}
	}
}
