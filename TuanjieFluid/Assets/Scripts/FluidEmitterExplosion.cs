using UnityEngine;

public class FluidEmitterExplosion : FluidEmitter
{
	public int renderFrames = 3;

	public override void Awake()
	{
		base.Awake();
		FluidSim.OnSimulationStep.AddListener(OnSimulationStep);
	}

	public void OnEmittersRendererd()
	{
	}

	public void OnSimulationStep()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (renderFrames > 0)
		{
			renderFrames--;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		FluidSim.OnSimulationStep.RemoveListener(OnSimulationStep);
	}
}
