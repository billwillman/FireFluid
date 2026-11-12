using System.Collections.Generic;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
	public bool showAbilities = true;

	public bool showStatusEffects = true;

	public GameObject abilityIconPrefab;

	public GameObject abilityContainer;

	public GameObject statusEffectIconPrefab;

	public GameObject statusEffectContainer;

	private Dictionary<string, AbilityStatusIcon> abilityStatusIcons = new Dictionary<string, AbilityStatusIcon>();

	private Dictionary<string, StatusEffectIcon> statusEffectIcons = new Dictionary<string, StatusEffectIcon>();

	private List<IAbilityStatus> abilityStatuses = new List<IAbilityStatus>();

	private void Start()
	{
		abilityStatusIcons.Clear();
		statusEffectIcons.Clear();
	}

	private void Update()
	{
		Player player = Player.Get();
		abilityContainer.SetActive(!player.GetAttributes().health.IsExhausted());
		List<string> list = new List<string>();
		if (showAbilities)
		{
			abilityStatuses.Clear();
			player.GetComponentsInChildren(abilityStatuses);
			foreach (IAbilityStatus abilityStatus in abilityStatuses)
			{
				if (!abilityStatusIcons.TryGetValue(abilityStatus.GetIdentifier(), out var value))
				{
					value = Object.Instantiate(abilityIconPrefab, abilityContainer.transform).GetComponent<AbilityStatusIcon>();
					value.Init(abilityStatus);
					abilityStatusIcons.Add(abilityStatus.GetIdentifier(), value);
				}
			}
			foreach (string key in abilityStatusIcons.Keys)
			{
				if (!abilityStatuses.Contains(abilityStatusIcons[key].abilityStatus))
				{
					list.Add(key);
				}
			}
		}
		else
		{
			foreach (string key2 in abilityStatusIcons.Keys)
			{
				list.Add(key2);
			}
		}
		foreach (string item in list)
		{
			Object.Destroy(abilityStatusIcons[item].gameObject);
			abilityStatusIcons.Remove(item);
		}
		list.Clear();
		if (showStatusEffects)
		{
			foreach (StatusEffect effect in StatusEffectManager.GetEffects(player.GetAttributes()))
			{
				if (!statusEffectIcons.TryGetValue(effect.GetTypeIdentifier(), out var value2))
				{
					value2 = Object.Instantiate(statusEffectIconPrefab, statusEffectContainer.transform).GetComponent<StatusEffectIcon>();
					value2.Init(effect);
					statusEffectIcons.Add(effect.GetTypeIdentifier(), value2);
				}
			}
			foreach (string key3 in statusEffectIcons.Keys)
			{
				if (statusEffectIcons[key3].statusEffect == null || statusEffectIcons[key3].statusEffect.IsExpired())
				{
					list.Add(key3);
				}
			}
		}
		else
		{
			foreach (string key4 in statusEffectIcons.Keys)
			{
				list.Add(key4);
			}
		}
		foreach (string item2 in list)
		{
			Object.Destroy(statusEffectIcons[item2].gameObject);
			statusEffectIcons.Remove(item2);
		}
	}
}
