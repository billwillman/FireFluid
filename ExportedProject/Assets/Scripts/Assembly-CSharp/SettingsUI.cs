using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
	public Selectable selectAfterClose;

	private void Start()
	{
	}

	private void Update()
	{
		if (InputManager.Menu == ButtonState.Down || InputManager.MenuBack == ButtonState.Down)
		{
			Close();
		}
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		UILayer.Open(this);
		LayoutGroup[] componentsInChildren = GetComponentsInChildren<LayoutGroup>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(componentsInChildren[i].GetComponent<RectTransform>());
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		selectAfterClose?.Select();
		InputManager.ConsumeInputs();
		UILayer.Close(this);
	}
}
