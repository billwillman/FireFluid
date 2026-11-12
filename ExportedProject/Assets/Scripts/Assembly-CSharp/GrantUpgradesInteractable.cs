using System.Collections.Generic;
using UnityEngine;

public class GrantUpgradesInteractable : MonoBehaviour, IInteractable
{
	private bool consumed;

	public bool resetAbilities;

	public GameObject consumedIndicator;

	public bool consumedOnInteract = true;

	public List<Upgrade> upgrades = new List<Upgrade>();

	public bool CanInteract()
	{
		string identifier = upgrades[0].GetIdentifier();
		Ability[] componentsInChildren = Player.GetNearestPlayer(base.transform.position).GetComponentsInChildren<Ability>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetIdentifier() == identifier)
			{
				return false;
			}
		}
		return !consumed;
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
		if (consumedOnInteract)
		{
			consumed = true;
		}
		if (resetAbilities)
		{
			c.GetComponent<Player>().ResetToDefault();
		}
		foreach (Upgrade upgrade in upgrades)
		{
			Upgrade.ApplyAll(c.GetComponent<Player>(), upgrade.gameObject);
		}
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
