using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class OverworldMenuUI : MonoBehaviour
{
	public Button firstSelection;

	private bool opened;

	private void Start()
	{
	}

	public void DisableMenu()
	{
		base.enabled = false;
	}

	public void Open()
	{
		if (!UILayer.AnyLayerOpen() && !Player.Get().GetComponentInChildren<Rigidbody>().isKinematic)
		{
			base.transform.GetChild(0).gameObject.SetActive(value: true);
			firstSelection.Select();
			UILayer.Open(this);
			opened = true;
		}
	}

	public void Close()
	{
		base.transform.GetChild(0).gameObject.SetActive(value: false);
		InputManager.ConsumeInputs();
		UILayer.Close(this);
		opened = false;
	}

	private void Update()
	{
		if ((opened && !UILayer.IsActive(this)) || CutSceneManager.Get().IsBusy())
		{
			return;
		}
		bool flag = InputManager.Menu == ButtonState.Down;
		bool flag2 = InputManager.MenuBack == ButtonState.Down;
		if (flag || flag2)
		{
			if (opened)
			{
				Close();
			}
			else if (flag)
			{
				Open();
			}
		}
	}
}
