using UnityEngine;

public class SetResolution : MonoBehaviour
{
	public int downSample = 4;

	private void Start()
	{
		Screen.SetResolution(Screen.width / downSample, Screen.height / downSample, fullscreen: true);
	}

	private void Update()
	{
	}
}
