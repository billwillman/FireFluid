using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class RunStats
{
	public RunState state = RunState.Ongoing;

	public string maskUsed = "";

	public float secondsOfSurvival;

	public float highestDamageHit;

	[NonSerialized]
	[XmlIgnore]
	public Dictionary<string, float> damageSourceStatsMap = new Dictionary<string, float>();

	public List<DamageSourceStat> damageSourceStats = new List<DamageSourceStat>();
}
