using System.Collections.Generic;
using UnityEngine;

public class GameObjectSetSwitch : MonoBehaviour
{
	public List<GameObjectSwitchSet> objectSets = new List<GameObjectSwitchSet>();

	public void ActivateSetEntry(string setId, string entryId)
	{
		GameObjectSwitchSet gameObjectSwitchSet = null;
		foreach (GameObjectSwitchSet objectSet in objectSets)
		{
			if (objectSet.id == setId)
			{
				gameObjectSwitchSet = objectSet;
				break;
			}
		}
		if (gameObjectSwitchSet == null)
		{
			Debug.Log("GameObjectSet not found: " + setId);
			return;
		}
		foreach (GameObjectSwitchSetEntry entry in gameObjectSwitchSet.entries)
		{
			foreach (GameObject @object in entry.objects)
			{
				@object.SetActive(value: false);
			}
		}
		GameObjectSwitchSetEntry gameObjectSwitchSetEntry = null;
		foreach (GameObjectSwitchSetEntry entry2 in gameObjectSwitchSet.entries)
		{
			if (entry2.id == entryId)
			{
				gameObjectSwitchSetEntry = entry2;
				break;
			}
		}
		if (gameObjectSwitchSetEntry == null)
		{
			Debug.Log("GameObjectSetEntry not found: " + entryId + " in GameObjectSet" + setId);
			return;
		}
		foreach (GameObject object2 in gameObjectSwitchSetEntry.objects)
		{
			object2.SetActive(value: true);
		}
	}
}
