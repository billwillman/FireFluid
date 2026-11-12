using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomImpuls : MonoBehaviour
{
	public float forceIntensity = 12f;

	public float torqueIntensity = 4f;

	private Rigidbody rigidbody;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(Random.onUnitSphere * forceIntensity * Random.value, ForceMode.Impulse);
		rigidbody.AddTorque(Random.onUnitSphere * torqueIntensity * Random.value, ForceMode.Impulse);
	}
}
