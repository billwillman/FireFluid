using UnityEngine;

public class InteractableShowDialog : MonoBehaviour, IInteractable
{
	public string dialog;

	public string dialogStyleId = "";

	public bool pushAsCutScene;

	public bool CanInteract()
	{
		return base.enabled;
	}

	public void OnEndInteract()
	{
	}

	public bool OnInteract()
	{
		return true;
	}

	public void OnStartInteract(InteractionController c)
	{
		if (pushAsCutScene)
		{
			CutSceneManager.Get().PushDialog(dialog, dialogStyleId);
		}
		else
		{
			DialogManager.Get().ShowDialog(dialog, dialogStyleId);
		}
	}
}
