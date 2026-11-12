using UnityEngine;

public class CutSceneDialog : MonoBehaviour
{
	public bool enterCinematic = true;

	public bool executeOnStart;

	public string dialogStyleId = "";

	[TextArea(3, 32)]
	public string dialog = "Hello World!";

	public AudioClip audioClip;

	private void Start()
	{
		if (executeOnStart)
		{
			Execute();
		}
	}

	public void Execute()
	{
		if (enterCinematic)
		{
			CutSceneManager.Get().PushDialog(dialog, dialogStyleId, audioClip);
		}
		else
		{
			DialogManager.Get().ShowDialog(dialog, dialogStyleId, audioClip);
		}
	}
}
