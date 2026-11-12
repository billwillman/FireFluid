using TMPro;
using UnityEngine;

public class CharacterSheetItemUI : MonoBehaviour
{
	public Item item;

	private TextMeshProUGUI text;

	private ToolTip toolTip;

	public void Build(Item item)
	{
		this.item = item;
		text = GetComponent<TextMeshProUGUI>();
		text.text = item.GetDisplayName(headline: false);
		toolTip = GetComponent<ToolTip>();
		toolTip.text = item.GetDescription();
	}
}
