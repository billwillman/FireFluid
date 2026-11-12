using UnityEngine;

public class AngularVelocityOnStart : MonoBehaviour
{
	public Vector3 velocityMin;

	public Vector3 velocityMax;

	public bool randomizeStartRotation = true;

	private void Start()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		component.angularVelocity += new Vector3(Random.Range(velocityMin.x, velocityMax.x), Random.Range(velocityMin.y, velocityMax.y), Random.Range(velocityMin.z, velocityMax.z));
		Debug.Log(component.angularVelocity);
		if (randomizeStartRotation)
		{
			component.rotation = Quaternion.Euler(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
			base.transform.localRotation = component.rotation;
		}
	}
}
