using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class PauseUI : MonoBehaviour
{
	public Button firstSelection;

	public GameObject settingsMenu;

	private void Start()
	{
	}

	public void DisablePauseMenu()
	{
		base.enabled = false;
	}

	public void Pause()
	{
		TimeManager.Get().paused.Push("PauseUI", value: true);
		base.transform.GetChild(0).gameObject.SetActive(value: true);
		firstSelection.Select();
		UILayer.Open(this);
	}

	public void UnPause()
	{
		TimeManager.Get().paused.Pop("PauseUI");
		base.transform.GetChild(0).gameObject.SetActive(value: false);
		InputManager.ConsumeInputs();
		UILayer.Close(this);
	}

	public void OpenSettings()
	{
		settingsMenu.GetComponent<SettingsUI>().Open();
	}

	private void Update()
	{
		bool flag = InputManager.Menu == ButtonState.Down;
		bool flag2 = InputManager.MenuBack == ButtonState.Down;
		if ((UILayer.IsActive(this) || !UILayer.AnyLayerOpen()) && !CutSceneManager.Get().IsBusy() && (flag || flag2))
		{
			if (TimeManager.Get().paused.Get("PauseUI") != null)
			{
				UnPause();
			}
			else if (flag)
			{
				Pause();
			}
		}
	}
}
