using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Encyclopedia : MonoBehaviour
{
	private static Encyclopedia instance;

	public List<CharacterMask> masks = new List<CharacterMask>();

	private Dictionary<string, CharacterMask> maskMap = new Dictionary<string, CharacterMask>();

	public List<Upgrade> upgrades = new List<Upgrade>();

	private Dictionary<string, Upgrade> upgradeMap = new Dictionary<string, Upgrade>();

	public List<Ability> abilities = new List<Ability>();

	private Dictionary<string, Ability> abilityMap = new Dictionary<string, Ability>();

	public List<Enemy> enemies = new List<Enemy>();

	private Dictionary<string, Enemy> enemyMap = new Dictionary<string, Enemy>();

	public List<Sacrifice> sacrifices = new List<Sacrifice>();

	private Dictionary<string, Sacrifice> sacrificeMap = new Dictionary<string, Sacrifice>();

	public static Encyclopedia Get()
	{
		return instance;
	}

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		maskMap.Clear();
		foreach (CharacterMask mask in masks)
		{
			maskMap.Add(mask.GetIdentifier(), mask);
		}
		upgradeMap.Clear();
		foreach (Upgrade upgrade in upgrades)
		{
			upgradeMap.Add(upgrade.GetIdentifier(), upgrade);
		}
		abilityMap.Clear();
		foreach (Ability ability in abilities)
		{
			abilityMap.Add(ability.GetIdentifier(), ability);
		}
		enemyMap.Clear();
		foreach (Enemy enemy in enemies)
		{
			enemyMap.Add(enemy.GetIdentifier(), enemy);
		}
		sacrificeMap.Clear();
		foreach (Sacrifice sacrifice in sacrifices)
		{
			sacrificeMap.Add(sacrifice.GetIdentifier(), sacrifice);
		}
	}

	public Upgrade GetUpgrade(string id)
	{
		return upgradeMap.GetValueOrDefault(id, null);
	}

	public Ability GetAbility(string id)
	{
		return abilityMap.GetValueOrDefault(id, null);
	}

	public CharacterMask GetMask(string id)
	{
		return maskMap.GetValueOrDefault(id, null);
	}

	public Enemy GetEnemy(string id)
	{
		return enemyMap.GetValueOrDefault(id, null);
	}

	public Sacrifice GetSacrifice(string id)
	{
		return sacrificeMap.GetValueOrDefault(id, null);
	}
}
