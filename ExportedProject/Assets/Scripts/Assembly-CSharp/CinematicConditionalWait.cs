using UnityEngine;

public class CinematicConditionalWait : CinematicAction
{
	public delegate bool ConditionSatisfied();

	private float waitTime;

	public ConditionSatisfied condition;

	public override void SetActor(GameObject actor)
	{
	}

	public CinematicConditionalWait(float timeInSeconds, ConditionSatisfied condition, bool blocking = true)
	{
		base.blocking = blocking;
		this.condition = condition;
		waitTime = timeInSeconds;
	}

	public override bool UpdateAction(float dt)
	{
		waitTime -= dt;
		if (!condition() || waitTime <= 0f)
		{
			return true;
		}
		return false;
	}
}
