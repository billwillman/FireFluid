using UnityEngine;

public class DrawBounds : MonoBehaviour
{
	public bool global = true;

	public Color color_sphere = new Color(0f, 0f, 0f, 0.5f);

	public Color color_bounds = new Color(1f, 1f, 0f, 0.5f);

	public bool Hierarchical = true;

	public bool Disable;

	public void OnDrawGizmos()
	{
		Bounds bounds = default(Bounds);
		if (Hierarchical)
		{
			bounds = base.gameObject.GetComponentsInChildren<Renderer>().ComputeBounds(global);
		}
		else
		{
			Renderer component = base.gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				bounds = ((!global) ? component.localBounds : component.bounds);
			}
		}
		if (global)
		{
			Gizmos.color = color_bounds;
			Gizmos.DrawCube(bounds.center, bounds.size);
			Gizmos.color = color_sphere;
			Gizmos.DrawSphere(bounds.center, bounds.size.magnitude * 0.1f);
		}
		else
		{
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.color = color_bounds;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(bounds.center, bounds.size);
			Gizmos.color = color_sphere;
			Gizmos.DrawSphere(bounds.center, bounds.size.magnitude * 0.1f);
			Gizmos.matrix = matrix;
		}
	}
}
