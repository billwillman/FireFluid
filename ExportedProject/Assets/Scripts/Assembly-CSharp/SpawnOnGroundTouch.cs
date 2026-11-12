using UnityEngine;

public class SpawnOnGroundTouch : MonoBehaviour
{
	public GameObject prefab;

	private float groundLevelY;

	private float previousPosY;

	private const float epsilon = 0.01f;

	private void Start()
	{
		groundLevelY = base.transform.position.y;
		previousPosY = base.transform.position.y;
	}

	private void Update()
	{
		if (previousPosY > groundLevelY + 0.01f && base.transform.position.y <= groundLevelY + 0.01f)
		{
			Object.Instantiate(prefab, base.transform.position, Quaternion.identity);
		}
		previousPosY = base.transform.position.y;
	}
}
