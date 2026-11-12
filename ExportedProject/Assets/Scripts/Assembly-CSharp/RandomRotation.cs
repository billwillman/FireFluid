using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	public Vector3 rotationMin;

	public Vector3 rotationMax;

	public bool randomizeStartRotation = true;

	private Vector3 rotation;

	private void Start()
	{
		rotation = new Vector3(Random.Range(rotationMin.x, rotationMax.x), Random.Range(rotationMin.y, rotationMax.y), Random.Range(rotationMin.z, rotationMax.z));
		if (randomizeStartRotation)
		{
			base.transform.localRotation = Quaternion.Euler(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
		}
	}

	private void Update()
	{
		base.transform.localRotation = Quaternion.Euler(rotation * Time.deltaTime) * base.transform.localRotation;
	}
}
