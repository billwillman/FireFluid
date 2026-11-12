using System;
using System.Collections.Generic;

[Serializable]
public class Feedback
{
	public string applicationIdentifier;

	public string version;

	public int saveFileIdentifier;

	public string message;

	public float playTime;

	public int unlockedLevels = 1;

	public int numberOfRunsPlayed;

	public int numberOfRunsWon;

	public int totalCurrencyCollected;

	public int totalCurrencySpent;

	public int upgradesUnlocked;

	public int heatAdded;

	public int upgradesInactive;

	public int currencyTossedInTheWell;

	public string currentScene;

	public int currentPlayerLevel;

	public float currentRunTime;

	public string currentMask = "";

	public List<string> currentUpgrades = new List<string>();
}
