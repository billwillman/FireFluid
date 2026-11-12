using UnityEngine;
using UnityEngine.UI;

public class CharacterSheetUpgradesContainerUI : MonoBehaviour
{
	public CharacterSheetUpgradeUI characterSheetUpgradeUIPrefab;

	public bool dirty = true;

	private void Rebuild()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Upgrade allAppliedUpgrade in UpgradeManager.Get().GetAllAppliedUpgrades())
		{
			if (allAppliedUpgrade.name != "")
			{
				Object.Instantiate(characterSheetUpgradeUIPrefab, base.transform).GetComponent<CharacterSheetUpgradeUI>().Build(allAppliedUpgrade);
			}
		}
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
