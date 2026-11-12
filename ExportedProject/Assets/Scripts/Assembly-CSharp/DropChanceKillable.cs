using System.Collections.Generic;
using UnityEngine;

public class DropChanceKillable : MonoBehaviour, IKillable
{
	public bool addDropAnimation = true;

	public LootTable dropTable;

	public List<DropChanceEntry> drops = new List<DropChanceEntry>();

	public void OnKill()
	{
		if ((bool)dropTable)
		{
			DropChanceEntry drop = dropTable.GetDrop(guaranteeDrop: false, Player.Get().GetAttributes().luck.GetCurrentValue());
			dropTable.InstantiateDrop(drop, base.transform.position);
		}
		foreach (DropChanceEntry drop2 in drops)
		{
			if (!(Random.value < drop2.chance))
			{
				continue;
			}
			GameObject gameObject = Object.Instantiate(drop2.prefab, base.transform.position, Quaternion.identity);
			if (addDropAnimation)
			{
				gameObject.transform.position = base.transform.position + Vector3.up * 2f;
				gameObject.AddComponent<DropAnimation>();
				if ((bool)gameObject.GetComponent<Item>())
				{
					SoundEffectManager.Play("item.drop", gameObject.transform.position);
				}
			}
		}
	}
}
