using System.Collections.Generic;
using UnityEngine;

public class InteractableShowUpgradeHint : InteractableShowHint, IInteractableHint
{
	public GameObject upgradeCardUIPrefab;

	private List<Upgrade> upgrades = new List<Upgrade>();

	private List<UpgradeCardUI> upgradeCards = new List<UpgradeCardUI>();

	public override void Awake()
	{
		Transform parent = (hintObject = base.transform.GetChild(0).GetChild(0));
		base.Awake();
		GrantUpgradesInteractable componentInParent = GetComponentInParent<GrantUpgradesInteractable>();
		if ((bool)componentInParent)
		{
			upgrades.Clear();
			upgrades.AddRange(componentInParent.upgrades);
		}
		else
		{
			GrantMaskInteractable componentInParent2 = GetComponentInParent<GrantMaskInteractable>();
			upgrades.Clear();
			upgrades.AddRange(componentInParent2.mask.upgrades);
		}
		foreach (Upgrade upgrade in upgrades)
		{
			UpgradeCardUI componentInChildren = Object.Instantiate(upgradeCardUIPrefab, parent).GetComponentInChildren<UpgradeCardUI>();
			componentInChildren.transform.localRotation = Quaternion.identity;
			componentInChildren.SetUp(upgrade);
			componentInChildren.OnSelect();
			upgradeCards.Add(componentInChildren);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!shown)
		{
			return;
		}
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			upgradeCard.UpdateState();
		}
	}
}
