using UnityEngine;

public class VelocityOnStart : MonoBehaviour
{
	public Vector3 velocityMin;

	public Vector3 velocityMax;

	private void Start()
	{
		GetComponent<Rigidbody>().linearVelocity += new Vector3(Random.Range(velocityMin.x, velocityMax.x), Random.Range(velocityMin.y, velocityMax.y), Random.Range(velocityMin.z, velocityMax.z));
	}
}
