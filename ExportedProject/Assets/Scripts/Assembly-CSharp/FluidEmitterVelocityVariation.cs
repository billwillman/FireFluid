using UnityEngine;

[RequireComponent(typeof(FluidEmitter))]
public class FluidEmitterVelocityVariation : MonoBehaviour
{
	public FluidEmitterVelocityVariationType type;

	public float intensity = 1f;

	public float speed = 1f;

	private FluidEmitter emitter;

	private void Start()
	{
		emitter = GetComponent<FluidEmitter>();
	}

	private void Update()
	{
		switch (type)
		{
		case FluidEmitterVelocityVariationType.Rotate:
			emitter.velocityX = Mathf.Sin(Time.time * speed) * intensity;
			emitter.velocityY = Mathf.Cos(Time.time * speed) * intensity;
			break;
		case FluidEmitterVelocityVariationType.Noise:
			emitter.velocityX = (Mathf.PerlinNoise1D(Time.time * speed) - 0.5f) * intensity;
			emitter.velocityY = (Mathf.PerlinNoise1D(Time.time * speed + 8199213f) - 0.5f) * intensity;
			break;
		}
	}
}
