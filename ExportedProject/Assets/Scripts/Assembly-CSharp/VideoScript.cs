using UnityEngine;

public class VideoScript : MonoBehaviour
{
	public GameObject lightning;

	public GameObject impact;

	public int count = 1;

	public float spawnFrequency = 1f;

	private float timer;

	private void Start()
	{
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer < 0f)
		{
			for (int i = 0; i < count; i++)
			{
				LightningStrike component = Object.Instantiate(lightning).GetComponent<LightningStrike>();
				component.start = Vector3.up * 64f;
				component.end = Vector3.zero;
				timer = spawnFrequency;
			}
		}
	}
}
