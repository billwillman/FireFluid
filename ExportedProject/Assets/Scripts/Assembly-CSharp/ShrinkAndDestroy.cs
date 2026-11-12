using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
	public float shrinkAfterSeconds = 8f;

	public float destroyAfterSeconds = 10f;

	private Vector3 originalScale;

	private float time;

	private void Start()
	{
		originalScale = base.transform.localScale;
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (time > shrinkAfterSeconds)
		{
			base.transform.localScale = originalScale * Mathf.Clamp01(1f - (time - shrinkAfterSeconds) / (destroyAfterSeconds - shrinkAfterSeconds));
		}
		if (time >= destroyAfterSeconds)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
