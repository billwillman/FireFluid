using UnityEngine;
using UnityEngine.UI;

public class InputUIElementBook : InputUIElement
{
	private BookUI book;

	public override void Update()
	{
		base.Update();
		if (!(book == null))
		{
			actionText.text = book.GetInteractionToolTip();
		}
	}

	public void LateUpdate()
	{
		bool activeInHierarchy = inputGraphic.gameObject.activeInHierarchy;
		if (!book.CanInteract())
		{
			inputGraphic.gameObject.SetActive(value: false);
		}
		else
		{
			inputGraphic.gameObject.SetActive(value: true);
		}
		if (activeInHierarchy ^ inputGraphic.gameObject.activeInHierarchy)
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
			if (value is BookUI)
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
