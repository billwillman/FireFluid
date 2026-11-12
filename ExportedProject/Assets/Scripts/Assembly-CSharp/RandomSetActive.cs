using UnityEngine;

public class RandomSetActive : MonoBehaviour
{
	private void Start()
	{
		if (Random.value < 0.5f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
