using UnityEngine;

public abstract class CinematicAction
{
	public CinematicMachine parent;

	protected bool blocking;

	public bool started;

	public IInterpolator interpolator = Interpolators.linear;

	public abstract bool UpdateAction(float dt);

	public virtual void StartAction()
	{
	}

	public virtual bool isBlocking()
	{
		return blocking;
	}

	public virtual void SetBlocking(bool blocking)
	{
		this.blocking = blocking;
	}

	public abstract void SetActor(GameObject actor);
}
