using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldLocalization : MonoBehaviour
{
	public string stringId;

	private void Start()
	{
		SetText();
	}

	private void OnEnable()
	{
		SetText();
	}

	public void SetText()
	{
		Text component = GetComponent<Text>();
		if ((bool)component)
		{
			component.text = StringLocalization.Get(stringId);
			LayoutRebuilder.ForceRebuildLayoutImmediate(component.rectTransform);
		}
		TextMesh component2 = GetComponent<TextMesh>();
		if ((bool)component2)
		{
			component2.text = StringLocalization.Get(stringId);
			LayoutRebuilder.ForceRebuildLayoutImmediate(component.rectTransform);
		}
		TextMeshProUGUI component3 = GetComponent<TextMeshProUGUI>();
		if ((bool)component3)
		{
			component3.text = StringLocalization.Get(stringId);
			LayoutRebuilder.ForceRebuildLayoutImmediate(component3.rectTransform);
		}
	}

	public static void UpdateAll()
	{
		TextFieldLocalization[] array = (TextFieldLocalization[])Object.FindObjectsOfType(typeof(TextFieldLocalization));
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetText();
		}
	}
}
