using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(1)]
public class FluidSimCPUMapperAsyncMaxDownsample : MonoBehaviour
{
	private static FluidSimCPUMapperAsyncMaxDownsample instance;

	private FluidSim sim;

	public int downSampleSteps = 2;

	private RenderTexture[] reactionRT;

	private RenderTexture[] densityRT;

	private RenderTexture[] velocityRT;

	private NativeArray<Vector4> reactionVecs;

	private NativeArray<Vector4> reactionVecsBack;

	private NativeArray<Vector4> densityVecs;

	private NativeArray<Vector4> densityVecsBack;

	private NativeArray<Vector2> velocityVecs;

	private NativeArray<Vector2> velocityVecsBack;

	private bool isReactionRequestPending;

	private bool isDensityRequestPending;

	private bool isVelocityRequestPending;

	private bool reactionValid;

	private bool densityValid;

	private bool velocityValid;

	public Shader downsampleShader;

	private Material downsampleMaterial;

	private Texture2D velocityTexture;

	private Color[] pixelColors;

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		sim = GetComponent<FluidSim>();
		int num = sim.GetReaction().width;
		int num2 = sim.GetReaction().height;
		int num3 = sim.GetDensity().width;
		int num4 = sim.GetDensity().height;
		reactionRT = new RenderTexture[downSampleSteps];
		densityRT = new RenderTexture[downSampleSteps];
		velocityRT = new RenderTexture[downSampleSteps];
		for (int i = 0; i < downSampleSteps; i++)
		{
			num /= 2;
			num2 /= 2;
			num3 /= 2;
			num4 /= 2;
			reactionRT[i] = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGBFloat);
			reactionRT[i].useMipMap = false;
			reactionRT[i].autoGenerateMips = false;
			reactionRT[i].enableRandomWrite = false;
			reactionRT[i].Create();
			densityRT[i] = new RenderTexture(num3, num4, 0, RenderTextureFormat.ARGBFloat);
			densityRT[i].useMipMap = false;
			densityRT[i].autoGenerateMips = false;
			densityRT[i].enableRandomWrite = false;
			densityRT[i].Create();
			velocityRT[i] = new RenderTexture(num3, num4, 0, RenderTextureFormat.RGFloat);
			velocityRT[i].useMipMap = false;
			velocityRT[i].autoGenerateMips = false;
			velocityRT[i].enableRandomWrite = false;
			velocityRT[i].Create();
		}
		int length = num * num2;
		int length2 = num3 * num4;
		reactionVecs = new NativeArray<Vector4>(length, Allocator.Persistent);
		reactionVecsBack = new NativeArray<Vector4>(length, Allocator.Persistent);
		densityVecs = new NativeArray<Vector4>(length2, Allocator.Persistent);
		densityVecsBack = new NativeArray<Vector4>(length2, Allocator.Persistent);
		velocityVecs = new NativeArray<Vector2>(length2, Allocator.Persistent);
		velocityVecsBack = new NativeArray<Vector2>(length2, Allocator.Persistent);
		downsampleMaterial = new Material(downsampleShader);
	}

	private void FixedUpdate()
	{
		if (!isReactionRequestPending)
		{
			Graphics.Blit(sim.GetReaction(), reactionRT[0], downsampleMaterial);
			for (int i = 0; i < reactionRT.Length - 1; i++)
			{
				Graphics.Blit(reactionRT[i], reactionRT[i + 1], downsampleMaterial);
			}
			isReactionRequestPending = true;
			AsyncGPUReadback.RequestIntoNativeArray(ref reactionVecsBack, reactionRT[reactionRT.Length - 1], 0, OnReactionReadback);
		}
		if (!isDensityRequestPending)
		{
			Graphics.Blit(sim.GetDensity(), densityRT[0], downsampleMaterial);
			for (int j = 0; j < densityRT.Length - 1; j++)
			{
				Graphics.Blit(densityRT[j], densityRT[j + 1], downsampleMaterial);
			}
			isDensityRequestPending = true;
			AsyncGPUReadback.RequestIntoNativeArray(ref densityVecsBack, densityRT[densityRT.Length - 1], 0, OnDensityReadback);
		}
		if (!isVelocityRequestPending)
		{
			Graphics.Blit(sim.GetVelocity(), velocityRT[0], downsampleMaterial);
			for (int k = 0; k < velocityRT.Length - 1; k++)
			{
				Graphics.Blit(velocityRT[k], velocityRT[k + 1], downsampleMaterial);
			}
			isVelocityRequestPending = true;
			AsyncGPUReadback.RequestIntoNativeArray(ref velocityVecsBack, velocityRT[velocityRT.Length - 1], 0, OnVelocityReadback);
		}
	}

	private void OnReactionReadback(AsyncGPUReadbackRequest request)
	{
		isReactionRequestPending = false;
		if (request.hasError)
		{
			Debug.LogWarning("Reaction GPU readback failed.");
			return;
		}
		NativeArray<Vector4> nativeArray = reactionVecs;
		reactionVecs = reactionVecsBack;
		reactionVecsBack = nativeArray;
		reactionValid = true;
	}

	private void OnDensityReadback(AsyncGPUReadbackRequest request)
	{
		isDensityRequestPending = false;
		if (request.hasError)
		{
			Debug.LogWarning("Density GPU readback failed.");
			return;
		}
		NativeArray<Vector4> nativeArray = densityVecs;
		densityVecs = densityVecsBack;
		densityVecsBack = nativeArray;
		densityValid = true;
	}

	private void OnVelocityReadback(AsyncGPUReadbackRequest request)
	{
		isVelocityRequestPending = false;
		if (request.hasError)
		{
			Debug.LogWarning("Density GPU readback failed.");
			return;
		}
		NativeArray<Vector2> nativeArray = velocityVecs;
		velocityVecs = velocityVecsBack;
		velocityVecsBack = nativeArray;
		velocityValid = true;
	}

	private Vector4 GetValue(NativeArray<Vector4> buffer, RenderTexture rt, Vector3 worldPos)
	{
		Vector2 vector = sim.WorldToTextureCoords(worldPos);
		int num = Mathf.FloorToInt(vector.x * (float)rt.width);
		int num2 = Mathf.FloorToInt(vector.y * (float)rt.height);
		if (num < 0 || num >= rt.width || num2 < 0 || num2 >= rt.height)
		{
			return Vector4.zero;
		}
		int index = num2 * rt.width + num;
		return buffer[index];
	}

	private Vector2 GetValue(NativeArray<Vector2> buffer, RenderTexture rt, Vector3 worldPos)
	{
		Vector2 vector = sim.WorldToTextureCoords(worldPos);
		int num = Mathf.FloorToInt(vector.x * (float)rt.width);
		int num2 = Mathf.FloorToInt(vector.y * (float)rt.height);
		if (num < 0 || num >= rt.width || num2 < 0 || num2 >= rt.height)
		{
			return Vector4.zero;
		}
		int index = num2 * rt.width + num;
		return buffer[index];
	}

	private Vector4 GetValueLocal(NativeArray<Vector4> buffer, RenderTexture rt, Vector2 uv)
	{
		int num = Mathf.FloorToInt(uv.x * (float)rt.width);
		int num2 = Mathf.FloorToInt(uv.y * (float)rt.height);
		if (num < 0 || num >= rt.width || num2 < 0 || num2 >= rt.height)
		{
			return Vector4.zero;
		}
		int index = num2 * rt.width + num;
		return buffer[index];
	}

	private Vector2 GetValueLocal(NativeArray<Vector2> buffer, RenderTexture rt, Vector2 uv)
	{
		int num = Mathf.FloorToInt(uv.x * (float)rt.width);
		int num2 = Mathf.FloorToInt(uv.y * (float)rt.height);
		if (num < 0 || num >= rt.width || num2 < 0 || num2 >= rt.height)
		{
			return Vector4.zero;
		}
		int index = num2 * rt.width + num;
		return buffer[index];
	}

	public Vector4 GetReactionValue(Vector3 worldPos)
	{
		if (!reactionValid)
		{
			return Vector4.zero;
		}
		return GetValue(reactionVecs, reactionRT[reactionRT.Length - 1], worldPos);
	}

	public Vector4 GetDensityValue(Vector3 worldPos)
	{
		if (!densityValid)
		{
			return Vector4.zero;
		}
		return GetValue(densityVecs, densityRT[densityRT.Length - 1], worldPos);
	}

	public Vector2 GetVelocityValue(Vector3 worldPos)
	{
		if (!velocityValid)
		{
			return Vector2.zero;
		}
		return GetValue(velocityVecs, velocityRT[velocityRT.Length - 1], worldPos);
	}

	public Vector4 GetReactionValueLocal(Vector2 uv)
	{
		if (!reactionValid)
		{
			return Vector4.zero;
		}
		return GetValueLocal(reactionVecs, reactionRT[reactionRT.Length - 1], uv);
	}

	public Vector4 GetDensityValueLocal(Vector2 uv)
	{
		if (!densityValid)
		{
			return Vector4.zero;
		}
		return GetValueLocal(densityVecs, densityRT[densityRT.Length - 1], uv);
	}

	public Vector4 GetVelocityValueLocal(Vector2 uv)
	{
		if (!velocityValid)
		{
			return Vector4.zero;
		}
		return GetValueLocal(velocityVecs, velocityRT[velocityRT.Length - 1], uv);
	}

	public Vector3 GetWorldPos(Vector2 uv)
	{
		return sim.TextureToWorldCoords(uv);
	}

	public Vector2 GetUvPos(Vector2 worldPos)
	{
		return sim.WorldToTextureCoords(worldPos);
	}

	public static FluidSimCPUMapperAsyncMaxDownsample Get()
	{
		return instance;
	}

	private void OnDestroy()
	{
		if (isReactionRequestPending || isDensityRequestPending)
		{
			Debug.Log("Waiting for GPU readbacks to complete before cleanup...");
			AsyncGPUReadback.WaitAllRequests();
		}
		if (reactionVecs.IsCreated)
		{
			reactionVecs.Dispose();
		}
		if (reactionVecsBack.IsCreated)
		{
			reactionVecsBack.Dispose();
		}
		if (densityVecs.IsCreated)
		{
			densityVecs.Dispose();
		}
		if (densityVecsBack.IsCreated)
		{
			densityVecsBack.Dispose();
		}
		for (int i = 0; i < reactionRT.Length; i++)
		{
			reactionRT[i].Release();
		}
		for (int j = 0; j < densityRT.Length; j++)
		{
			densityRT[j].Release();
		}
		for (int k = 0; k < velocityRT.Length; k++)
		{
			velocityRT[k].Release();
		}
	}

	private void OnGUI()
	{
	}
}
