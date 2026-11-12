using TMPro;
using UnityEngine;

public class NumberOfDeniesLabel : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = PersistentDataComponent.GetDataInstance().GetNumberOfInactiveUpgrades() + "/" + PersistentDataComponent.GetDataInstance().GetMaxInactiveUpgrades();
	}
}
