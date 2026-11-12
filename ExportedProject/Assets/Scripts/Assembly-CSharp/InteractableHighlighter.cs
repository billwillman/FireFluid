using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlighter : MonoBehaviour
{
	[SerializeField]
	private Material interactableHighlightMaterial;

	private readonly Dictionary<SkinnedMeshRenderer, Mesh> bakedMeshes = new Dictionary<SkinnedMeshRenderer, Mesh>();

	private readonly List<MeshFilter> tempMeshFilters = new List<MeshFilter>(32);

	private readonly List<SkinnedMeshRenderer> tempSkinned = new List<SkinnedMeshRenderer>(32);

	public void DrawInteractableHighlight(MonoBehaviour interactable)
	{
		if (interactableHighlightMaterial == null || interactable == null)
		{
			return;
		}
		tempMeshFilters.Clear();
		interactable.GetComponentsInChildren(tempMeshFilters);
		foreach (MeshFilter tempMeshFilter in tempMeshFilters)
		{
			if (!(tempMeshFilter.sharedMesh == null) && tempMeshFilter.sharedMesh.vertexCount > 4)
			{
				Graphics.DrawMesh(tempMeshFilter.sharedMesh, tempMeshFilter.transform.localToWorldMatrix, interactableHighlightMaterial, tempMeshFilter.gameObject.layer);
			}
		}
		tempSkinned.Clear();
		interactable.GetComponentsInChildren(tempSkinned);
		foreach (SkinnedMeshRenderer item in tempSkinned)
		{
			if (!(item.sharedMesh == null))
			{
				if (!bakedMeshes.TryGetValue(item, out var value))
				{
					value = new Mesh
					{
						name = item.name + "_bakedMesh",
						indexFormat = item.sharedMesh.indexFormat
					};
					bakedMeshes.Add(item, value);
				}
				item.BakeMesh(value);
				Graphics.DrawMesh(value, item.localToWorldMatrix, interactableHighlightMaterial, item.gameObject.layer);
			}
		}
		CleanupDestroyedRenderers();
	}

	private void CleanupDestroyedRenderers()
	{
		List<SkinnedMeshRenderer> list = null;
		foreach (KeyValuePair<SkinnedMeshRenderer, Mesh> bakedMesh in bakedMeshes)
		{
			if (bakedMesh.Key == null)
			{
				if (list == null)
				{
					list = new List<SkinnedMeshRenderer>();
				}
				list.Add(bakedMesh.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (SkinnedMeshRenderer item in list)
		{
			bakedMeshes.Remove(item);
		}
	}
}
