using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptedUpgrade
{
	[HideInInspector]
	public bool granted;

	public float time;

	public List<Upgrade> availableUpgrades = new List<Upgrade>();
}
