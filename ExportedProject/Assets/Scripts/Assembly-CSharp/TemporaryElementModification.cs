using UnityEngine;

public class TemporaryElementModification : MonoBehaviour
{
	public float duration = 3f;

	public float chanceToG;

	public float chanceToB = 1f;

	private CharacterAttributesPlayer attributes;

	private void Start()
	{
		attributes = GetComponentInParent<CharacterAttributesPlayer>();
		if (Mathf.Abs(chanceToG) > 0.001f)
		{
			attributes.chanceToG.value.Push("TemporaryElementModification", chanceToG, ValueModifierType.Add);
		}
		if (Mathf.Abs(chanceToB) > 0.001f)
		{
			attributes.chanceToB.value.Push("TemporaryElementModification", chanceToB, ValueModifierType.Add);
		}
	}

	private void Update()
	{
		duration -= Time.deltaTime * attributes.attackSpeed.GetCurrentValue();
		if (duration < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		attributes.chanceToG.value.PopAll("TemporaryElementModification");
		attributes.chanceToB.value.PopAll("TemporaryElementModification");
	}
}
