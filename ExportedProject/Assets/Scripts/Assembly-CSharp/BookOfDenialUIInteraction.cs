using UnityEngine;

public class BookOfDenialUIInteraction : MonoBehaviour, IUpgradeInteraction
{
	private BookUI bookUI;

	private float currentResetInputHoldTime;

	public float resetInputHoldTime = 2f;

	private void Start()
	{
		bookUI = GetComponent<BookUI>();
	}

	public bool CanInteractUpgrade(Upgrade upgrade)
	{
		if (upgrade == null)
		{
			return false;
		}
		return upgrade.CanActivateOrDeactivate();
	}

	public void OnUpgradeInteraction(Upgrade upgrade)
	{
		upgrade.ActivateOrDeactivate();
	}

	public string GetInteractionToolTip(Upgrade upgrade)
	{
		UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(upgrade.GetIdentifier());
		if (upgrade != null)
		{
			string text = "";
			if (upgrade.GetActivateDeactivateCost() != 0)
			{
				text = " (" + upgrade.GetActivateDeactivateCost() + "<sprite name=currency>)";
			}
			if (upgrade.CanActivateOrDeactivate())
			{
				if (upgradeEntry.active)
				{
					return StringLocalization.Get("bookofdenial_deny") + text;
				}
				return StringLocalization.Get("bookofdenial_remember") + text;
			}
			if (upgradeEntry.level == 0)
			{
				return "<color=#ff0000>" + StringLocalization.Get("bookofdenial_locked") + "</color>";
			}
			if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(upgrade.GetActivateDeactivateCost()))
			{
				return "<color=#ff0000>" + StringLocalization.Get("bookofdenial_maxdenialreached") + "</color> (" + PersistentDataComponent.GetDataInstance().GetNumberOfInactiveUpgrades() + "/" + PersistentDataComponent.GetDataInstance().GetMaxInactiveUpgrades() + ")";
			}
			return "<color=#ff0000>" + StringLocalization.Get("general_notenoughcurrency") + text + "</color>";
		}
		return StringLocalization.Get("bookofdenial_denyremember");
	}

	public void Update()
	{
		if (!bookUI.IsResponsive())
		{
			return;
		}
		if (InputManager.SecondaryAction == ButtonState.Pressed)
		{
			currentResetInputHoldTime += Time.unscaledDeltaTime;
			if (currentResetInputHoldTime > 0.1f)
			{
				SoundEffectManager.Play("mysc.magic", currentResetInputHoldTime / resetInputHoldTime);
			}
			Transform model = bookUI.model;
			if (currentResetInputHoldTime > 3f)
			{
				currentResetInputHoldTime = 0f;
				foreach (UpgradeEntry upgradeEntry in PersistentDataComponent.GetDataInstance().upgradeEntries)
				{
					upgradeEntry.active = true;
				}
				SoundEffectManager.Play("mysc.impact");
				SoundEffectManager.Play("explosion.big");
				bookUI.UpdatePageUpgrades();
				bookUI.UpdateHighlightColors(100f);
			}
			float num = currentResetInputHoldTime;
			model.localRotation *= Quaternion.Euler(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num));
		}
		else
		{
			currentResetInputHoldTime = 0f;
		}
	}
}
