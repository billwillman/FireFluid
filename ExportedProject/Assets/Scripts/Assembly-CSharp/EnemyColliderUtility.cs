using UnityEngine;
using UnityEngine.AI;

public static class EnemyColliderUtility
{
	public static void RestrictNavAgentRadius(GameObject obj, float maxRadius)
	{
		NavMeshAgent component = obj.GetComponent<NavMeshAgent>();
		if (!(component == null) && component.radius > maxRadius)
		{
			component.radius = maxRadius;
		}
	}

	private static float GetHorizontalRadius(Collider col, Vector3 center)
	{
		if (col is SphereCollider sphereCollider)
		{
			Vector3 vector = col.transform.TransformPoint(sphereCollider.center);
			float num = Mathf.Max(col.transform.lossyScale.x, col.transform.lossyScale.z);
			return Vector2.Distance(new Vector2(center.x, center.z), new Vector2(vector.x, vector.z)) + sphereCollider.radius * num;
		}
		if (col is CapsuleCollider capsuleCollider)
		{
			Vector3 vector2 = col.transform.TransformPoint(capsuleCollider.center);
			float num2 = Mathf.Max(col.transform.lossyScale.x, col.transform.lossyScale.z);
			return Vector2.Distance(new Vector2(center.x, center.z), new Vector2(vector2.x, vector2.z)) + capsuleCollider.radius * num2;
		}
		Bounds bounds = col.bounds;
		Vector3 extents = bounds.extents;
		return Vector2.Distance(new Vector2(center.x, center.z), new Vector2(bounds.center.x, bounds.center.z)) + Mathf.Max(extents.x, extents.z);
	}

	public static void RestrictColliderRadius(GameObject obj, float maxRadius)
	{
		Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		Bounds bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			bounds.Encapsulate(componentsInChildren[i].bounds);
		}
		float num = 0f;
		Vector3 center = bounds.center;
		Collider[] array = componentsInChildren;
		for (int j = 0; j < array.Length; j++)
		{
			float horizontalRadius = GetHorizontalRadius(array[j], center);
			if (horizontalRadius > num)
			{
				num = horizontalRadius;
			}
		}
		if (num <= 0f)
		{
			return;
		}
		float num2 = maxRadius / num;
		if (num2 >= 1f)
		{
			return;
		}
		array = componentsInChildren;
		foreach (Collider collider in array)
		{
			if (collider is CapsuleCollider capsuleCollider)
			{
				capsuleCollider.radius *= num2;
			}
			else if (collider is SphereCollider sphereCollider)
			{
				sphereCollider.radius *= num2;
			}
			else if (collider is BoxCollider boxCollider)
			{
				boxCollider.size *= num2;
			}
		}
	}
}
