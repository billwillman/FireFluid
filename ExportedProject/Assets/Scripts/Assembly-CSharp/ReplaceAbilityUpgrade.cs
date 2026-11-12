using UnityEngine;

public class ReplaceAbilityUpgrade : Upgrade
{
	public Ability abilityToReplace;

	public Ability newAbility;

	public override bool RequirementsMet(Player player)
	{
		bool flag = false;
		Ability[] componentsInChildren = player.GetComponentsInChildren<Ability>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetIdentifier() == abilityToReplace.GetIdentifier())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		return base.RequirementsMet(player);
	}

	public override void Apply(Player player)
	{
		base.Apply(player);
		Ability ability = null;
		Ability[] componentsInChildren = player.GetComponentsInChildren<Ability>();
		foreach (Ability ability2 in componentsInChildren)
		{
			if (ability2.GetIdentifier() == abilityToReplace.GetIdentifier())
			{
				ability = ability2;
				break;
			}
		}
		if (!(ability == null))
		{
			Ability component = Object.Instantiate(newAbility.gameObject, ability.transform.parent).GetComponent<Ability>();
			component.transform.localPosition = Vector3.zero;
			component.transform.localRotation = Quaternion.identity;
			ValueModification.ShallowCopyAllValueStacks(ability, component);
			Object.Destroy(ability.gameObject);
		}
	}
}
