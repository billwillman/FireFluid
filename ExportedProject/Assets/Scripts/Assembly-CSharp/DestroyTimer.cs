using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
	public float time = 2f;

	private void Update()
	{
		time -= Time.deltaTime;
		if (time < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
