using UnityEngine;

public class CinematicKinematicRotateTo : CinematicAction
{
	private Vector3 startingAngles;

	private Vector3 destinationAngles;

	private Transform actorTransform;

	private float time;

	private float passedTime;

	public override void SetActor(GameObject actor)
	{
		actorTransform = actor.transform;
	}

	public CinematicKinematicRotateTo(Vector3 eulerAngles, float time, bool blocking = false)
	{
		this.time = time;
		destinationAngles = eulerAngles;
		base.blocking = blocking;
	}

	public override void StartAction()
	{
		startingAngles = actorTransform.rotation.eulerAngles;
	}

	public override bool UpdateAction(float dt)
	{
		passedTime += dt;
		float t = Mathf.Clamp01(passedTime / time);
		Vector3 vector = interpolator.interpolate(startingAngles, destinationAngles, t);
		actorTransform.rotation = Quaternion.Euler(vector.x, vector.y, vector.z);
		if (passedTime >= time)
		{
			return true;
		}
		return false;
	}
}
