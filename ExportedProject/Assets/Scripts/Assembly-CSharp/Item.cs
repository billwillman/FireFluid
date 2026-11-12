using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
	public string identifier;

	private int seed = -1;

	public int level = 1;

	public GameObject worldModel;

	public GameObject detailedModel;

	public CharacterAttributes owner;

	public List<GameObject> possibleModifiers = new List<GameObject>();

	public IItemModifier prefix;

	public IItemModifier suffix;

	public string dropSound = "item.pendant";

	private bool evaluated;

	public string GetIdentifier()
	{
		return identifier;
	}

	public string GetInstanceIdentifier()
	{
		return identifier + seed;
	}

	public string GetDisplayName(bool headline = true)
	{
		string text = "";
		if (prefix.GetIdentifier() != "")
		{
			text = StringLocalization.Get("item_prefix_" + prefix.GetIdentifier());
		}
		string text2 = "";
		if (suffix.GetIdentifier() != "")
		{
			text2 = StringLocalization.Get("item_suffix_" + suffix.GetIdentifier());
		}
		if (headline)
		{
			return "<color=#ddb888><size=150%>~" + text + " " + StringLocalization.Get("item_name_" + GetIdentifier()) + " " + text2 + "~</size></color>";
		}
		return text + " " + StringLocalization.Get("item_name_" + GetIdentifier()) + " " + text2;
	}

	public string GetDescription()
	{
		return string.Concat("" + prefix.GetDescription(level), suffix.GetDescription(level));
	}

	public void Evaluate()
	{
		if (evaluated)
		{
			return;
		}
		seed = Random.Range(0, int.MaxValue);
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		foreach (GameObject possibleModifier in possibleModifiers)
		{
			IItemModifier component = possibleModifier.GetComponent<IItemModifier>();
			if (component.GetType() == ItemModifierType.Prefix)
			{
				list.Add(possibleModifier);
			}
			if (component.GetType() == ItemModifierType.Suffix)
			{
				list2.Add(possibleModifier);
			}
		}
		prefix = Object.Instantiate(list[Random.Range(0, list.Count)].gameObject, base.transform).GetComponent<IItemModifier>();
		suffix = Object.Instantiate(list2[Random.Range(0, list2.Count)].gameObject, base.transform).GetComponent<IItemModifier>();
		prefix.SetItem(this);
		suffix.SetItem(this);
		prefix.Evaluate(level);
		suffix.Evaluate(level);
		MiniMap.Get()?.Mark(base.transform, MiniMap.COLOR_ITEM, 2f);
		evaluated = true;
	}

	public void OnEquip(CharacterAttributes owner)
	{
		this.owner = owner;
		prefix.OnEquip(owner);
		suffix.OnEquip(owner);
		MiniMap.Get()?.Unmark(base.transform);
	}

	public void OnUnequip()
	{
		prefix.OnUnequip(owner);
		suffix.OnUnequip(owner);
		owner = null;
	}

	private void Start()
	{
		Evaluate();
	}

	public bool CanInteract()
	{
		return true;
	}

	public void OnEndInteract()
	{
	}

	public bool OnInteract()
	{
		return true;
	}

	public void OnStartInteract(InteractionController c)
	{
		ItemController component = c.GetComponent<ItemController>();
		if (component != null)
		{
			component.RequestEquip(this);
		}
	}
}
