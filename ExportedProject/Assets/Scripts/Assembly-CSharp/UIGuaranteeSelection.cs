using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGuaranteeSelection : MonoBehaviour
{
	public bool onlyGamepad = true;

	private Selectable selectable;

	private void Start()
	{
		selectable = GetComponent<Selectable>();
	}

	private void Update()
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if ((!onlyGamepad || InputManager.inputMode == InputMode.Gamepad) && (currentSelectedGameObject == null || !currentSelectedGameObject.activeInHierarchy))
		{
			selectable.Select();
		}
	}
}
