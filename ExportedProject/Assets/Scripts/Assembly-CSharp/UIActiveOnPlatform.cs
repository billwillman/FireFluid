using UnityEngine;
using UnityEngine.UI;

public class UIActiveOnPlatform : MonoBehaviour
{
	protected Selectable selectable;

	public bool windows = true;

	public bool android = true;

	public bool nintendoSwitch = true;

	public virtual void Awake()
	{
		selectable = GetComponent<Selectable>();
		SetActive(windows);
	}

	protected void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
		if (!active)
		{
			if (selectable.navigation.selectOnUp != null && selectable.navigation.selectOnUp.navigation.selectOnDown == selectable)
			{
				Navigation navigation = selectable.navigation.selectOnUp.navigation;
				navigation.selectOnDown = selectable.navigation.selectOnDown;
				selectable.navigation.selectOnUp.navigation = navigation;
			}
			if (selectable.navigation.selectOnDown != null && selectable.navigation.selectOnDown.navigation.selectOnUp == selectable)
			{
				Navigation navigation2 = selectable.navigation.selectOnDown.navigation;
				navigation2.selectOnUp = selectable.navigation.selectOnUp;
				selectable.navigation.selectOnDown.navigation = navigation2;
			}
			if (selectable.navigation.selectOnLeft != null && selectable.navigation.selectOnLeft.navigation.selectOnRight == selectable)
			{
				Navigation navigation3 = selectable.navigation.selectOnLeft.navigation;
				navigation3.selectOnRight = selectable.navigation.selectOnRight;
				selectable.navigation.selectOnLeft.navigation = navigation3;
			}
			if (selectable.navigation.selectOnRight != null && selectable.navigation.selectOnRight.navigation.selectOnLeft == selectable)
			{
				Navigation navigation4 = selectable.navigation.selectOnRight.navigation;
				navigation4.selectOnLeft = selectable.navigation.selectOnLeft;
				selectable.navigation.selectOnRight.navigation = navigation4;
			}
		}
	}
}
