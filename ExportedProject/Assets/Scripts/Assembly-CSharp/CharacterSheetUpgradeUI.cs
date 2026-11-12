using UnityEngine;
using UnityEngine.UI;

public class CharacterSheetUpgradeUI : MonoBehaviour
{
	public Upgrade upgrade;

	private Image image;

	private ToolTip toolTip;

	public void Build(Upgrade upgrade)
	{
		this.upgrade = upgrade;
		image = GetComponent<Image>();
		image.sprite = upgrade.icon;
		toolTip = GetComponent<ToolTip>();
		toolTip.text = "<b><color=#ddb888>" + upgrade.GetName() + "</color></b><br><br>" + Upgrade.GetDescriptionAll(upgrade.gameObject);
	}
}
