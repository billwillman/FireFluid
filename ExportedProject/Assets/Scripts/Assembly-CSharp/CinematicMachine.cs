using System.Collections.Generic;
using UnityEngine;

public class CinematicMachine
{
	private GameObject actor;

	private List<CinematicAction> actions = new List<CinematicAction>();

	public bool isCinematic;

	public float fastForward = 1f;

	private IInterpolator interpolator = Interpolators.linear;

	private List<CinematicAction> remove = new List<CinematicAction>();

	public void PushAction(CinematicAction action)
	{
		action.parent = this;
		action.interpolator = interpolator;
		action.SetActor(actor);
		actions.Add(action);
	}

	public void ClearActions()
	{
		actions.Clear();
	}

	public void SetActor(GameObject actor)
	{
		this.actor = actor;
	}

	public GameObject GetActor()
	{
		if (actor == null)
		{
			Debug.LogWarning("Trying to access CinematicProcessor's actor, but actor is NULL.");
		}
		return actor;
	}

	public void SetInterpolator(IInterpolator interpolator)
	{
		this.interpolator = interpolator;
	}

	public void EnterCinematicMode()
	{
		PushAction(new CinematicFunction(delegate
		{
			isCinematic = true;
			return true;
		}));
	}

	public void ExitCinematicMode()
	{
		PushAction(new CinematicFunction(delegate
		{
			isCinematic = false;
			return true;
		}));
	}

	public void UpdateActions(float dt)
	{
		remove.Clear();
		for (int i = 0; i < actions.Count; i++)
		{
			if (!actions[i].started)
			{
				actions[i].StartAction();
				actions[i].started = true;
			}
			if (actions[i].UpdateAction(dt))
			{
				remove.Add(actions[i]);
			}
			if (actions[i].isBlocking())
			{
				break;
			}
		}
		foreach (CinematicAction item in remove)
		{
			actions.Remove(item);
		}
	}

	public bool IsBusy()
	{
		return actions.Count > 0;
	}
}
