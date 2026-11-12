using System.Collections.Generic;
using UnityEngine;

public class CharacterMask : MonoBehaviour
{
	public const string DEFAULT_IDENTIFIER = "impatienceandneglect";

	public string identifier = "impatienceandneglect";

	public List<Upgrade> upgrades = new List<Upgrade>();

	public Sprite icon;

	public List<string> dialogs = new List<string>();

	public GameObject highResModel;

	public string GetIdentifier()
	{
		return identifier;
	}

	public virtual void Apply(Player player)
	{
		foreach (Upgrade upgrade in upgrades)
		{
			GameObject gameObject = Object.Instantiate(upgrade.gameObject);
			Upgrade.ApplyAll(player, gameObject);
		}
		GameObjectSetSwitch component = player.GetComponent<GameObjectSetSwitch>();
		if ((bool)component)
		{
			component.ActivateSetEntry("masks", GetIdentifier());
		}
		PersistentDataComponent.GetDataInstance().currentMask = GetIdentifier();
	}

	public void TriggerDialog(bool asCutScene = false)
	{
		if (dialogs.Count > 0)
		{
			if (asCutScene)
			{
				CutSceneManager.Get().PushDialog(dialogs[Random.Range(0, dialogs.Count)], GetIdentifier());
			}
			else
			{
				DialogManager.Get().ShowDialog(dialogs[Random.Range(0, dialogs.Count)], GetIdentifier());
			}
		}
	}

	public string GetDisplayName()
	{
		return StringLocalization.Get("mask_" + GetIdentifier());
	}
}
