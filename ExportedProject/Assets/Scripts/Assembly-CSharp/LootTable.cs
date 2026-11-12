using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
	public List<DropChanceEntry> drops = new List<DropChanceEntry>();

	public void DropLoot(Vector3 position, bool guaranteedDop = false)
	{
		DropChanceEntry drop = GetDrop(guaranteedDop, Player.Get().GetAttributes().luck.GetCurrentValue());
		if (drop != null)
		{
			InstantiateDrop(drop, position);
		}
	}

	public DropChanceEntry GetDrop(bool guaranteeDrop = false, float luck = 10f)
	{
		DropChanceEntry dropChanceEntry = null;
		if (guaranteeDrop)
		{
			float num = 0f;
			foreach (DropChanceEntry drop in drops)
			{
				num += drop.chance;
			}
			float num2 = Random.value * num;
			foreach (DropChanceEntry drop2 in drops)
			{
				if (num2 < drop2.chance)
				{
					dropChanceEntry = drop2;
					break;
				}
				num2 -= drop2.chance;
			}
			if (dropChanceEntry == null)
			{
				dropChanceEntry = drops[drops.Count - 1];
			}
		}
		else
		{
			foreach (DropChanceEntry drop3 in drops)
			{
				if (Random.value < drop3.chance)
				{
					dropChanceEntry = drop3;
					break;
				}
			}
		}
		if (Random.value < Mathf.Clamp01((luck - 10f) * 0.01f))
		{
			if (dropChanceEntry == null)
			{
				dropChanceEntry = GetDrop(guaranteeDrop: true);
			}
			else
			{
				List<DropChanceEntry> list = new List<DropChanceEntry>();
				foreach (DropChanceEntry drop4 in drops)
				{
					if (drop4.quality > dropChanceEntry.quality)
					{
						list.Add(drop4);
					}
				}
				if (list.Count > 0)
				{
					dropChanceEntry = list[Random.Range(0, list.Count)];
				}
			}
		}
		return dropChanceEntry;
	}

	public GameObject InstantiateDrop(DropChanceEntry drop, Vector3 position, bool addDropAnimation = true)
	{
		if (drop == null || drop.prefab == null)
		{
			return null;
		}
		if ((bool)PositionProvider.Get())
		{
			position = PositionProvider.Get().GetValidPositionNear(position);
		}
		GameObject gameObject = Object.Instantiate(drop.prefab, position, Quaternion.identity);
		if (addDropAnimation)
		{
			gameObject.transform.position = position;
			gameObject.AddComponent<DropAnimation>();
			if ((bool)gameObject.GetComponent<Item>())
			{
				SoundEffectManager.Play("item.drop", gameObject.transform.position);
			}
			else if ((bool)gameObject.GetComponent<Gem>())
			{
				gameObject.transform.position = position + Vector3.up * 2f;
			}
		}
		return gameObject;
	}
}
