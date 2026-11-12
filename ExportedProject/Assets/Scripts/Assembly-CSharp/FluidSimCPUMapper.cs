using UnityEngine;

[DefaultExecutionOrder(1)]
public class FluidSimCPUMapper : MonoBehaviour
{
	private static FluidSimCPUMapper instance;

	private FluidSim sim;

	public FetchMode fetchMode;

	public int downSample = 4;

	private RenderTexture reaction;

	private RenderTexture density;

	private Texture2D reactionCPU;

	private Texture2D densityCPU;

	private int frame;

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		sim = GetComponent<FluidSim>();
		reaction = new RenderTexture(sim.GetReaction().width / downSample, sim.GetReaction().height / downSample, 0, sim.GetReaction().format);
		density = new RenderTexture(sim.GetDensity().width / downSample, sim.GetDensity().height / downSample, 0, sim.GetDensity().format);
		reactionCPU = new Texture2D(reaction.width, reaction.height, TextureFormat.ARGB32, mipChain: false);
		densityCPU = new Texture2D(reaction.width, reaction.height, TextureFormat.ARGB32, mipChain: false);
	}

	private void FixedUpdate()
	{
		frame++;
		Graphics.Blit(sim.GetReaction(), reaction);
		Graphics.Blit(sim.GetDensity(), density);
		Rect rect = new Rect(0f, 0f, 1f, 1f);
		switch (fetchMode)
		{
		case FetchMode.Half:
			rect = new Rect(0f, (float)(frame % 2) * 0.5f, 1f, 0.5f);
			break;
		case FetchMode.Quad:
			rect.x = (float)(frame % 2) * 0.5f;
			rect.y = (float)(frame / 2 % 2) * 0.5f;
			rect.width = 0.5f;
			rect.height = 0.5f;
			break;
		}
		RenderTexture.active = reaction;
		reactionCPU.ReadPixels(new Rect(rect.x * (float)reaction.width, rect.y * (float)reaction.height, rect.width * (float)reaction.width, rect.height * (float)reaction.height), (int)(rect.x * (float)reaction.width), (int)(rect.y * (float)reaction.height), recalculateMipMaps: false);
		reactionCPU.Apply(updateMipmaps: false);
		RenderTexture.active = density;
		densityCPU.ReadPixels(new Rect(rect.x * (float)density.width, rect.y * (float)density.height, rect.width * (float)density.width, rect.height * (float)density.height), (int)(rect.x * (float)density.width), (int)(rect.y * (float)density.height), recalculateMipMaps: false);
		densityCPU.Apply(updateMipmaps: false);
	}

	public Vector4 GetReactionValue(Vector3 worldPos)
	{
		Vector2 vector = sim.WorldToTextureCoords(worldPos);
		Vector2Int vector2Int = new Vector2Int(Mathf.FloorToInt(vector.x * (float)reaction.width), Mathf.FloorToInt(vector.y * (float)reaction.height));
		return reactionCPU.GetPixel(vector2Int.x, vector2Int.y);
	}

	public Vector4 GetDensityValue(Vector3 worldPos)
	{
		Vector2 vector = sim.WorldToTextureCoords(worldPos);
		Vector2Int vector2Int = new Vector2Int(Mathf.FloorToInt(vector.x * (float)reaction.width), Mathf.FloorToInt(vector.y * (float)reaction.height));
		return densityCPU.GetPixel(vector2Int.x, vector2Int.y);
	}

	public static FluidSimCPUMapper Get()
	{
		return instance;
	}
}
