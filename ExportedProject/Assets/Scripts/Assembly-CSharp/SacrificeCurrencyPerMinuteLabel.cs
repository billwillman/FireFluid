using TMPro;
using UnityEngine;

public class SacrificeCurrencyPerMinuteLabel : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = StringLocalization.Get("sacrifice_currencyperminute").Replace("{currency}", "<size=150%>" + Sacrifice.GetExtraCurrencyPerMinute() + "<sprite name=currency></size>");
	}
}
