public interface IUpgradeInteraction
{
	bool CanInteractUpgrade(Upgrade upgrade);

	void OnUpgradeInteraction(Upgrade upgrade);

	string GetInteractionToolTip(Upgrade upgrade);
}
