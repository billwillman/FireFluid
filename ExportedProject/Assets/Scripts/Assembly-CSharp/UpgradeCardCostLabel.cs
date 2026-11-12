using TMPro;
using UnityEngine;

public class UpgradeCardCostLabel : MonoBehaviour
{
	public CostLabelType type;

	public Vector3 offsetToCard;

	private TextMeshProUGUI text;

	private UpgradeCardCollectionUI collectionUI;

	private RectTransform rectTransform;

	private void Start()
	{
		collectionUI = GetComponentInParent<UpgradeCardCollectionUI>();
		text = GetComponentInChildren<TextMeshProUGUI>();
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		string text = ((type != CostLabelType.LevelUp) ? (collectionUI.cursorSelected.upgrade.GetActivateDeactivateCost() + "<sprite name=currency>") : (collectionUI.cursorSelected.upgrade.GetLevelUpCost() + "<sprite name=currency>"));
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(collectionUI.cursorSelected.upgrade.GetIdentifier());
		if (type == CostLabelType.LevelUp)
		{
			if (collectionUI.cursorSelected.upgrade.GetMaxLevel() <= upgradeEntry.level)
			{
				text = "Max.";
			}
			else if (!PersistentDataComponent.GetDataInstance().CanSpendCurrency(collectionUI.cursorSelected.upgrade.GetLevelUpCost()))
			{
				text = "<color=#FF2222>" + text + "</color>";
			}
		}
		else if (!PersistentDataComponent.GetDataInstance().CanSpendCurrency(collectionUI.cursorSelected.upgrade.GetActivateDeactivateCost()))
		{
			text = "<color=#FF2222>" + text + "</color>";
		}
		this.text.text = text;
		RectTransform component = collectionUI.cursorSelected.GetComponent<RectTransform>();
		rectTransform.position = component.position;
		rectTransform.localPosition += offsetToCard;
	}
}
