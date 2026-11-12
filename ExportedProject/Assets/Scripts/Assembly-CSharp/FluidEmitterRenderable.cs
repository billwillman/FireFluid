using System.Collections.Generic;
using UnityEngine;

public class FluidEmitterRenderable : MonoBehaviour
{
	public static List<FluidEmitterRenderable> all = new List<FluidEmitterRenderable>();

	private List<Renderer> renderers = new List<Renderer>();

	private static Dictionary<int, List<Renderer>> renderersOnLayer = new Dictionary<int, List<Renderer>>();

	public virtual void Awake()
	{
		all.Add(this);
	}

	public virtual void Start()
	{
		GetComponentsInChildren(renderers);
		foreach (Renderer renderer in renderers)
		{
			List<Renderer> list;
			if (!renderersOnLayer.ContainsKey(renderer.gameObject.layer))
			{
				list = new List<Renderer>();
				renderersOnLayer.Add(renderer.gameObject.layer, list);
			}
			else
			{
				list = renderersOnLayer[renderer.gameObject.layer];
			}
			list.Add(renderer);
		}
	}

	public List<Renderer> GetRenderers()
	{
		return renderers;
	}

	public static List<Renderer> GetRenderersOnLayer(int layerId)
	{
		if (renderersOnLayer.ContainsKey(layerId))
		{
			return renderersOnLayer[layerId];
		}
		return new List<Renderer>();
	}

	public virtual void OnDestroy()
	{
		all.Remove(this);
		foreach (Renderer renderer in renderers)
		{
			if (renderersOnLayer.ContainsKey(renderer.gameObject.layer))
			{
				renderersOnLayer[renderer.gameObject.layer].Remove(renderer);
			}
		}
	}
}
