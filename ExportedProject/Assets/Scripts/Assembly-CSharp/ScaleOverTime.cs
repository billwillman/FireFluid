using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
	public float speed = 0.1f;

	public float targetScaleFactor;

	private Vector3 scale;

	private void Start()
	{
		scale = base.transform.localScale;
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, scale * targetScaleFactor, Time.deltaTime * speed);
	}
}
