using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshVolumeFromCollider : MonoBehaviour
{
	[Tooltip("NavMesh area name to apply (e.g. Not Walkable)")]
	public string areaName = "Not Walkable";

	private void Awake()
	{
		GenerateVolumes();
	}

	private void GenerateVolumes()
	{
		int areaFromName = NavMesh.GetAreaFromName(areaName);
		if (areaFromName < 0)
		{
			Debug.LogError("NavMesh area '" + areaName + "' not found.");
			return;
		}
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.isTrigger)
			{
				continue;
			}
			NavMeshModifierVolume navMeshModifierVolume = collider.GetComponent<NavMeshModifierVolume>();
			if (navMeshModifierVolume == null)
			{
				navMeshModifierVolume = collider.gameObject.AddComponent<NavMeshModifierVolume>();
			}
			navMeshModifierVolume.area = areaFromName;
			if (collider is BoxCollider boxCollider)
			{
				navMeshModifierVolume.center = boxCollider.center;
				navMeshModifierVolume.size = boxCollider.size;
			}
			else if (collider is SphereCollider sphereCollider)
			{
				float num = sphereCollider.radius * 2f;
				navMeshModifierVolume.center = sphereCollider.center;
				navMeshModifierVolume.size = Vector3.one * num;
			}
			else if (collider is CapsuleCollider capsuleCollider)
			{
				float num2 = capsuleCollider.radius * 2f;
				Vector3 size = Vector3.zero;
				switch (capsuleCollider.direction)
				{
				case 0:
					size = new Vector3(capsuleCollider.height, num2, num2);
					break;
				case 1:
					size = new Vector3(num2, capsuleCollider.height, num2);
					break;
				case 2:
					size = new Vector3(num2, num2, capsuleCollider.height);
					break;
				}
				navMeshModifierVolume.center = capsuleCollider.center;
				navMeshModifierVolume.size = size;
			}
			else
			{
				navMeshModifierVolume.center = collider.transform.InverseTransformPoint(collider.bounds.center);
				navMeshModifierVolume.size = collider.bounds.size;
			}
			navMeshModifierVolume.size += Vector3.up * 1f;
		}
	}
}
