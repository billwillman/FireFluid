using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectSwitchSet
{
	public string id = "";

	public List<GameObjectSwitchSetEntry> entries = new List<GameObjectSwitchSetEntry>();
}
