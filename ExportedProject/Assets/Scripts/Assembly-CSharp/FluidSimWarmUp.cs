using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(FluidSim))]
public class FluidSimWarmUp : MonoBehaviour
{
	public int steps = 10;

	public float deltaTime = 0.2f;

	public int frameWait = 3;

	private void Update()
	{
		if (frameWait == 0)
		{
			FluidSim component = GetComponent<FluidSim>();
			component.RenderEmitterPasses();
			for (int i = 0; i < steps; i++)
			{
				component.Simulate(deltaTime);
			}
		}
		frameWait--;
	}
}
