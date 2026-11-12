using UnityEngine;

public class ScaleReveal : MonoBehaviour
{
	public float seconds = 1f;

	private Vector3 originalScale;

	private float time;

	private void Start()
	{
		originalScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (time > seconds)
		{
			base.transform.localScale = originalScale;
		}
		else
		{
			base.transform.localScale = originalScale * Mathf.Clamp01(time / seconds);
		}
	}
}
