using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributeModificationSacrifice : Sacrifice, IGameEventListener<OnEnemySpawnedParams>
{
	public bool applyToBossses;

	public float chanceToApplyAll = 0.1f;

	public float chanceToApplyAttribute = 1f;

	public List<AttributeModification> modifications = new List<AttributeModification>();

	public override string GetDescription()
	{
		string text = "";
		SacrificeEntry sacrificeEntry = PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier());
		int num = 0;
		foreach (AttributeModification modification in modifications)
		{
			text += modification.GetDescription(sacrificeEntry.level, evaluated: false);
			if (num < modifications.Count - 1)
			{
				text += ", ";
			}
			num++;
		}
		return StringLocalization.Get("sacrifice_description_genericattribute").Replace("{chance}", Mathf.RoundToInt(100f * chanceToApplyAll * chanceToApplyAttribute) + "%").Replace("{attributes}", text);
	}

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnEnemySpawnedParams eventParameters)
	{
		CharacterAttributes enemy = eventParameters.enemy;
		BasicAI component = enemy.GetComponent<BasicAI>();
		if (!applyToBossses && (bool)component && component.boss)
		{
			return IGameEventResult.SUCCESS;
		}
		if (Random.value > chanceToApplyAll)
		{
			return IGameEventResult.SUCCESS;
		}
		SacrificeEntry sacrificeEntry = PersistentDataComponent.GetDataInstance().GetSacrificeEntry(GetIdentifier());
		foreach (AttributeModification modification in modifications)
		{
			if (!(Random.value > chanceToApplyAttribute))
			{
				modification.EvaluateModificationValue(sacrificeEntry.level);
				modification.ApplyModificationOnContext(enemy.gameObject, GetIdentifier());
				component.boosted = true;
				if (modification.attributePath.Contains("health"))
				{
					enemy.transform.localScale *= 1f + modification.GetModificationValue() * 0.1f;
				}
			}
		}
		return IGameEventResult.SUCCESS;
	}

	private void Start()
	{
		GameEventManager.Get().OnEnemySpawned.AddListener(this);
	}

	private void OnDestroy()
	{
		GameEventManager.Get().OnEnemySpawned.RemoveListener(this);
	}
}
