using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class UnlockableBarrier : MonoBehaviour
{
	public string unlockDialog = "A new path has been unlocked.";

	public bool lockedInDemo;

	public List<BarrierRequirement> requirements = new List<BarrierRequirement>();

	public List<GameObject> lockedInteractables = new List<GameObject>();

	public string unlockIdentifier = "";

	public bool RequirementsMet()
	{
		bool flag = true;
		foreach (BarrierRequirement requirement in requirements)
		{
			flag = flag && requirement.IsMet();
		}
		return flag;
	}

	public bool IsLockedPersistent()
	{
		if (PersistentDataComponent.GetDataInstance().GetData((unlockIdentifier == "") ? base.gameObject.name : unlockIdentifier) != null)
		{
			return false;
		}
		return true;
	}

	private void Start()
	{
		if (lockedInDemo)
		{
			if ((bool)base.gameObject.GetComponent<InteractableShowDialog>())
			{
				Object.Destroy(base.gameObject.GetComponent<InteractableShowDialog>());
			}
			base.gameObject.AddComponent<LockedInDemoInteractableDialog>();
			Lock();
			return;
		}
		bool flag = IsLockedPersistent();
		if (RequirementsMet() || !flag)
		{
			if (flag)
			{
				FirstUnlock();
				PersistentDataComponent.GetDataInstance().SetData((unlockIdentifier == "") ? base.gameObject.name : unlockIdentifier, true);
			}
			else
			{
				Unlock();
			}
		}
		else
		{
			Lock();
		}
	}

	public void Unlock()
	{
		base.gameObject.SetActive(value: false);
		Debug.Log(base.gameObject.name + " unlocked!");
		foreach (GameObject lockedInteractable in lockedInteractables)
		{
			IInteractable[] components = lockedInteractable.GetComponents<IInteractable>();
			foreach (IInteractable interactable in components)
			{
				if (interactable is MonoBehaviour)
				{
					((MonoBehaviour)interactable).enabled = true;
				}
			}
		}
	}

	private void FirstUnlock()
	{
		Debug.Log(base.gameObject.name + " unlocked for the first time!");
		Unlock();
		CutSceneManager.Get().PushUnlockAnimation(unlockDialog, base.transform.position);
	}

	private void Lock()
	{
		foreach (GameObject lockedInteractable in lockedInteractables)
		{
			IInteractable[] components = lockedInteractable.GetComponents<IInteractable>();
			foreach (IInteractable interactable in components)
			{
				if (interactable is MonoBehaviour)
				{
					((MonoBehaviour)interactable).enabled = false;
				}
			}
		}
	}
}
