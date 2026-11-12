using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUseNumberSetting : MonoBehaviour
{
	private TextMeshProUGUI textMeshPro;

	private Text text;

	private void Start()
	{
		text = GetComponent<Text>();
		textMeshPro = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if ((bool)text)
		{
			text.enabled = Settings.showNumbers;
		}
		if ((bool)textMeshPro)
		{
			textMeshPro.enabled = Settings.showNumbers;
		}
	}
}
