using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabPageSwitcher : MonoBehaviour
{
	public Sprite activeTabSprite;

	public Sprite inactiveTabSprite;

	public List<TabPage> pages = new List<TabPage>();

	public string soundEffectId = "mysc.flick";

	private TabPage activeTab;

	private void OnEnable()
	{
		ActivateTabPage(pages[0]);
	}

	private void Start()
	{
		foreach (TabPage page in pages)
		{
			page.header.onClick.AddListener(delegate
			{
				ActivateTabPage(page);
			});
		}
	}

	private void Update()
	{
		if (InputManager.MenuNextPage == ButtonState.Down)
		{
			FlipPage();
		}
		if (InputManager.MenuPreviousPage == ButtonState.Down)
		{
			FlipPage(-1);
		}
	}

	private void FlipPage(int numberOfPages = 1)
	{
		int num = -1;
		for (int i = 0; i < pages.Count; i++)
		{
			if (pages[i] == activeTab)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			ActivateTabPage(pages[(num + pages.Count + numberOfPages) % pages.Count]);
		}
	}

	private void ActivateTabPage(TabPage page)
	{
		SoundEffectManager.Play(soundEffectId);
		foreach (TabPage page2 in pages)
		{
			page2.content.SetActive(value: false);
			page2.header.GetComponent<Image>().sprite = inactiveTabSprite;
		}
		page.content.SetActive(value: true);
		if (InputManager.inputMode == InputMode.Gamepad)
		{
			page.firstSelection?.Select();
		}
		page.header.GetComponent<Image>().sprite = activeTabSprite;
		activeTab = page;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}
}
