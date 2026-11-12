using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	public bool destroyIfTargetIsNull = true;

	public Transform target;

	public Vector3 offset = new Vector3(0f, 1f, 0f);

	private void Update()
	{
		if (target == null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.position = target.position + offset;
		}
	}
}
