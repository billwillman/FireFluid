using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedbackUI : MonoBehaviour
{
	public GameObject hint;

	public GameObject canvas;

	public TextMeshProUGUI text;

	private bool open;

	private const string MODIFIERID = "FeedbackUI";

	private void Update()
	{
		if (Keyboard.current == null || CutSceneManager.Get().IsBusy() || !Settings.enableSendingFeedback)
		{
			canvas.SetActive(value: false);
			hint.SetActive(value: false);
			return;
		}
		hint.SetActive(!open);
		if (!open)
		{
			if (Keyboard.current.f11Key.wasPressedThisFrame)
			{
				Open();
			}
		}
		else if (Keyboard.current.f11Key.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			Close();
		}
	}

	public void Open()
	{
		canvas.SetActive(value: true);
		open = true;
		UILayer.Open(this);
		InputManager.ConsumeInputs();
		TimeManager.Get()?.paused.Push("FeedbackUI", value: true);
		canvas.GetComponentInChildren<TMP_InputField>().Select();
	}

	public void Close()
	{
		canvas.SetActive(value: false);
		open = false;
		UILayer.Close(this);
		InputManager.ConsumeInputs();
		TimeManager.Get()?.paused.PopAll("FeedbackUI");
	}

	public void Send()
	{
		FeedbackSystem.Send(text.text);
		text.text = "";
	}
}
