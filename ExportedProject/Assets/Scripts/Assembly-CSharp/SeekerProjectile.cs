using UnityEngine;

public class SeekerProjectile : Projectile
{
	public float seekerFallOff = 0.01f;

	public float seekerSpread = 1f;

	public float seekerLerp = 1f;

	private Vector3 initialVelocity;

	private Vector3 seekerVelocity;

	private Vector3 spread;

	public override void Initialize(CharacterAttributes owner, Vector3 velocity)
	{
		initialVelocity = velocity;
		spread = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * seekerSpread;
		seekerVelocity = velocity + spread;
		base.Initialize(owner, seekerVelocity);
	}

	public override void UpdateTargetSeeking()
	{
		initialVelocity = GetTargetSeekingVelocity(initialVelocity);
	}

	public override void Update()
	{
		base.Update();
		if (!rigidbody)
		{
			rigidbody = GetComponent<Rigidbody>();
			return;
		}
		spread = Vector3.Lerp(spread, Vector3.zero, seekerFallOff * Time.deltaTime);
		rigidbody.linearVelocity = Vector3.Lerp(rigidbody.linearVelocity, initialVelocity - spread, Time.deltaTime * seekerLerp);
	}
}
