using System;
using UnityEngine;

public class ContainedLiquidDriver : MonoBehaviour
{
	public Material material;

	[Header("Tilt Influence")]
	public float rotationInfluence = 0.2f;

	[Header("Sway Spring")]
	public float springStrength = 30f;

	public float springDamping = 3f;

	[Header("Tilt Limit")]
	[Range(0f, 45f)]
	public float maxTiltAngle = 45f;

	[Header("Wave Settings")]
	public float movementInfluence = 0.2f;

	public float waveDamping = 2f;

	private Vector3 gravity;

	private Vector3 surfaceDirection;

	private Vector3 tiltOffset;

	private Vector3 tiltVelocity;

	private float displacement;

	private Quaternion lastRotation;

	private Vector3 lastPosition;

	private void Start()
	{
		gravity = -Camera.main.transform.up;
		surfaceDirection = gravity;
		displacement = 0f;
		lastRotation = base.transform.rotation;
		lastPosition = base.transform.position;
	}

	private void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (!(unscaledDeltaTime < 0.0001f))
		{
			Vector3 up = base.transform.up;
			Vector3 vector = lastRotation * Vector3.up;
			Vector3 vector2 = Vector3.ProjectOnPlane(up - vector, gravity);
			tiltVelocity -= vector2 * rotationInfluence / unscaledDeltaTime;
			Vector3 vector3 = -tiltOffset * springStrength;
			tiltVelocity += vector3 * unscaledDeltaTime;
			tiltVelocity *= Mathf.Exp((0f - springDamping) * unscaledDeltaTime);
			tiltOffset += tiltVelocity * unscaledDeltaTime;
			float num = maxTiltAngle * (MathF.PI / 180f);
			if (tiltOffset.magnitude > num)
			{
				tiltOffset = tiltOffset.normalized * num;
				tiltVelocity = Vector3.ProjectOnPlane(tiltVelocity, tiltOffset);
			}
			tiltOffset = Vector3.ProjectOnPlane(tiltOffset, gravity);
			surfaceDirection = (gravity + tiltOffset).normalized;
			displacement += tiltVelocity.magnitude * movementInfluence * unscaledDeltaTime;
			displacement = Mathf.Min(displacement, 2f);
			displacement = Mathf.Lerp(displacement, 0f, unscaledDeltaTime * waveDamping);
			material.SetVector("_SurfaceDirection", surfaceDirection);
			material.SetFloat("_Displacement", displacement);
			lastRotation = base.transform.rotation;
			lastPosition = base.transform.position;
		}
	}
}
