using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
	private static UpgradeManager instance;

	public GameObject upgradeCardCollectionUIPrefab;

	private Transform upgradeCardContainer;

	public bool useAllUpgradesFromEncyclopedia = true;

	private List<string> uniqueUpgradesGranted = new List<string>();

	public List<Upgrade> upgrades = new List<Upgrade>();

	public List<ScriptedUpgrade> scriptedUpgrades = new List<ScriptedUpgrade>();

	public Upgrade noUpgradeAvailableUpgrade;

	public int numberOfOptionsPerDraw = 3;

	public List<string> upgradeHistory = new List<string>();

	private Transform upgradeOptionsParent;

	private Transform upgradeHistoryParent;

	public static UpgradeManager Get()
	{
		return instance;
	}

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		upgradeOptionsParent = new GameObject("Current Upgrade Options").transform;
		upgradeOptionsParent.SetParent(base.transform);
		upgradeHistoryParent = new GameObject("Upgrade History").transform;
		upgradeHistoryParent.SetParent(base.transform);
		if (!useAllUpgradesFromEncyclopedia)
		{
			return;
		}
		upgrades.Clear();
		foreach (Upgrade upgrade in Encyclopedia.Get().upgrades)
		{
			if (PersistentDataComponent.GetDataInstance().GetUpgradeEntry(upgrade.GetIdentifier()).active && !upgrade.lockedInDemo && upgrade.canBeFoundDuringRun)
			{
				upgrades.Add(upgrade);
			}
		}
	}

	public void SpendLevelUp(CharacterAttributesPlayer player)
	{
		if (player.unspendLevelUps > 0)
		{
			player.unspendLevelUps--;
			DrawUpgrade();
		}
	}

	private void UpdateUnspendLevelUps()
	{
		if (UILayer.AnyLayerOpen())
		{
			return;
		}
		Player player = Player.Get();
		CharacterAttributesPlayer attributes = player.GetAttributes();
		if (!attributes || attributes.unspendLevelUps <= 0 || attributes.health.IsExhausted() || RunManager.Get().state != RunState.Ongoing)
		{
			return;
		}
		switch (Settings.levelUpMethod)
		{
		case LevelUpMethod.ImmediateDraw:
			SpendLevelUp(attributes);
			break;
		case LevelUpMethod.ManualDraw:
			if (InputManager.Character == ButtonState.Down)
			{
				SpendLevelUp(attributes);
			}
			break;
		case LevelUpMethod.AutoRandomDraw:
		{
			attributes.unspendLevelUps--;
			List<Upgrade> list = DrawUpgradeChoices(player);
			if (list.Count > 0)
			{
				Upgrade upgrade = list[UnityEngine.Random.Range(0, list.Count)];
				UpgradeSelected(upgrade, player);
				TextParticleManager.Show(upgrade.GetName(), player.transform.position + Vector3.up * 2f, TextAttributes.COLOR_XP);
			}
			break;
		}
		}
	}

	private void Update()
	{
		foreach (ScriptedUpgrade scriptedUpgrade in scriptedUpgrades)
		{
			if (!scriptedUpgrade.granted && TimeManager.Get().time > scriptedUpgrade.time)
			{
				DrawUpgrade(scriptedUpgrade.availableUpgrades);
				scriptedUpgrade.granted = true;
			}
		}
		UpdateUnspendLevelUps();
	}

	public List<Upgrade> GetAvailableUpgrades(Player player)
	{
		List<Upgrade> list = new List<Upgrade>();
		foreach (Upgrade upgrade in upgrades)
		{
			if (upgrade.RequirementsMet(player) && (!upgrade.unique || !uniqueUpgradesGranted.Contains(upgrade.GetIdentifier())))
			{
				list.Add(upgrade);
			}
		}
		return list;
	}

	public Upgrade InstantiateUpgrade(Upgrade upgrade)
	{
		Upgrade component = UnityEngine.Object.Instantiate(upgrade.gameObject, upgradeOptionsParent.transform).GetComponent<Upgrade>();
		component.OnDraw();
		return component;
	}

	public List<Upgrade> DrawUpgradeChoices(Player player)
	{
		List<Upgrade> list = new List<Upgrade>();
		List<Upgrade> availableUpgrades = GetAvailableUpgrades(player);
		for (int i = 0; i < numberOfOptionsPerDraw; i++)
		{
			if (availableUpgrades.Count > 0)
			{
				Upgrade upgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
				availableUpgrades.Remove(upgrade);
				Upgrade item = InstantiateUpgrade(upgrade);
				list.Add(item);
			}
		}
		if (list.Count == 0)
		{
			list.Add(InstantiateUpgrade(noUpgradeAvailableUpgrade));
		}
		return list;
	}

	public void DrawUpgrade()
	{
		DrawUpgrade(null, null);
	}

	public void DrawUpgrade(List<Upgrade> upgradeChoices)
	{
		DrawUpgrade(upgradeChoices, null);
	}

	public void DrawUpgrade(List<Upgrade> upgradeChoices, Player player)
	{
		if (player == null)
		{
			player = Player.Get();
		}
		UpgradeCardCollectionUI upgradeCardCollectionUI = UnityEngine.Object.Instantiate(upgradeCardCollectionUIPrefab).GetComponent<UpgradeCardCollectionUI>();
		upgradeCardCollectionUI.upgradesOverride.Clear();
		upgradeCardCollectionUI.noInputTime = ((Settings.levelUpMethod == LevelUpMethod.ImmediateDraw) ? 0.5f : 0f);
		if (upgradeChoices == null)
		{
			upgradeChoices = DrawUpgradeChoices(player);
		}
		foreach (Upgrade upgradeChoice in upgradeChoices)
		{
			upgradeChoice.GetEntry().found++;
		}
		upgradeCardCollectionUI.upgradesOverride.AddRange(upgradeChoices);
		upgradeCardCollectionUI.OnSelected.AddListener(delegate(UpgradeEntry entry, bool selected)
		{
			try
			{
				foreach (Upgrade upgradeChoice2 in upgradeChoices)
				{
					if (upgradeChoice2.GetIdentifier() == entry.id)
					{
						UpgradeSelected(upgradeChoice2, player);
						break;
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			upgradeCardCollectionUI.Close();
		});
	}

	public void DrawSingleUpgrade(Player player)
	{
		if (player == null)
		{
			player = Player.Get();
		}
		UpgradeCardCollectionUI upgradeCardCollectionUI = UnityEngine.Object.Instantiate(upgradeCardCollectionUIPrefab).GetComponent<UpgradeCardCollectionUI>();
		upgradeCardCollectionUI.upgradesOverride.Clear();
		upgradeCardCollectionUI.gridX = 1;
		List<Upgrade> availableUpgrades = GetAvailableUpgrades(player);
		if (availableUpgrades.Count < 1)
		{
			return;
		}
		List<Upgrade> list = new List<Upgrade>();
		list.Add(availableUpgrades[0]);
		upgradeCardCollectionUI.upgradesOverride.AddRange(list);
		upgradeCardCollectionUI.OnSelected.AddListener(delegate(UpgradeEntry entry, bool selected)
		{
			try
			{
				Upgrade[] componentsInChildren = upgradeOptionsParent.GetComponentsInChildren<Upgrade>();
				foreach (Upgrade upgrade in componentsInChildren)
				{
					if (upgrade.GetIdentifier() == entry.id)
					{
						UpgradeSelected(upgrade, player);
						break;
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			upgradeCardCollectionUI.Close();
		});
		upgradeCardCollectionUI.QueueRandomize(availableUpgrades);
	}

	public void UpgradeSelected(Upgrade u, Player player)
	{
		if (u.unique)
		{
			uniqueUpgradesGranted.Add(u.GetIdentifier());
		}
		Upgrade.ApplyAll(Player.Get(), u.gameObject);
		u.GetEntry().picked++;
		foreach (Transform item in upgradeOptionsParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public void AddUpgradeToHistory(Upgrade u)
	{
		upgradeHistory.Add(u.GetIdentifier());
		if (u.IsInstance())
		{
			u.transform.SetParent(upgradeHistoryParent.transform);
		}
	}

	public List<Upgrade> GetAllAppliedUpgrades()
	{
		List<Upgrade> list = new List<Upgrade>();
		upgradeHistoryParent.GetComponentsInChildren(list);
		return list;
	}
}
