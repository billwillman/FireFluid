using UnityEngine;

public class Blink : MonoBehaviour
{
	private Renderer renderer;

	private float waitTime;

	private void Start()
	{
		renderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		waitTime -= Time.deltaTime;
		if (!(waitTime > 0f))
		{
			if (Random.value < 0.005f)
			{
				renderer.enabled = false;
				waitTime = 0.1f;
			}
			else
			{
				renderer.enabled = true;
			}
		}
	}
}
