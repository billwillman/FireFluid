using UnityEngine;

public class InstantiatePrefabInteractable : MonoBehaviour, IInteractable
{
	public GameObject prefab;

	protected GameObject instance;

	public virtual bool CanInteract()
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

	public virtual void OnStartInteract(InteractionController c)
	{
		if (instance == null)
		{
			instance = Object.Instantiate(prefab, base.transform.position, Quaternion.identity);
		}
	}
}
