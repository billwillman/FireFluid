using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
	public TextMeshProUGUI header;

	public Button optionButton;

	public void Start()
	{
		UILayer.Open(this);
	}

	public void OnDestroy()
	{
		UILayer.Close(this);
	}
}
