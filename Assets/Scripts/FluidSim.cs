using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class FluidSim : MonoBehaviour
{
	public static FluidSim main;

	private const int NUM_THREADS = 16;

	public const float TIME_STEP = 0.1f;

	private const int NUM_PARTICLES = 4096;

	private const int NUM_PARTICLESSQRT = 64;

	public bool shiftTextures;

	public SimulateFunction simulate = SimulateFunction.UNITY_FIXEDUPDATE;

	public bool enableParticles;

	public bool useMultiGridPressureSolver = true;

	public static int emitterTemperatureLayer = 31;

	public static int emitterReactionLayer = 30;

	public static int emitterVelocityLayer = 29;

	public static int obstacleLayer = 28;

	public float viscosity;

	public float fireDiffusion;

	public int gridWidth = 512;

	public int gridHeight = 256;

	public int simulationDownSample = 1;

	public int iterations = 20;

	public float vorticityStrength = 1f;

	public float densityAmount = 0.1f;

	public float densityDissipation = 0.999f;

	public float densityBuoyancy = 1f;

	public float densityWeight = 0.0125f;

	public float temperatureDissipation = 0.995f;

	public float reactionDecay = 0.001f;

	public float reactionExtinguishment = 0.01f;

	public float velocityDissipation = 0.995f;

	public float noiseIntensity = 0.1f;

	public float particleAdvectionFactor = 1f;

	public float velocityEmitterScaling = 128f;

	public float temperatureEmitterScaling = 0.2f;

	public Color densityEmittedByR = new Color(0.85f, 0.25f, 0.05f, 1f);

	public Color densityEmittedByG = new Color(0.1f, 0.75f, 0.15f, 1f);

	public Color densityEmittedByB = new Color(0.15f, 0.35f, 0.95f, 1f);

	private int gridWidthDownSample;

	private int gridHeightDownSample;

	private float ambientTemperature;

	public Material particleMaterial;

	public Shader applyDiffusion;

	public Shader applyImpulse;

	public Shader applyAdvect;

	public Shader computeVorticity;

	public Shader computeConfinement;

	public Shader applyEmitterRenderTexture;

	public Shader applyBuoyancy;

	public Shader applyNoise;

	public Shader computeDivergence;

	public Shader computePressure;

	public Shader computeMultiGridPressure;

	public Shader computeProjection;

	public Shader clearRenderTexture;

	public Shader applyObstacleBorder;

	public Shader shiftTexture;

	public Shader computeLight;

	public bool computeLighting = true;

	private Material applyDiffusionMaterial;

	private Material applyImpulseMaterial;

	private Material applyAdvectMaterial;

	private Material computeVorticityMaterial;

	private Material computeConfinementMaterial;

	private Material applyNoiseMaterial;

	private Material applyEmitterRenderTextureMaterial;

	private Material applyBuoyancyMaterial;

	private Material computeDivergenceMaterial;

	private Material computePressureMaterial;

	private Material computeMultiGridPressureMaterial;

	private Material computeProjectionMaterial;

	private Material clearRenderTextureMaterial;

	private Material applyObstacleBorderMaterial;

	private Material shiftTextureMaterial;

	private Material computeLightMaterial;

	private Material renderMaterial;

	private Vector2 gridSize;

	private Vector2 gridSizeDownSample;

	private Vector2 pixelSize;

	private Vector2 pixelSizeDownSample;

	public ComputeShader particleAdvection;

	private ComputeBuffer particles;

	private RenderTexture density;

	private RenderTexture velocity;

	private RenderTexture pressure;

	private RenderTexture temperature;

	private RenderTexture reaction;

	private RenderTexture temp1f;

	private RenderTexture temp2f;

	private RenderTexture temp3f;

	private RenderTexture temp4f;

	private RenderTexture temp1fDownSample;

	private RenderTexture temp2fDownSample;

	private RenderTexture temp3fDownSample;

	private RenderTexture lightTex;

	private RenderTexture blitDummy;

	private Vector2Int pixelCoordsOriginLastShift;

	private RenderTexture detailNoiseTexture;

	public RenderEmitterPass emitterTemperaturePass;

	public RenderEmitterPass emitterReactionPass;

	public RenderEmitterPass emitterVelocityPass;

	public RenderEmitterPass emitterObstaclePass;

	private bool simulationDirty;

	private Vector4 tempDissipation;

	private Vector4 densityDissipation4;

	private Vector4 velocityDissipation4;

	private Vector4 reactionDecay4;

	private Vector4 emitterTempOffset;

	private Vector4 reactionOffset;

	private Vector4 velocityOffset;

	private Vector3 lastSimPos;

	private Vector3 lastSimX;

	private Vector3 lastSimY;

	private Vector2 lastSimScale;

	[HideInInspector]
	public static UnityEvent OnSimulationStep = new UnityEvent();

	[HideInInspector]
	public static UnityEvent OnEmittersRendered = new UnityEvent();

	private void Awake()
	{
	}

	private void Start()
	{
		Init();
		if (main == null)
		{
			main = this;
		}
	}

	private static Shader FindFluidShader(Shader current, string shaderName)
	{
		if (current != null)
		{
			return current;
		}
		return Shader.Find(shaderName);
	}

	private void EnsureShaders()
	{
		applyDiffusion = FindFluidShader(applyDiffusion, "2DFluidSim/ApplyDiffusion");
		applyImpulse = FindFluidShader(applyImpulse, "2DFluidSim/ApplyImpulse");
		applyAdvect = FindFluidShader(applyAdvect, "2DFluidSim/ApplyAdvection");
		computeVorticity = FindFluidShader(computeVorticity, "2DFluidSim/ComputeVorticity");
		computeConfinement = FindFluidShader(computeConfinement, "2DFluidSim/ComputeConfinement");
		applyEmitterRenderTexture = FindFluidShader(applyEmitterRenderTexture, "2DFluidSim/ApplyEmitterRenderTexture");
		applyBuoyancy = FindFluidShader(applyBuoyancy, "2DFluidSim/ApplyBuoyancy");
		applyNoise = FindFluidShader(applyNoise, "2DFluidSim/ApplyNoise");
		computeDivergence = FindFluidShader(computeDivergence, "2DFluidSim/ComputeDivergence");
		computePressure = FindFluidShader(computePressure, "2DFluidSim/ComputePressure");
		computeMultiGridPressure = FindFluidShader(computeMultiGridPressure, "2DFluidSim/ComputeMultiGridPressure");
		computeProjection = FindFluidShader(computeProjection, "2DFluidSim/ComputeProjection");
		clearRenderTexture = FindFluidShader(clearRenderTexture, "2DFluidSim/ClearRenderTexture");
		applyObstacleBorder = FindFluidShader(applyObstacleBorder, "2DFluidSim/ApplyObstacleBorder");
		shiftTexture = FindFluidShader(shiftTexture, "2DFluidSim/ShiftTexture");
		computeLight = FindFluidShader(computeLight, "2DFluidSim/ComputeLight");
	}

	public void Init()
	{
		EnsureShaders();
		if (particles != null)
		{
			particles.Release();
			particles = null;
		}
		gridWidth = Mathf.ClosestPowerOfTwo(gridWidth);
		gridHeight = Mathf.ClosestPowerOfTwo(gridHeight);
		gridWidthDownSample = Mathf.ClosestPowerOfTwo(gridWidth / simulationDownSample);
		gridHeightDownSample = Mathf.ClosestPowerOfTwo(gridHeight / simulationDownSample);
		gridSize = new Vector2(gridWidth, gridHeight);
		gridSizeDownSample = new Vector2(gridWidthDownSample, gridHeightDownSample);
		pixelSize = new Vector2(1f / (float)gridWidth, 1f / (float)gridHeight);
		pixelSizeDownSample = new Vector2(1f / (float)gridWidthDownSample, 1f / (float)gridHeightDownSample);
		CreateSurface(ref density, gridWidth, gridHeight, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear);
		CreateSurface(ref velocity, gridWidthDownSample, gridHeightDownSample, RenderTextureFormat.RGHalf, FilterMode.Bilinear, useMipMaps: true);
		CreateSurface(ref temperature, gridWidth, gridHeight, RenderTextureFormat.RHalf, FilterMode.Bilinear);
		CreateSurface(ref reaction, gridWidth, gridHeight, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear);
		CreateSurface(ref lightTex, gridWidth / 16, gridHeight / 16, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear, useMipMaps: true);
		CreateSurface(ref detailNoiseTexture, gridWidth, gridHeight, RenderTextureFormat.RFloat, FilterMode.Bilinear);
		CreateSurface(ref temp1f, gridWidth, gridHeight, RenderTextureFormat.RHalf, FilterMode.Bilinear);
		CreateSurface(ref temp2f, gridWidth, gridHeight, RenderTextureFormat.RGHalf, FilterMode.Bilinear);
		CreateSurface(ref temp3f, gridWidth, gridHeight, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear);
		CreateSurface(ref temp4f, gridWidth, gridHeight, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear);
		CreateSurface(ref pressure, gridWidthDownSample, gridHeightDownSample, RenderTextureFormat.RHalf, FilterMode.Bilinear, useMipMaps: true);
		CreateSurface(ref temp1fDownSample, gridWidthDownSample, gridHeightDownSample, RenderTextureFormat.RHalf, FilterMode.Bilinear, useMipMaps: true);
		CreateSurface(ref temp2fDownSample, gridWidthDownSample, gridHeightDownSample, RenderTextureFormat.RGHalf, FilterMode.Bilinear, useMipMaps: true);
		CreateSurface(ref temp3fDownSample, gridWidthDownSample, gridHeightDownSample, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear);
		applyDiffusionMaterial = new Material(applyDiffusion);
		applyImpulseMaterial = new Material(applyImpulse);
		applyEmitterRenderTextureMaterial = new Material(applyEmitterRenderTexture);
		applyAdvectMaterial = new Material(applyAdvect);
		applyNoiseMaterial = new Material(applyNoise);
		computeVorticityMaterial = new Material(computeVorticity);
		computeConfinementMaterial = new Material(computeConfinement);
		applyBuoyancyMaterial = new Material(applyBuoyancy);
		computeDivergenceMaterial = new Material(computeDivergence);
		computePressureMaterial = new Material(computePressure);
		computeMultiGridPressureMaterial = new Material(computeMultiGridPressure);
		computeProjectionMaterial = new Material(computeProjection);
		if (clearRenderTextureMaterial == null)
		{
			clearRenderTextureMaterial = new Material(clearRenderTexture);
		}
		applyObstacleBorderMaterial = new Material(applyObstacleBorder);
		shiftTextureMaterial = new Material(shiftTexture);
		computeLightMaterial = new Material(computeLight);
		renderMaterial = GetComponent<Renderer>().material;
		Shader renderShader = Shader.Find("2DFluidSim/FluidRender3D");
		if (renderShader != null && renderMaterial.shader != renderShader)
		{
			renderMaterial.shader = renderShader;
		}
		if (enableParticles && particleAdvection != null)
		{
			particles = new ComputeBuffer(4096, 12);
			InitParticles(particles);
		}
		pixelCoordsOriginLastShift = WorldToPixelCoords(Vector3.zero, simulationDownSample);
		emitterTemperaturePass = new RenderEmitterPass("emitterTemperaturePass", emitterTemperatureLayer, new Vector2Int(gridWidth, gridHeight), base.transform, new Color(0.5f, 0f, 0f));
		emitterReactionPass = new RenderEmitterPass("emitterReactionPass", emitterReactionLayer, new Vector2Int(gridWidth, gridHeight), base.transform, new Color(0f, 0f, 0f));
		emitterVelocityPass = new RenderEmitterPass("emitterVelocityPass", emitterVelocityLayer, new Vector2Int(gridWidth, gridHeight), base.transform, new Color(0.5f, 0.5f, 0f));
		emitterObstaclePass = new RenderEmitterPass("emitterObstaclePass", obstacleLayer, new Vector2Int(gridWidth, gridHeight), base.transform, Color.black);
		emitterTempOffset = new Vector4(-0.5f, 0f, 0f, 0f);
		reactionOffset = new Vector4(0f, 0f, 0f, 0f);
		velocityOffset = new Vector4(-0.5f, -0.5f, 0f, 0f);
		tempDissipation = new Vector4(temperatureDissipation, 1f, 1f, 1f);
		densityDissipation4 = new Vector4(densityDissipation, densityDissipation, densityDissipation, densityDissipation);
		velocityDissipation4 = new Vector4(velocityDissipation, velocityDissipation, 1f, 1f);
		reactionDecay4 = new Vector4(reactionDecay, reactionDecay, reactionDecay, 0f);
		blitDummy = new RenderTexture(1, 1, 0);
	}

	private void UpdateCachedDissipationVectors()
	{
		tempDissipation.x = temperatureDissipation;
		densityDissipation4.Set(densityDissipation, densityDissipation, densityDissipation, densityDissipation);
		velocityDissipation4.Set(velocityDissipation, velocityDissipation, 1f, 1f);
		reactionDecay4.Set(reactionDecay, reactionDecay, reactionDecay, 0f);
	}

	public RenderTexture GetReaction()
	{
		return reaction;
	}

	public RenderTexture GetDensity()
	{
		return density;
	}

	public RenderTexture GetVelocity()
	{
		return velocity;
	}

	private void CreateSurface(ref RenderTexture surface, int width, int height, RenderTextureFormat format, FilterMode filter, bool useMipMaps = false)
	{
		surface = new RenderTexture(width, height, 0, format, RenderTextureReadWrite.Linear);
		surface.filterMode = filter;
		surface.autoGenerateMips = false;
		surface.wrapMode = TextureWrapMode.Clamp;
		surface.useMipMap = useMipMaps;
		surface.Create();
	}

	public void Resize(int width, int height, int depth)
	{
		Debug.Log("resizeing...");
		OnDestroy();
		gridWidth = width;
		gridHeight = height;
		Start();
	}

	public Vector3 TextureToWorldCoords(Vector2 uv)
	{
		uv -= new Vector2(0.5f, 0.5f);
		uv.x *= base.transform.lossyScale.x;
		uv.y *= base.transform.lossyScale.y;
		return base.transform.position + base.transform.right * uv.x + base.transform.up * uv.y;
	}

	public Vector2 WorldToTextureCoords(Vector3 world)
	{
		Vector2 vector = new Vector2(Vector3.Dot(base.transform.right, world - base.transform.position), Vector3.Dot(base.transform.up, world - base.transform.position));
		vector.x /= base.transform.lossyScale.x;
		vector.y /= base.transform.lossyScale.y;
		return vector + new Vector2(0.5f, 0.5f);
	}

	public Vector2 WorldToSimVelocity(Vector3 worldVelocity)
	{
		Vector2 vector = WorldToTextureCoords(Vector3.zero);
		return (WorldToTextureCoords(worldVelocity) - vector) / velocityEmitterScaling / pixelSize * 0.25f;
	}

	public Vector2Int WorldToPixelCoords(Vector3 world, int downSample = 1)
	{
		Vector2 vector = WorldToTextureCoords(world);
		return new Vector2Int(Mathf.FloorToInt(vector.x * (float)gridWidth / (float)downSample), Mathf.FloorToInt(vector.y * (float)gridHeight / (float)downSample));
	}

	private void ApplyDiffusion(float dt, ref RenderTexture source, ref RenderTexture dest, float diffusion, Vector2 pixelSize)
	{
		applyDiffusionMaterial.SetVector("_PixelSize", pixelSize);
		applyDiffusionMaterial.SetFloat("_DeltaTime", dt);
		applyDiffusionMaterial.SetFloat("_Diffusion", diffusion);
		applyDiffusionMaterial.SetTexture("_Read", source);
		Graphics.Blit(null, dest, applyDiffusionMaterial);
		Swap(ref source, ref dest);
	}

	private void InitParticles(ComputeBuffer buffer)
	{
		float[] array = new float[12288];
		for (int i = 0; i < 4096; i++)
		{
			FluidEmitter randomEmitter = FluidEmitter.GetRandomEmitter();
			if ((bool)randomEmitter)
			{
				Vector2 vector = WorldToTextureCoords(randomEmitter.transform.position);
				float f = Random.Range(0f, 360f);
				array[i * 3] = vector.x + Mathf.Sin(f) * Random.Range(0f, 0.02f);
				array[i * 3 + 1] = vector.y + Mathf.Cos(f) * Random.Range(0f, 0.02f);
				array[i * 3 + 2] = -1f;
			}
			else
			{
				array[i * 3] = Random.Range(0f, 1f);
				array[i * 3 + 1] = Random.Range(0f, 1f);
				array[i * 3 + 2] = -1f;
			}
		}
		buffer.SetData(array);
	}

	private void Swap(ref RenderTexture a, ref RenderTexture b)
	{
		RenderTexture renderTexture = a;
		a = b;
		b = renderTexture;
	}

	private void Clear(RenderTexture buffer, Color clearColor)
	{
		clearRenderTextureMaterial.SetColor("_ClearColor", clearColor);
		Graphics.Blit(null, buffer, clearRenderTextureMaterial);
	}

	private void ApplyObstacleBorder()
	{
		Graphics.Blit(emitterObstaclePass.RenderTexture, temp3f);
		applyObstacleBorderMaterial.SetTexture("_Read", temp3f);
		applyObstacleBorderMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		Graphics.Blit(null, emitterObstaclePass.RenderTexture, applyObstacleBorderMaterial);
	}

	private void ApplyExtinguishmentImpulse(float amount)
	{
		applyImpulseMaterial.SetFloat("_Amount", amount * 0.2f);
		applyImpulseMaterial.SetFloat("_Extinguishment", reactionExtinguishment);
		applyImpulseMaterial.SetTexture("_Density", density);
		applyImpulseMaterial.SetTexture("_Reaction", reaction);
		applyImpulseMaterial.SetColor("_DensityEmittedByR", densityEmittedByR);
		applyImpulseMaterial.SetColor("_DensityEmittedByG", densityEmittedByG);
		applyImpulseMaterial.SetColor("_DensityEmittedByB", densityEmittedByB);
		Graphics.Blit(null, temp4f, applyImpulseMaterial);
		Swap(ref temp4f, ref density);
	}

	private void ApplyBuoyancy(float dt)
	{
		applyBuoyancyMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		applyBuoyancyMaterial.SetVector("_Up", new Vector4(0f, 1f, 0f, 0f));
		applyBuoyancyMaterial.SetFloat("_Buoyancy", densityBuoyancy);
		applyBuoyancyMaterial.SetFloat("_AmbientTemperature", ambientTemperature);
		applyBuoyancyMaterial.SetFloat("_Weight", densityWeight);
		applyBuoyancyMaterial.SetFloat("_DeltaTime", dt);
		applyBuoyancyMaterial.SetTexture("_Temperature", temperature);
		applyBuoyancyMaterial.SetTexture("_Velocity", velocity);
		applyBuoyancyMaterial.SetTexture("_Density", density);
		Graphics.Blit(null, temp2fDownSample, applyBuoyancyMaterial);
		Swap(ref temp2fDownSample, ref velocity);
	}

	private void ApplyAdvection(float dt, Vector4 dissipation, Vector4 decay, ref RenderTexture source, ref RenderTexture dest, Vector2 pixelSize)
	{
		applyAdvectMaterial.SetFloat("_DeltaTime", dt);
		applyAdvectMaterial.SetVector("_Dissipate", dissipation);
		applyAdvectMaterial.SetVector("_Decay", decay);
		applyAdvectMaterial.SetTexture("_Read", source);
		applyAdvectMaterial.SetTexture("_Velocity", velocity);
		applyAdvectMaterial.SetTexture("_Obstacle", emitterObstaclePass.RenderTexture);
		applyAdvectMaterial.SetVector("_PixelSize", pixelSize);
		Graphics.Blit(null, dest, applyAdvectMaterial);
		Swap(ref source, ref dest);
	}

	private void ApplyNoise(float intensity, ref RenderTexture source, ref RenderTexture dest, Vector2 pixelSize)
	{
		applyNoiseMaterial.SetFloat("_Intensity", intensity);
		applyNoiseMaterial.SetTexture("_Read", source);
		applyNoiseMaterial.SetVector("_PixelSize", pixelSize);
		Graphics.Blit(null, dest, applyNoiseMaterial);
		Swap(ref source, ref dest);
	}

	private void ComputeVorticityConfinement(float dt)
	{
		computeVorticityMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computeVorticityMaterial.SetTexture("_Velocity", velocity);
		Graphics.Blit(null, temp1fDownSample, computeVorticityMaterial);
		computeConfinementMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computeConfinementMaterial.SetFloat("_DeltaTime", dt);
		computeConfinementMaterial.SetFloat("_Epsilon", vorticityStrength);
		computeConfinementMaterial.SetTexture("_Velocity", velocity);
		computeConfinementMaterial.SetTexture("_Vorticity", temp1fDownSample);
		computeConfinementMaterial.SetTexture("_Reaction", reaction);
		Graphics.Blit(null, temp2fDownSample, computeConfinementMaterial);
		Swap(ref temp2fDownSample, ref velocity);
	}

	private void ComputeDivergence()
	{
		computeDivergenceMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computeDivergenceMaterial.SetTexture("_Velocity", velocity);
		computeDivergenceMaterial.SetTexture("_Obstacle", emitterObstaclePass.RenderTexture);
		Graphics.Blit(null, temp2fDownSample, computeDivergenceMaterial);
	}

	private void ComputePressure()
	{
		computePressureMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computePressureMaterial.SetTexture("_Divergence", temp2fDownSample);
		computePressureMaterial.SetTexture("_Obstacle", emitterObstaclePass.RenderTexture);
		for (int i = 0; i < iterations; i++)
		{
			computePressureMaterial.SetTexture("_Pressure", pressure);
			Graphics.Blit(null, temp1fDownSample, computePressureMaterial);
			Swap(ref temp1fDownSample, ref pressure);
		}
	}

	private void ComputeMultiGridPressure()
	{
		temp2fDownSample.GenerateMips();
		computeMultiGridPressureMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computeMultiGridPressureMaterial.SetTexture("_Divergence", temp2fDownSample);
		computeMultiGridPressureMaterial.SetTexture("_Obstacle", emitterObstaclePass.RenderTexture);
		for (int i = 0; i < iterations; i++)
		{
			pressure.GenerateMips();
			computeMultiGridPressureMaterial.SetTexture("_Pressure", pressure);
			Graphics.Blit(null, temp1fDownSample, computeMultiGridPressureMaterial);
			Swap(ref temp1fDownSample, ref pressure);
		}
	}

	private void ComputeProjection()
	{
		computeProjectionMaterial.SetVector("_PixelSize", pixelSizeDownSample);
		computeProjectionMaterial.SetTexture("_Obstacle", emitterObstaclePass.RenderTexture);
		computeProjectionMaterial.SetTexture("_Pressure", pressure);
		computeProjectionMaterial.SetTexture("_Velocity", velocity);
		Graphics.Blit(null, temp2fDownSample, computeProjectionMaterial);
		Swap(ref velocity, ref temp2fDownSample);
	}

	private void AdvectParticles()
	{
		particleAdvection.SetVector("_Size", gridSize / simulationDownSample);
		particleAdvection.SetFloat("_ForceFactor", particleAdvectionFactor / (float)simulationDownSample);
		particleAdvection.SetFloat("_DeltaTime", 0.1f);
		particleAdvection.SetFloat("_NumParticlesSqrt", 64f);
		particleAdvection.SetTexture(0, "_Velocity", velocity);
		particleAdvection.SetBuffer(0, "_Particles", particles);
		FluidEmitter randomEmitter = FluidEmitter.GetRandomEmitter();
		if (randomEmitter != null)
		{
			particleAdvection.SetVector("_Position", WorldToTextureCoords(randomEmitter.transform.localPosition));
			particleAdvection.SetFloat("_Radius", 0.01f);
		}
		particleAdvection.Dispatch(0, 4, 4, 1);
	}

	private void ApplyEmitterTexture(RenderTexture emitterRenderTexture, ref RenderTexture dest, ref RenderTexture buffer, float dt, Vector4 offset, float scaling = 1f, int mode = 0)
	{
		applyEmitterRenderTextureMaterial.SetTexture("_EmitterTexture", emitterRenderTexture);
		applyEmitterRenderTextureMaterial.SetTexture("_Read", dest);
		applyEmitterRenderTextureMaterial.SetVector("_Offset", offset);
		applyEmitterRenderTextureMaterial.SetFloat("_Scaling", scaling);
		applyEmitterRenderTextureMaterial.SetInt("_Mode", mode);
		Graphics.Blit(null, buffer, applyEmitterRenderTextureMaterial);
		Swap(ref dest, ref buffer);
	}

	public Vector2 GetPixelSize()
	{
		return pixelSize * new Vector2(base.transform.lossyScale.x, base.transform.lossyScale.y);
	}

	private void ShiftTexture(ref RenderTexture buffer, ref RenderTexture dest, Vector2 shift, Vector2 pixelSize)
	{
		shiftTextureMaterial.SetVector("_PixelSize", pixelSize);
		shiftTextureMaterial.SetVector("_Shift", shift);
		shiftTextureMaterial.SetTexture("_Read", buffer);
		Graphics.Blit(buffer, dest, shiftTextureMaterial);
		Swap(ref buffer, ref dest);
	}

	public void ShiftTextures()
	{
		Vector2Int vector2Int = WorldToPixelCoords(Vector3.zero, simulationDownSample);
		Vector2Int vector2Int2 = pixelCoordsOriginLastShift - vector2Int;
		if (Mathf.Abs(vector2Int2.x) != 0 || Mathf.Abs(vector2Int2.y) != 0)
		{
			ShiftTexture(ref density, ref temp4f, vector2Int2, pixelSize * simulationDownSample);
			ShiftTexture(ref temperature, ref temp1f, vector2Int2, pixelSize * simulationDownSample);
			ShiftTexture(ref reaction, ref temp4f, vector2Int2, pixelSize * simulationDownSample);
			ShiftTexture(ref detailNoiseTexture, ref temp1f, vector2Int2, pixelSize * simulationDownSample);
			ShiftTexture(ref velocity, ref temp2fDownSample, vector2Int2, pixelSize * simulationDownSample);
			if (particleAdvection != null && particles != null)
			{
				particleAdvection.SetBuffer(1, "_Particles", particles);
				particleAdvection.SetVector("_AddOffset", -vector2Int2 / gridSizeDownSample);
				particleAdvection.Dispatch(1, 4, 4, 1);
			}
			pixelCoordsOriginLastShift = vector2Int;
			simulationDirty = true;
		}
	}

	private void Update()
	{
		if (shiftTextures)
		{
			ShiftTextures();
		}
		BindRenderTextures();
		RenderEmitterPasses();
		if (simulate == SimulateFunction.UNITY_UPDATE)
		{
			Simulate(0.1f);
		}
		if (enableParticles && particleMaterial != null && particles != null)
		{
			particleMaterial.SetMatrix("_Transform", base.transform.localToWorldMatrix);
			particleMaterial.SetBuffer("particleBuffer", particles);
			int layer = LayerMask.NameToLayer("Default");
			Graphics.DrawProcedural(particleMaterial, new Bounds(Vector3.zero, Vector3.one * 1000000f), MeshTopology.Points, 4096, 1, null, null, ShadowCastingMode.Off, receiveShadows: false, layer);
		}
		UpdateCachedDissipationVectors();
	}

	private void ComputeLighting()
	{
		computeLightMaterial.SetTexture("_Reaction", reaction);
		Graphics.Blit(reaction, lightTex, computeLightMaterial);
		computeLightMaterial.SetTexture("_Reaction", lightTex);
		Graphics.Blit(lightTex, temp4f, computeLightMaterial);
		computeLightMaterial.SetTexture("_Reaction", temp4f);
		Graphics.Blit(temp4f, lightTex, computeLightMaterial);
		computeLightMaterial.SetTexture("_Reaction", lightTex);
		Graphics.Blit(lightTex, temp4f, computeLightMaterial);
		computeLightMaterial.SetTexture("_Reaction", temp4f);
		Graphics.Blit(temp4f, lightTex, computeLightMaterial);
		Shader.SetGlobalTexture("_LightTex", lightTex);
		Shader.SetGlobalVector("_SimX", base.transform.right);
		Shader.SetGlobalVector("_SimY", base.transform.up);
		Shader.SetGlobalVector("_SimPos", base.transform.position);
		Shader.SetGlobalVector("_SimScale", new Vector2(base.transform.lossyScale.x, base.transform.lossyScale.y));
	}

	public void StepForEditor()
	{
		if (main == null)
		{
			main = this;
		}
		if (emitterReactionPass == null || reaction == null)
		{
			Init();
		}
		RenderEmitterPasses();
		Simulate(0.1f);
		BindRenderTextures();
	}

	public void RenderEmitterPasses()
	{
		emitterTemperaturePass.Render();
		emitterReactionPass.Render();
		emitterVelocityPass.Render();
		emitterObstaclePass.Render();
		OnEmittersRendered?.Invoke();
	}

	public void Simulate(float dt)
	{
		OnSimulationStep?.Invoke();
		ApplyObstacleBorder();
		ApplyEmitterTexture(emitterTemperaturePass.RenderTexture, ref temperature, ref temp1f, dt, emitterTempOffset, temperatureEmitterScaling);
		ApplyEmitterTexture(emitterReactionPass.RenderTexture, ref reaction, ref temp3f, dt, reactionOffset, 1f, 1);
		ApplyEmitterTexture(emitterVelocityPass.RenderTexture, ref velocity, ref temp2fDownSample, dt, velocityOffset, velocityEmitterScaling, 1);
		if (viscosity > 0.001f)
		{
			ApplyDiffusion(dt, ref velocity, ref temp2fDownSample, viscosity, pixelSizeDownSample);
		}
		if (fireDiffusion > 0.001f)
		{
			ApplyDiffusion(dt, ref reaction, ref temp3f, fireDiffusion, pixelSize);
		}
		ApplyAdvection(dt, tempDissipation, Vector4.zero, ref temperature, ref temp1f, pixelSize);
		ApplyAdvection(dt, Vector4.one, reactionDecay4, ref reaction, ref temp3f, pixelSize);
		ApplyAdvection(dt, densityDissipation4, Vector4.zero, ref density, ref temp4f, pixelSize);
		ApplyAdvection(dt, Vector4.one, Vector4.zero, ref detailNoiseTexture, ref temp1f, pixelSize);
		ApplyNoise(noiseIntensity, ref detailNoiseTexture, ref temp1f, pixelSize);
		ApplyAdvection(dt, velocityDissipation4, Vector4.zero, ref velocity, ref temp2fDownSample, pixelSize);
		if (densityBuoyancy > 0.001f)
		{
			ApplyBuoyancy(dt);
		}
		ApplyExtinguishmentImpulse(densityAmount);
		ComputeVorticityConfinement(dt);
		ComputeDivergence();
		if (useMultiGridPressureSolver)
		{
			ComputeMultiGridPressure();
		}
		else
		{
			ComputePressure();
		}
		ComputeProjection();
		if (enableParticles && particleAdvection != null && particles != null)
		{
			AdvectParticles();
		}
		simulationDirty = true;
	}

	private void UpdateGlobalShaderParams()
	{
		if (lastSimPos != base.transform.position)
		{
			Shader.SetGlobalVector("_SimPos", base.transform.position);
			lastSimPos = base.transform.position;
		}
		if (lastSimX != base.transform.right)
		{
			Shader.SetGlobalVector("_SimX", base.transform.right);
			lastSimX = base.transform.right;
		}
		if (lastSimY != base.transform.up)
		{
			Shader.SetGlobalVector("_SimY", base.transform.up);
			lastSimY = base.transform.up;
		}
		Vector2 vector = new Vector2(base.transform.lossyScale.x, base.transform.lossyScale.y);
		if (lastSimScale != vector)
		{
			Shader.SetGlobalVector("_SimScale", vector);
			lastSimScale = vector;
		}
	}

	private void BindRenderTextures()
	{
		if (simulationDirty)
		{
			renderMaterial.SetTexture("_Density", density);
			renderMaterial.SetTexture("_Pressure", pressure);
			renderMaterial.SetTexture("_Temperature", temperature);
			renderMaterial.SetTexture("_Reaction", reaction);
			renderMaterial.SetTexture("_Velocity", velocity);
			renderMaterial.SetTexture("_DetailNoise", detailNoiseTexture);
			if (computeLighting)
			{
				ComputeLighting();
				renderMaterial.SetTexture("_LightMap", lightTex);
				Shader.SetGlobalFloat("_EditorMode", 0f);
			}
			else
			{
				clearRenderTextureMaterial.SetColor("_ClearColor", Color.black);
				Graphics.Blit(blitDummy, lightTex, clearRenderTextureMaterial);
				renderMaterial.SetTexture("_LightMap", lightTex);
			}
			UpdateGlobalShaderParams();
			simulationDirty = false;
		}
	}

	private void FixedUpdate()
	{
		if (simulate == SimulateFunction.UNITY_FIXEDUPDATE)
		{
			Simulate(0.1f);
		}
	}

	[ContextMenu("Clean")]
	public void CleanSim()
	{
		Simulate(0f);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Shader.SetGlobalFloat("_EditorMode", 1f);
		}
	}

	private void OnDestroy()
	{
		particles?.Release();
		if (main == this)
		{
			main = null;
		}
		emitterTemperaturePass?.Release();
		emitterReactionPass?.Release();
		emitterVelocityPass?.Release();
	}

	private void OnGUI()
	{
	}

	private void OnDrawGizmos()
	{
		if (emitterReactionPass != null)
		{
			emitterReactionPass.DrawDebugFrustumGizmo(Color.red);
		}
	}
}
