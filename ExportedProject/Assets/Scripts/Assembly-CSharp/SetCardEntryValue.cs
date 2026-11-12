using System.Collections.Generic;
using UnityEngine;

public class SetCardEntryValue : MonoBehaviour
{
	public void SetCardActive(UpgradeEntry entry, bool active)
	{
		entry.active = active;
	}

	public void CheckSelectionForActive(UpgradeEntry entry, List<UpgradeCardUI> selection, UpgradeCardUI upgradeCard)
	{
		if (entry.active)
		{
			selection.Add(upgradeCard);
		}
	}

	public void LockIfCantLevelUp(List<UpgradeCardUI> upgradeCards)
	{
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			if (upgradeCard.upgrade.CanLevelUp())
			{
				upgradeCard.SetLocked(locked: false);
			}
			else
			{
				upgradeCard.SetLocked(locked: true);
			}
		}
	}

	public void LockIfCantActivateOrDeactivate(List<UpgradeCardUI> upgradeCards)
	{
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			if (upgradeCard.upgrade.CanActivateOrDeactivate())
			{
				upgradeCard.SetLocked(locked: false);
			}
			else
			{
				upgradeCard.SetLocked(locked: true);
			}
		}
	}

	public void IncreaseLevel(UpgradeEntry entry, int levels = 1)
	{
		entry.level += levels;
	}

	public void IncreaseLevelForCurrency(UpgradeEntry entry, bool active)
	{
		int levelUpCost = Encyclopedia.Get().GetUpgrade(entry.id).GetLevelUpCost();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(levelUpCost))
		{
			PersistentDataComponent.GetDataInstance().SpendCurrency(levelUpCost);
			entry.level++;
		}
	}

	public void SetActiveForCurrency(UpgradeEntry entry, bool active)
	{
		int activateDeactivateCost = Encyclopedia.Get().GetUpgrade(entry.id).GetActivateDeactivateCost();
		if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(activateDeactivateCost))
		{
			PersistentDataComponent.GetDataInstance().SpendCurrency(activateDeactivateCost);
			entry.active = active;
		}
	}
}
