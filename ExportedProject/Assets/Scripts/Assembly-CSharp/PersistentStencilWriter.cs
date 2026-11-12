using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PersistentStencilWriter : MonoBehaviour
{
	[Tooltip("Material used only to write stencil values. Should not modify original material.")]
	public Material stencilMaterial;

	private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

	private List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();

	private Dictionary<SkinnedMeshRenderer, Mesh> bakedMeshes = new Dictionary<SkinnedMeshRenderer, Mesh>();

	private void LateUpdate()
	{
		meshRenderers.Clear();
		skinnedRenderers.Clear();
		meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
		skinnedRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
		foreach (MeshRenderer meshRenderer in meshRenderers)
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if (!(component?.sharedMesh == null))
			{
				Graphics.DrawMesh(component.sharedMesh, meshRenderer.transform.localToWorldMatrix, stencilMaterial, meshRenderer.gameObject.layer);
			}
		}
		foreach (SkinnedMeshRenderer skinnedRenderer in skinnedRenderers)
		{
			if (!bakedMeshes.TryGetValue(skinnedRenderer, out var value))
			{
				value = new Mesh();
				bakedMeshes[skinnedRenderer] = value;
			}
			skinnedRenderer.BakeMesh(value);
			Graphics.DrawMesh(value, skinnedRenderer.transform.localToWorldMatrix, stencilMaterial, skinnedRenderer.gameObject.layer);
		}
	}
}
