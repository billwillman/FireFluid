using UnityEngine;

public interface IAbilityStatus
{
	string GetIdentifier();

	Sprite GetIcon();

	float GetSize();

	string GetToolTip();

	float GetCooldown();

	float GetCurrentCooldown();

	int GetCooldownStacks();

	int GetCurrentCooldownStacks();

	bool CooldownStacksFull();

	bool CurrentlyTriggering();

	bool CooldownIsExhausted();

	bool CanSpendMana(bool spendAttempt);
}
