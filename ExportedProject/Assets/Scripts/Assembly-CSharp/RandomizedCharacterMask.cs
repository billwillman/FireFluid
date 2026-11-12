using System.Collections.Generic;
using UnityEngine;

public class RandomizedCharacterMask : CharacterMask
{
	public List<Upgrade> secretUpgrades = new List<Upgrade>();

	public override void Apply(Player player)
	{
		List<Upgrade> list = new List<Upgrade>();
		List<Upgrade> list2 = new List<Upgrade>();
		foreach (CharacterMask mask in Encyclopedia.Get().masks)
		{
			foreach (Upgrade upgrade in mask.upgrades)
			{
				if (upgrade is InstantiatePrefabUpgrade)
				{
					Ability component = ((InstantiatePrefabUpgrade)upgrade).prefab.GetComponent<Ability>();
					if (component.slot == AbilitySlot.Attack)
					{
						list.Add(upgrade);
					}
					else if (component.slot == AbilitySlot.SecondaryAction)
					{
						list2.Add(upgrade);
					}
				}
			}
		}
		foreach (Upgrade secretUpgrade in secretUpgrades)
		{
			if (secretUpgrade is InstantiatePrefabUpgrade)
			{
				Ability component2 = ((InstantiatePrefabUpgrade)secretUpgrade).prefab.GetComponent<Ability>();
				if (component2.slot == AbilitySlot.Attack)
				{
					list.Add(secretUpgrade);
				}
				else if (component2.slot == AbilitySlot.SecondaryAction)
				{
					list2.Add(secretUpgrade);
				}
			}
		}
		GameObject gameObject = Object.Instantiate(list[Random.Range(0, list.Count)].gameObject);
		Upgrade.ApplyAll(player, gameObject);
		GameObject gameObject2 = Object.Instantiate(list2[Random.Range(0, list2.Count)].gameObject);
		Upgrade.ApplyAll(player, gameObject2);
		base.Apply(player);
	}
}
