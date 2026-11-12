using System.Collections.Generic;
using UnityEngine;

public class FluidEmitterRenderable : MonoBehaviour
{
	public static List<FluidEmitterRenderable> all = new List<FluidEmitterRenderable>();

	private List<Renderer> renderers = new List<Renderer>();

	private static Dictionary<int, List<Renderer>> renderersOnLayer = new Dictionary<int, List<Renderer>>();

	public virtual void Awake()
	{
		if (!all.Contains(this))
		{
			all.Add(this);
		}
	}

	public virtual void OnEnable()
	{
		if (!all.Contains(this))
		{
			all.Add(this);
		}
		RefreshRenderers();
	}

	public virtual void Start()
	{
		RefreshRenderers();
	}

	public void RefreshRenderers()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Renderer> listed;
			if (renderer != null && renderersOnLayer.TryGetValue(renderer.gameObject.layer, out listed))
			{
				listed.Remove(renderer);
			}
		}
		renderers.Clear();
		GetComponentsInChildren(true, renderers);
		foreach (Renderer renderer in renderers)
		{
			if (renderer == null)
			{
				continue;
			}
			List<Renderer> list;
			if (!renderersOnLayer.TryGetValue(renderer.gameObject.layer, out list))
			{
				list = new List<Renderer>();
				renderersOnLayer.Add(renderer.gameObject.layer, list);
			}
			if (!list.Contains(renderer))
			{
				list.Add(renderer);
			}
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
