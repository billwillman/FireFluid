using UnityEngine;

public class BurdenRequirementSurviveTime : BurdenRequirement
{
	public float currentSeconds;

	public int seconds = 30;

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_requirement_survivetime").Replace("{seconds}", Mathf.FloorToInt((float)seconds - currentSeconds + 0.001f).ToString() ?? "");
	}

	public override string GetLabelText()
	{
		return Mathf.CeilToInt((float)seconds - currentSeconds) + "<size=50%>s</size>";
	}

	public override float GetProgress()
	{
		return 1f * currentSeconds / (float)seconds;
	}

	public override bool IsMet()
	{
		return currentSeconds >= (float)seconds;
	}

	public void Update()
	{
		currentSeconds += Time.deltaTime;
	}
}
