using System.Collections.Generic;
using UnityEngine;

public class FluidObstacle : FluidEmitterRenderable
{
	private Material material;

	private SpriteRenderer spriteRenderer;

	public new static List<FluidObstacle> all = new List<FluidObstacle>();

	public override void Awake()
	{
		base.gameObject.layer = FluidSim.obstacleLayer;
		base.Awake();
		all.Add(this);
	}

	public override void Start()
	{
		base.Start();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		all.Remove(this);
	}
}
