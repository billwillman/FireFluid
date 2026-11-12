using UnityEngine;

[DefaultExecutionOrder(-1)]
public class FluidSimFollowTransform : MonoBehaviour
{
	private FluidSim fluidSim;

	public Transform target;

	public Vector2 offset;

	private void Start()
	{
		fluidSim = GetComponent<FluidSim>();
	}

	private void Update()
	{
		Vector3 vector = new Vector3(Vector3.Dot(base.transform.right, target.transform.position), Vector3.Dot(base.transform.up, target.transform.position), Vector3.Dot(base.transform.forward, base.transform.position));
		Vector2 pixelSize = fluidSim.GetPixelSize();
		Vector2 vector2 = pixelSize * fluidSim.simulationDownSample * 16f;
		vector.x = (float)Mathf.FloorToInt(vector.x / vector2.x) * vector2.x + offset.x + pixelSize.x * 0.5f;
		vector.y = (float)Mathf.FloorToInt(vector.y / vector2.y) * vector2.y + offset.y + pixelSize.y * 0.5f;
		Vector3 position = base.transform.right * vector.x + base.transform.up * vector.y + base.transform.forward * vector.z;
		base.transform.position = position;
	}
}
