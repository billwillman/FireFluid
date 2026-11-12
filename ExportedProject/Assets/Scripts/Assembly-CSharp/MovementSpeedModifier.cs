using UnityEngine;

public class MovementSpeedModifier : MonoBehaviour
{
	private CharacterAttributes attributes;

	public float chanceToModify = 0.5f;

	public float modifyMult = 1f;

	public float modifyAdd = 1f;

	private void Awake()
	{
		if (Random.value < chanceToModify)
		{
			attributes = GetComponent<CharacterAttributes>();
			attributes.movementSpeed.value.Push("speedMod", modifyMult, ValueModifierType.Multiply);
			attributes.movementSpeed.value.Push("speedMod", modifyAdd, ValueModifierType.Add);
			GetComponent<BasicAI>().boosted = true;
		}
	}
}
