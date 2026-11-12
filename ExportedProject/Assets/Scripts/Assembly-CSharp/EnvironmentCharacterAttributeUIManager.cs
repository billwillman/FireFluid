using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCharacterAttributeUIManager : MonoBehaviour
{
	private Dictionary<CharacterAttributes, GameObject> attributesUIs = new Dictionary<CharacterAttributes, GameObject>();

	public GameObject characterAttributeUIPrefab;

	public GameObject characterAttributeBossUIPrefab;

	public GameObject characterAttributeBoostedUIPrefab;

	private RectTransform canvasRect;

	private void Start()
	{
		canvasRect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		foreach (CharacterAttributes item in CharacterAttributes.all)
		{
			if (attributesUIs.ContainsKey(item) || item is CharacterAttributesPlayer || item is CharacterAttributesPassive)
			{
				continue;
			}
			BasicAI component = item.GetComponent<BasicAI>();
			bool flag = false;
			if ((bool)component && component.boss)
			{
				flag = true;
			}
			bool flag2 = false;
			if ((bool)component && component.boosted)
			{
				flag2 = true;
			}
			if (!item.health.IsFull() || flag)
			{
				GameObject gameObject = null;
				if (flag)
				{
					gameObject = Object.Instantiate(characterAttributeBossUIPrefab, base.transform);
				}
				else if (flag2)
				{
					gameObject = Object.Instantiate(characterAttributeBoostedUIPrefab, base.transform);
					SetCanvasPosToWorldPos componentInChildren = gameObject.GetComponentInChildren<SetCanvasPosToWorldPos>();
					componentInChildren.target = item.transform;
					componentInChildren.canvasRect = canvasRect;
				}
				else
				{
					gameObject = Object.Instantiate(characterAttributeUIPrefab, base.transform);
					SetCanvasPosToWorldPos componentInChildren2 = gameObject.GetComponentInChildren<SetCanvasPosToWorldPos>();
					componentInChildren2.target = item.transform;
					componentInChildren2.canvasRect = canvasRect;
				}
				DynamicCharacterAttributeUIElement[] componentsInChildren = gameObject.GetComponentsInChildren<DynamicCharacterAttributeUIElement>();
				foreach (DynamicCharacterAttributeUIElement obj in componentsInChildren)
				{
					obj.attributes = item;
					obj.HighlightChange();
				}
				attributesUIs.Add(item, gameObject);
			}
		}
		List<CharacterAttributes> list = new List<CharacterAttributes>();
		foreach (CharacterAttributes key in attributesUIs.Keys)
		{
			if (!CharacterAttributes.all.Contains(key) || key == null || key.health.IsExhausted())
			{
				Object.Destroy(attributesUIs[key]);
				list.Add(key);
			}
		}
		foreach (CharacterAttributes item2 in list)
		{
			attributesUIs.Remove(item2);
		}
	}
}
