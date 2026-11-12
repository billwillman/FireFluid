using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
	private float currencyFreezeTime = 1.5f;

	private int currencyDisplayed;

	private int currencyAdded;

	private float currencyAddedTimer;

	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		if (PersistentDataComponent.GetDataInstance() != null)
		{
			currencyDisplayed = PersistentDataComponent.GetDataInstance().currency;
		}
	}

	private void FixedUpdate()
	{
		if (currencyDisplayed + currencyAdded != PersistentDataComponent.GetDataInstance().currency)
		{
			currencyAdded = PersistentDataComponent.GetDataInstance().currency - currencyDisplayed;
			currencyAddedTimer = currencyFreezeTime;
		}
		currencyAddedTimer -= Time.deltaTime;
		if (currencyAddedTimer < 0f)
		{
			int num = (int)((float)currencyAdded * 0.01f);
			if (currencyAdded > 0)
			{
				num++;
			}
			if (currencyAdded < 0)
			{
				num--;
			}
			currencyAdded -= num;
			currencyDisplayed += num;
		}
		text.text = "<color=#FFFF00>" + currencyDisplayed + "</color>";
		if (currencyAdded > 0)
		{
			TextMeshProUGUI textMeshProUGUI = text;
			textMeshProUGUI.text = textMeshProUGUI.text + "<color=#44FF44> +" + currencyAdded + "</color>";
		}
		if (currencyAdded < 0)
		{
			TextMeshProUGUI textMeshProUGUI2 = text;
			textMeshProUGUI2.text = textMeshProUGUI2.text + "<color=#FF8888> " + currencyAdded + "</color>";
		}
	}

	public bool IsCounting()
	{
		return currencyAdded > 0;
	}
}
