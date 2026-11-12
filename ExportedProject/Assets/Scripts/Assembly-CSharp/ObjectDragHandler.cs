using UnityEngine;

public class ObjectDragHandler : MonoBehaviour
{
	public Camera cam;

	private DragObject dragObject;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (raycastHit2D.collider != null && (bool)raycastHit2D.collider.GetComponent<DragObject>())
			{
				dragObject = raycastHit2D.collider.GetComponent<DragObject>();
			}
		}
		if (!Input.GetMouseButton(0))
		{
			dragObject = null;
		}
		if (dragObject != null)
		{
			Vector3 position = cam.ScreenToWorldPoint(Input.mousePosition);
			position.z = dragObject.transform.position.z;
			dragObject.transform.position = position;
		}
	}
}
