using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUIManager : MonoBehaviour
{
	public delegate void OptionSelected(int option);

	public OptionsUI optionsUIPrefab;

	private static OptionsUIManager instance;

	public static OptionsUIManager Get()
	{
		return instance;
	}

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Open(string header, List<string> options, OptionSelected optionSelected)
	{
		OptionsUI optionsUI = Object.Instantiate(optionsUIPrefab.gameObject).GetComponent<OptionsUI>();
		optionsUI.header.text = header;
		optionsUI.GetComponent<Canvas>().worldCamera = Camera.main;
		optionsUI.GetComponent<Canvas>().sortingLayerName = "UI";
		optionsUI.GetComponent<Canvas>().sortingOrder = 100;
		int num = 0;
		Button button = null;
		foreach (string option in options)
		{
			GameObject gameObject = Object.Instantiate(optionsUI.optionButton.gameObject, optionsUI.optionButton.transform.parent);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = option;
			Button component = gameObject.GetComponent<Button>();
			int o = num;
			component.onClick.AddListener(delegate
			{
				optionSelected(o);
				Object.Destroy(optionsUI.gameObject);
				InputManager.ConsumeInputs();
			});
			if (num == 0)
			{
				gameObject.AddComponent<UIGuaranteeSelection>();
				if (InputManager.inputMode == InputMode.Gamepad)
				{
					component.Select();
				}
			}
			num++;
			if (button != null)
			{
				Navigation navigation = button.navigation;
				navigation.selectOnDown = component;
				button.navigation = navigation;
				Navigation navigation2 = component.navigation;
				navigation2.selectOnUp = button;
				component.navigation = navigation2;
			}
			button = component;
		}
		optionsUI.optionButton.gameObject.SetActive(value: false);
	}

	private void Update()
	{
	}
}
