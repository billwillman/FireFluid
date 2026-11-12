using UnityEngine;

public class FireWalk : Ability
{
	public Vector3 reaction = new Vector3(0.1f, 0f, 0f);

	private FluidEmitter fluidEmitter;

	public bool emitting = true;

	private new void Start()
	{
		fluidEmitter = GetComponentInChildren<FluidEmitter>();
	}

	public override bool CanTrigger(AbilityCastParameters parameters)
	{
		if (fluidEmitter == null)
		{
			Debug.Log("FireWalk is missing an fluid emitter");
			return false;
		}
		return base.CanTrigger(parameters);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		base.Trigger(parameters);
		fluidEmitter.reaction = reaction;
		fluidEmitter.temperature = Mathf.Max(reaction.x, Mathf.Max(reaction.y, reaction.z));
		return true;
	}

	public override void TriggerEndChanneling()
	{
		base.TriggerEndChanneling();
		fluidEmitter.reaction = Vector3.zero;
		fluidEmitter.temperature = 0f;
	}
}
