using UnityEngine;
using UnityEngine.UI;

public class InputUIElementBookDenialReset : InputUIElement
{
	private BookUI book;

	public override void Start()
	{
		base.Start();
		actionText.text = StringLocalization.Get("bookofdenial_reset");
	}

	public override void Update()
	{
		base.Update();
		_ = book == null;
	}

	public void LateUpdate()
	{
		if (inputGraphic.gameObject.activeInHierarchy ^ inputGraphic.gameObject.activeInHierarchy)
		{
			LayoutGroup[] componentsInChildren = GetComponentsInChildren<LayoutGroup>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(componentsInChildren[i].GetComponent<RectTransform>());
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<LayoutGroup>().GetComponent<RectTransform>());
		}
	}

	public override bool IsShowing()
	{
		if ((bool)characterController && !characterController.enableControlls.GetCurrentValue())
		{
			return false;
		}
		if ((bool)DialogManager.Get() && DialogManager.Get().IsInDialog())
		{
			return false;
		}
		book = null;
		foreach (object value in UILayer.layers.Values)
		{
			if (value is BookUI && (bool)((BookUI)value).GetComponent<BookOfDenialUIInteraction>())
			{
				book = (BookUI)value;
			}
		}
		if (book != null)
		{
			return true;
		}
		return false;
	}
}
