using UnityEngine;
using UnityEngine.UI;

public class CharacterSheetItemContainerUI : MonoBehaviour
{
	public GameObject headline;

	public CharacterSheetItemUI characterSheetItemUIPrefab;

	public bool dirty = true;

	private void Rebuild()
	{
		foreach (Transform item2 in base.transform)
		{
			Object.Destroy(item2.gameObject);
		}
		int num = 0;
		Item[] componentsInChildren = Player.Get().GetComponentsInChildren<Item>();
		foreach (Item item in componentsInChildren)
		{
			if (!item.GetIdentifier().Contains("potion"))
			{
				Object.Instantiate(characterSheetItemUIPrefab, base.transform).GetComponent<CharacterSheetItemUI>().Build(item);
				num++;
			}
		}
		headline.SetActive(num > 0);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent.GetComponent<RectTransform>());
		dirty = false;
	}

	private void OnEnable()
	{
		dirty = true;
	}

	private void LateUpdate()
	{
		if (dirty)
		{
			Rebuild();
		}
	}
}
