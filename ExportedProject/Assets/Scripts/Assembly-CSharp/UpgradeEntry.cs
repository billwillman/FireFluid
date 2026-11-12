using System;

[Serializable]
public class UpgradeEntry
{
	public int level;

	public string id;

	public int found;

	public int picked;

	public bool active = true;
}
