using UnityEngine;

public class CameraSettings : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	private void Update()
	{
	}
}
