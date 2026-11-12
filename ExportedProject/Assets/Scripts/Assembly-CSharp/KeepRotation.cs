using UnityEngine;

[DefaultExecutionOrder(100)]
public class KeepRotation : MonoBehaviour
{
	private Quaternion baseRotation;

	private void Start()
	{
		baseRotation = base.transform.rotation;
	}

	private void Update()
	{
		base.transform.rotation = baseRotation;
	}
}
