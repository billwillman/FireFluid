using UnityEngine;

public class GrantMaskInteractable : MonoBehaviour, IInteractable
{
	public GameObject consumedIndicator;

	public CharacterMask mask;

	public bool CanInteract()
	{
		if (PersistentDataComponent.GetDataInstance().currentMask != mask.GetIdentifier())
		{
			return base.enabled;
		}
		return false;
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
		c.GetComponent<Player>().ResetToDefault();
		mask.Apply(c.GetComponent<Player>());
		mask.TriggerDialog();
	}

	private void Update()
	{
		if (consumedIndicator != null)
		{
			if (!CanInteract())
			{
				consumedIndicator.SetActive(value: false);
			}
			else
			{
				consumedIndicator.SetActive(value: true);
			}
		}
	}
}
