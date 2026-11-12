using UnityEngine;

public class CinematicSetPositionTo : CinematicAction
{
	private Vector3 position;

	private Vector3 startingPosition;

	private bool startingPositionSet;

	private Transform actorTransform;

	private Transform relativeTo;

	private float time;

	private float passedTime;

	public CinematicSetPositionTo(Vector3 position, float time, Transform relativeTo = null, bool blocking = false)
	{
		this.position = position;
		base.blocking = blocking;
		this.time = time;
		this.relativeTo = relativeTo;
	}

	public override void SetActor(GameObject actor)
	{
		actorTransform = actor.transform;
	}

	public override bool UpdateAction(float dt)
	{
		if (!startingPositionSet)
		{
			startingPosition = actorTransform.position;
			startingPositionSet = true;
		}
		Vector3 b = ((!(relativeTo == null)) ? (relativeTo.position + position) : position);
		passedTime += dt;
		float t = Mathf.Clamp01(passedTime / time);
		Vector3 vector = interpolator.interpolate(startingPosition, b, t);
		actorTransform.position = vector;
		if (passedTime >= time)
		{
			actorTransform.position = b;
			return true;
		}
		return false;
	}
}
