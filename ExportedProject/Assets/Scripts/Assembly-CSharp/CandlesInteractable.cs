using System.Collections.Generic;
using UnityEngine;

public class CandlesInteractable : InstantiatePrefabInteractable
{
	public List<GameObject> candleFlames = new List<GameObject>();

	private void Start()
	{
	}

	private void Update()
	{
		int num = PersistentDataComponent.GetDataInstance().currentlySelectedLevel - 1;
		int num2 = 0;
		foreach (GameObject candleFlame in candleFlames)
		{
			candleFlame.SetActive(num2 == num);
			num2++;
		}
	}

	public override void OnStartInteract(InteractionController c)
	{
		DialogManager.Get().ShowDialog(StringLocalization.Get("dialog_lockedindemo"), "static");
	}
}
