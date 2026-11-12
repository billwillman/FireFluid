using UnityEngine;

public class CinematicKinematicScaleTo : CinematicAction
{
	private Vector3 startingScale;

	private Vector3 destinationScale;

	private Transform actorTransform;

	private float time;

	private float passedTime;

	public override void SetActor(GameObject actor)
	{
		actorTransform = actor.transform;
	}

	public CinematicKinematicScaleTo(Vector3 destinationScale, float time, bool blocking = false)
	{
		this.time = time;
		this.destinationScale = destinationScale;
		base.blocking = blocking;
	}

	public override void StartAction()
	{
		startingScale = actorTransform.localScale;
	}

	public override bool UpdateAction(float dt)
	{
		passedTime += dt;
		float t = Mathf.Clamp01(passedTime / time);
		Vector3 localScale = interpolator.interpolate(startingScale, destinationScale, t);
		actorTransform.localScale = localScale;
		if (passedTime >= time)
		{
			actorTransform.localScale = destinationScale;
			return true;
		}
		return false;
	}
}
