using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
	public bool showDoNotDistribute = true;

	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = "";
		TextMeshProUGUI textMeshProUGUI = text;
		textMeshProUGUI.text = textMeshProUGUI.text + "V" + BuildInformation.GetVersionString();
		if (showDoNotDistribute)
		{
			text.text += "<br><color=#884444>Pre Alpha Version.<br>Do Not Distribute.</color>";
		}
	}
}
