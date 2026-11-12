using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RenderEmitterPass
{
	private string _name;

	private int _layer;

	private Vector2Int _resolution;

	private Transform _origin;

	private Color _clearColor;

	private Matrix4x4 _lastViewMatrix;

	private Matrix4x4 _lastProjectionMatrix;

	public RenderTexture RenderTexture { get; private set; }

	public RenderEmitterPass(string name, int layer, Vector2Int resolution, Transform originTransform, Color? clearColor = null)
	{
		_name = name;
		_layer = layer;
		_resolution = resolution;
		_origin = originTransform;
		_clearColor = clearColor ?? Color.black;
		InitRenderTexture();
		Clear();
	}

	private void InitRenderTexture()
	{
		RenderTexture = new RenderTexture(_resolution.x, _resolution.y, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture.filterMode = FilterMode.Bilinear;
		RenderTexture.wrapMode = TextureWrapMode.Clamp;
		RenderTexture.name = "EmitterRT_" + _name;
		RenderTexture.Create();
	}

	public void Clear()
	{
		CommandBuffer commandBuffer = CommandBufferPool.Get("Render Emitters - " + _name + " - Clear");
		commandBuffer.SetRenderTarget(RenderTexture);
		commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, _clearColor);
		Graphics.ExecuteCommandBuffer(commandBuffer);
		CommandBufferPool.Release(commandBuffer);
	}

	public void Render()
	{
		List<Renderer> renderersOnLayer = FluidEmitterRenderable.GetRenderersOnLayer(_layer);
		CommandBuffer commandBuffer = CommandBufferPool.Get("Render Emitters - " + _name);
		commandBuffer.SetRenderTarget(RenderTexture);
		commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, _clearColor);
		float num = _origin.lossyScale.y / 2f;
		float num2 = _origin.lossyScale.x / 2f;
		_lastProjectionMatrix = Matrix4x4.Ortho(0f - num2, num2, 0f - num, num, 0f, 100f);
		Vector3 pos = _origin.position + _origin.rotation * new Vector3(0f, 0f, 50f);
		Quaternion rotation = _origin.rotation;
		_lastViewMatrix = Matrix4x4.TRS(pos, rotation, Vector3.one).inverse;
		commandBuffer.SetViewProjectionMatrices(_lastViewMatrix, _lastProjectionMatrix);
		foreach (Renderer item in renderersOnLayer)
		{
			if ((bool)item && item.gameObject.activeInHierarchy && item != null)
			{
				commandBuffer.DrawRenderer(item, item.sharedMaterial);
			}
		}
		commandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
		Graphics.ExecuteCommandBuffer(commandBuffer);
		CommandBufferPool.Release(commandBuffer);
	}

	private void FindRenderersInLayer(LayerMask layerMask, List<Renderer> result)
	{
		foreach (FluidEmitterRenderable item in FluidEmitterRenderable.all)
		{
			if (item == null || !item.gameObject.activeInHierarchy)
			{
				continue;
			}
			foreach (Renderer renderer in item.GetRenderers())
			{
				if (renderer != null && ((1 << renderer.gameObject.layer) & (int)layerMask) != 0)
				{
					result.Add(renderer);
				}
			}
		}
	}

	public void Release()
	{
		if (RenderTexture != null)
		{
			RenderTexture.Release();
			UnityEngine.Object.Destroy(RenderTexture);
			RenderTexture = null;
		}
	}

	public void DrawDebugFrustumGizmo(Color? color = null)
	{
		if (!(_origin == null) && !(_lastProjectionMatrix == Matrix4x4.zero) && !(_lastViewMatrix == Matrix4x4.zero))
		{
			Gizmos.color = color ?? Color.green;
			Matrix4x4 inverse = (_lastProjectionMatrix * _lastViewMatrix).inverse;
			Vector3[] array = new Vector3[8]
			{
				new Vector3(-1f, -1f, -1f),
				new Vector3(1f, -1f, -1f),
				new Vector3(1f, 1f, -1f),
				new Vector3(-1f, 1f, -1f),
				new Vector3(-1f, -1f, 1f),
				new Vector3(1f, -1f, 1f),
				new Vector3(1f, 1f, 1f),
				new Vector3(-1f, 1f, 1f)
			};
			Vector3[] array2 = new Vector3[8];
			for (int i = 0; i < 8; i++)
			{
				array2[i] = inverse.MultiplyPoint(array[i]);
			}
			Gizmos.DrawLine(array2[0], array2[1]);
			Gizmos.DrawLine(array2[1], array2[2]);
			Gizmos.DrawLine(array2[2], array2[3]);
			Gizmos.DrawLine(array2[3], array2[0]);
			Gizmos.DrawLine(array2[4], array2[5]);
			Gizmos.DrawLine(array2[5], array2[6]);
			Gizmos.DrawLine(array2[6], array2[7]);
			Gizmos.DrawLine(array2[7], array2[4]);
			for (int j = 0; j < 4; j++)
			{
				Gizmos.DrawLine(array2[j], array2[j + 4]);
			}
			Vector3 vector = _origin.position + _origin.rotation * new Vector3(0f, 0f, 50f);
			Vector3 forward = _origin.forward;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(vector, vector + forward * 2f);
			Gizmos.DrawLine(vector + Vector3.left * 0.1f, vector + Vector3.right * 0.1f);
			Gizmos.DrawLine(vector + Vector3.up * 0.1f, vector + Vector3.down * 0.1f);
		}
	}
}
