using UnityEngine;

public class CinematicWait : CinematicAction
{
	private float waitTime;

	public override void SetActor(GameObject actor)
	{
	}

	public CinematicWait(float timeInSeconds, bool blocking = true)
	{
		base.blocking = blocking;
		waitTime = timeInSeconds;
	}

	public override bool UpdateAction(float dt)
	{
		waitTime -= dt;
		if (waitTime <= 0f)
		{
			return true;
		}
		return false;
	}
}
