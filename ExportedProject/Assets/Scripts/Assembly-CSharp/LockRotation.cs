using UnityEngine;

public class LockRotation : MonoBehaviour
{
	public Vector3 eulerAngles = Vector3.zero;

	public void Update()
	{
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
