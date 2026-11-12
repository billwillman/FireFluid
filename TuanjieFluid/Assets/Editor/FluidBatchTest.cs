using System;
using UnityEditor;
using UnityEngine;

public static class FluidBatchTest
{
	public static void Run()
	{
		int exitCode = 1;
		try
		{
			exitCode = RunInternal();
		}
		catch (Exception exception)
		{
			Debug.LogError("FluidBatchTest exception: " + exception);
			exitCode = 1;
		}
		EditorApplication.Exit(exitCode);
	}

	private static int RunInternal()
	{
		string[] shaderNames =
		{
			"2DFluidSim/FluidRender3D",
			"2DFluidSim/FluidRender",
			"2DFluidSim/ApplyAdvection",
			"2DFluidSim/ApplyBuoyancy",
			"2DFluidSim/ApplyDiffusion",
			"2DFluidSim/ApplyEmitterRenderTexture",
			"2DFluidSim/ApplyImpulse",
			"2DFluidSim/ApplyNoise",
			"2DFluidSim/ApplyObstacleBorder",
			"2DFluidSim/ClearRenderTexture",
			"2DFluidSim/ComputeConfinement",
			"2DFluidSim/ComputeDivergence",
			"2DFluidSim/ComputeLight",
			"2DFluidSim/ComputeMultiGridPressure",
			"2DFluidSim/ComputePressure",
			"2DFluidSim/ComputeProjection",
			"2DFluidSim/ComputeVorticity",
			"2DFluidSim/ShiftTexture",
			"FluidSim/EmitterReaction",
			"FluidSim/EmitterTemperature",
			"FluidSim/EmitterVelocity"
		};

		bool shaderFailed = false;
		foreach (string shaderName in shaderNames)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				Debug.LogError("FluidBatchTest missing shader " + shaderName);
				shaderFailed = true;
				continue;
			}
			if (ShaderUtil.ShaderHasError(shader))
			{
				Debug.LogError("FluidBatchTest shader error " + shaderName);
				shaderFailed = true;
			}
		}
		if (shaderFailed)
		{
			return 2;
		}

		Sprite sprite = MakeSprite();
		GameObject simObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		simObject.name = "FluidBatchSim";
		simObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		simObject.transform.localScale = new Vector3(16f, 16f, 1f);
		UnityEngine.Object.DestroyImmediate(simObject.GetComponent<Collider>());
		FluidSim sim = simObject.AddComponent<FluidSim>();
		sim.enableParticles = false;
		sim.velocityEmitterScaling = 128f;
		sim.Init();

		GameObject emitterObject = new GameObject("FluidBatchEmitter");
		emitterObject.transform.SetParent(simObject.transform, false);
		emitterObject.transform.localPosition = new Vector3(0f, 0f, -0.55f);
		emitterObject.transform.localScale = new Vector3(0.12f, 0.12f, 1f);
		SpriteRenderer spriteRenderer = emitterObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		FluidEmitter emitter = emitterObject.AddComponent<FluidEmitter>();
		emitter.reaction = new Vector3(1f, 0f, 0f);
		emitter.temperature = 1f;
		emitter.outwardsForce = 0.4f;
		emitter.Awake();
		emitter.Start();

		sim.RenderEmitterPasses();
		for (int i = 0; i < 6; i++)
		{
			sim.Simulate(0.1f);
		}

		float reactionPeak = PeakChannel(sim.GetReaction());
		Debug.Log("FluidBatchTest reactionPeak=" + reactionPeak.ToString("0.####"));
		if (reactionPeak < 0.05f)
		{
			Debug.LogError("FluidBatchTest reaction stayed empty");
			return 3;
		}
		Debug.Log("FluidBatchTest passed");
		return 0;
	}

	private static float PeakChannel(RenderTexture source)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		Graphics.Blit(source, temporary);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D readable = new Texture2D(temporary.width, temporary.height, TextureFormat.RGBA32, false, true);
		readable.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
		readable.Apply();
		RenderTexture.active = previous;
		Color[] pixels = readable.GetPixels();
		float peak = 0f;
		for (int i = 0; i < pixels.Length; i++)
		{
			peak = Mathf.Max(peak, pixels[i].r);
			peak = Mathf.Max(peak, pixels[i].g);
			peak = Mathf.Max(peak, pixels[i].b);
		}
		UnityEngine.Object.DestroyImmediate(readable);
		RenderTexture.ReleaseTemporary(temporary);
		return peak;
	}

	private static Sprite MakeSprite()
	{
		Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
		for (int y = 0; y < 64; y++)
		{
			for (int x = 0; x < 64; x++)
			{
				float dx = (x + 0.5f) / 64f * 2f - 1f;
				float dy = (y + 0.5f) / 64f * 2f - 1f;
				float alpha = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha * alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 64f);
	}
}
