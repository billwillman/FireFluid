using UnityEngine;

public class CinematicFunction : CinematicAction
{
	private CinematicDelegate action;

	public GameObject actor;

	public CinematicFunction(CinematicDelegate action, bool blocking = true)
	{
		this.action = action;
		base.blocking = blocking;
	}

	public override void SetActor(GameObject actor)
	{
		this.actor = actor;
	}

	public override bool UpdateAction(float dt)
	{
		return action(dt);
	}
}
