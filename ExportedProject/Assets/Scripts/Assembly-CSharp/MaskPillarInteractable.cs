using UnityEngine;

public class MaskPillarInteractable : InstantiatePrefabInteractable
{
	public GameObject maskModelContainer;

	public CharacterMask mask;

	public override bool CanInteract()
	{
		if (base.CanInteract())
		{
			return PersistentDataComponent.GetDataInstance().currentMask != mask.GetIdentifier();
		}
		return false;
	}

	public override void OnStartInteract(InteractionController c)
	{
		base.OnStartInteract(c);
		instance.GetComponent<MaskUI>().mask = mask;
		if (PersistentDataComponent.GetDataInstance().GetData(mask.GetIdentifier()) == null)
		{
			PersistentDataComponent.GetDataInstance().SetData(mask.GetIdentifier(), true);
		}
	}

	private void Update()
	{
		if (maskModelContainer != null)
		{
			if (!CanInteract())
			{
				maskModelContainer.SetActive(value: false);
			}
			else
			{
				maskModelContainer.SetActive(value: true);
			}
		}
	}
}
