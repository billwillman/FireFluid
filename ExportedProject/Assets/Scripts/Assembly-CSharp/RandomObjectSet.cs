using UnityEngine;

public class RandomObjectSet : GameObjectSetSwitch
{
	private void Start()
	{
		foreach (GameObjectSwitchSet objectSet in objectSets)
		{
			GameObjectSwitchSetEntry gameObjectSwitchSetEntry = objectSet.entries[Random.Range(0, objectSet.entries.Count)];
			ActivateSetEntry(objectSet.id, gameObjectSwitchSetEntry.id);
		}
	}
}
