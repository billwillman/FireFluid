using TMPro;
using UnityEngine;

public class HarvestText : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		string text = "<u>" + StringLocalization.Get("tree_harvested") + ":</u><br>";
		if (PersistentDataComponent.GetDataInstance().treeModifications.Count == 0)
		{
			text = text + "<color=#888888>" + StringLocalization.Get("tree_nothingharvested") + "</color>";
		}
		else
		{
			foreach (AttributeModification treeModification in PersistentDataComponent.GetDataInstance().treeModifications)
			{
				text = text + treeModification.GetDescription() + "<br>";
			}
		}
		this.text.text = text;
	}
}
