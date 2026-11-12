using UnityEngine;

public class BookOfWisdomUIInteraction : MonoBehaviour, IUpgradeInteraction
{
	public bool CanInteractUpgrade(Upgrade upgrade)
	{
		if (upgrade == null)
		{
			return false;
		}
		if (upgrade.lockedInDemo)
		{
			return false;
		}
		if (upgrade.CanLevelUp())
		{
			return upgrade.UnlockRequirementsMet();
		}
		return false;
	}

	public void OnUpgradeInteraction(Upgrade upgrade)
	{
		upgrade.LevelUp();
	}

	public string GetInteractionToolTip(Upgrade upgrade)
	{
		if (upgrade != null)
		{
			if (upgrade.lockedInDemo)
			{
				return StringLocalization.Get("dialog_lockedindemo");
			}
			if (upgrade.CanLevelUp())
			{
				if (upgrade.GetEntry().level == 0)
				{
					if (upgrade.UnlockRequirementsMet())
					{
						bool flag = (int)(Time.unscaledTime * 4f) % 2 == 0;
						return (flag ? "<color=#888888>" : "") + StringLocalization.Get("bookofwisdom_unlockupgrade") + (flag ? "</color>" : "") + " (" + upgrade.GetLevelUpCost() + "<sprite name=currency>)";
					}
					return "<color=#ff0000>" + StringLocalization.Get("bookofwisdom_requirementsnotmet") + "</color>";
				}
				if (upgrade.UnlockRequirementsMet())
				{
					return StringLocalization.Get("bookofwisdom_addheat") + " (" + upgrade.GetLevelUpCost() + "<sprite name=currency>)";
				}
				return "<color=#ff0000>" + StringLocalization.Get("bookofwisdom_requirementsnotmet") + "</color>";
			}
			if (upgrade.MaxLevelReached())
			{
				return "<color=#ff0000>" + StringLocalization.Get("bookofwisdom_maximumheatreached") + "</color>";
			}
			return "<color=#ff0000>" + StringLocalization.Get("general_notenoughcurrency") + " (" + upgrade.GetLevelUpCost() + ")</color>";
		}
		return StringLocalization.Get("bookofwisdom_addheat");
	}
}
