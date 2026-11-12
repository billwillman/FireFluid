using UnityEngine;

public class AdjustRendererDensity : MonoBehaviour
{
	private FluidSimCPUMapperAsync cpuMapper;

	private Material renderer;

	public float averageReactionValue;

	private void Start()
	{
		renderer = GetComponent<Renderer>().sharedMaterial;
		cpuMapper = GetComponent<FluidSimCPUMapperAsync>();
	}

	private void Update()
	{
		int num = 32;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			Vector4 reactionValueLocal = cpuMapper.GetReactionValueLocal(new Vector2(Random.value, Random.value));
			num2 += reactionValueLocal.x + reactionValueLocal.y + reactionValueLocal.z;
		}
		num2 *= 128f / (float)num;
		averageReactionValue = Mathf.Lerp(averageReactionValue, num2, Time.deltaTime * 16f);
	}
}
