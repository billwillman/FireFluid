using UnityEngine;

public class BasicPlayerSummon : BasicAI
{
	public float decayPerSecond = 0.02f;

	public CharacterAttributes owner;

	public override void Start()
	{
		base.Start();
		Player.decoys.Add(GetAttributes());
		owner = Player.Get().GetAttributes();
		GetAttributes().attackSpeed.value.Push("playerscaling", (owner.attackSpeed.GetCurrentValue() - owner.attackSpeed.GetCurrentBaseValue()) * 0.5f, ValueModifierType.Add);
		GetAttributes().damage.value.Push("playerscaling", (owner.damage.GetCurrentValue() - owner.damage.GetCurrentBaseValue()) * 0.2f, ValueModifierType.Add);
		GetAttributes().area.value.Push("playerscaling", (owner.area.GetCurrentValue() - owner.area.GetCurrentBaseValue()) * 0.5f, ValueModifierType.Add);
		GetAttributes().movementSpeed.value.Push("playerscaling", (owner.movementSpeed.GetCurrentValue() - owner.movementSpeed.GetCurrentBaseValue()) * 0.5f, ValueModifierType.Add);
		GetAttributes().health.value.Push("playerscaling", (owner.health.GetCurrentValue() - owner.health.GetCurrentBaseValue()) * 0.5f, ValueModifierType.Add);
		GetAttributes().health.Fill();
	}

	public override void Update()
	{
		GetAttributes().ReceiveDamage(GetAttributes().health.GetCurrentValue() * decayPerSecond * Time.deltaTime, GetAttributes(), "decay");
		base.Update();
	}

	public override void UpdateTarget()
	{
		distanceToTarget = float.MaxValue;
		Enemy nearest = Enemy.GetNearest(base.transform.position);
		if (nearest != null)
		{
			target = nearest.GetAttributes();
		}
		else
		{
			target = null;
		}
		hasTarget = target != null && !target.health.IsExhausted();
		if (hasTarget)
		{
			distanceToTarget = Vector3.ProjectOnPlane(target.transform.position - base.transform.position, Vector3.up).magnitude;
		}
	}

	public void OnDestroy()
	{
		Player.decoys.Remove(GetAttributes());
	}
}
