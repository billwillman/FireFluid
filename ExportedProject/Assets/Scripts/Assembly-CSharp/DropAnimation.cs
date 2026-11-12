using UnityEngine;

public class DropAnimation : MonoBehaviour
{
	private float dropHeight = 4f;

	private float power = 2f;

	private float speed = 2f;

	private float currentHeight;

	private float currentHeight0;

	private float time;

	public float randomOffset;

	private Vector3 offset;

	public float DropCurve(float t)
	{
		return (1f - Mathf.Abs(Mathf.Pow(t * 2f - 1f, power))) * dropHeight;
	}

	private void Start()
	{
		currentHeight = DropCurve(0f);
		currentHeight0 = currentHeight;
		offset = Random.insideUnitSphere * randomOffset;
		offset.y = 0f;
	}

	private void Update()
	{
		time += Time.deltaTime * speed;
		currentHeight = DropCurve(Mathf.Clamp01(time));
		base.transform.position += Vector3.up * (currentHeight - currentHeight0) + offset * Time.deltaTime;
		currentHeight0 = currentHeight;
		if (time > 1f)
		{
			Item component = GetComponent<Item>();
			if ((bool)component)
			{
				SoundEffectManager.Play(component.dropSound, base.transform.position);
			}
			Object.Destroy(this);
		}
	}
}
