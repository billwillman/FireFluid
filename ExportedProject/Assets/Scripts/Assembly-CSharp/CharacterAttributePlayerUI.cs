using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributePlayerUI : MonoBehaviour
{
	public CharacterAttributes characterAttributes;

	[HideInInspector]
	public List<DynamicCharacterAttributeUIElement> elements;

	private void Awake()
	{
		GetComponentsInChildren(elements);
		foreach (DynamicCharacterAttributeUIElement element in elements)
		{
			element.attributes = characterAttributes;
		}
	}

	private void Update()
	{
		if (characterAttributes == null)
		{
			return;
		}
		if (characterAttributes.health.IsExhausted())
		{
			foreach (DynamicCharacterAttributeUIElement element in elements)
			{
				element.gameObject.SetActive(value: false);
			}
			return;
		}
		foreach (DynamicCharacterAttributeUIElement element2 in elements)
		{
			element2.gameObject.SetActive(value: true);
		}
	}
}
