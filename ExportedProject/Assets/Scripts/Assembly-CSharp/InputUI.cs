using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputUI : MonoBehaviour
{
	public GameObject inputGraphicContainer;

	public List<InputUIElement> inputElementPrefabs = new List<InputUIElement>();

	private List<InputUIElement> inputElements = new List<InputUIElement>();

	private Canvas canvas;

	public bool showOnTop;

	private void Start()
	{
		foreach (InputUIElement inputElementPrefab in inputElementPrefabs)
		{
			GameObject gameObject = Object.Instantiate(inputElementPrefab.gameObject, inputGraphicContainer.transform);
			inputElements.Add(gameObject.GetComponent<InputUIElement>());
		}
		canvas = GetComponent<Canvas>();
	}

	private void LateUpdate()
	{
		bool flag = false;
		bool flag2 = false;
		foreach (InputUIElement inputElement in inputElements)
		{
			bool activeInHierarchy = inputElement.gameObject.activeInHierarchy;
			inputElement.gameObject.SetActive(inputElement.IsShowing());
			if (activeInHierarchy != inputElement.gameObject.activeInHierarchy)
			{
				flag = true;
			}
			if (inputElement.gameObject.activeInHierarchy)
			{
				flag2 = true;
			}
		}
		if (Settings.showInputUI)
		{
			canvas.enabled = flag2;
		}
		else
		{
			canvas.enabled = false;
		}
		if (flag)
		{
			LayoutGroup[] componentsInChildren = GetComponentsInChildren<LayoutGroup>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(componentsInChildren[i].GetComponent<RectTransform>());
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<LayoutGroup>().GetComponent<RectTransform>());
		}
		showOnTop = false;
		InteractionController component = Player.Get().GetComponent<InteractionController>();
		if (component != null && component.GetNearestInteractable() != null && component.GetNearestInteractable() is GrantMaskInteractable)
		{
			showOnTop = true;
		}
		RectTransform component2 = inputGraphicContainer.GetComponent<RectTransform>();
		int num = 16;
		if (CutSceneManager.Get().IsBusy())
		{
			inputGraphicContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 48f);
			num = 48;
		}
		else if (SceneManager.Get().IsInUnderworld())
		{
			inputGraphicContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 96f);
			num = 96;
		}
		else
		{
			num = 16;
		}
		if (showOnTop)
		{
			component2.anchorMin = new Vector2(0.5f, 1f);
			component2.anchorMax = new Vector2(0.5f, 1f);
			component2.pivot = new Vector2(0.5f, 1f);
			num = -num;
		}
		else
		{
			component2.anchorMin = new Vector2(0.5f, 0f);
			component2.anchorMax = new Vector2(0.5f, 0f);
			component2.pivot = new Vector2(0.5f, 0f);
		}
		component2.anchoredPosition = new Vector2(0f, num);
	}
}
