using TMPro;
using UnityEngine;

public class CurrencyTossedLabel : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = StringLocalization.Get("wishingwell_currencytossed").Replace("{currency}", "<b>" + PersistentDataComponent.GetDataInstance().currencyTossedIntoWell) + "</b>";
	}
}
