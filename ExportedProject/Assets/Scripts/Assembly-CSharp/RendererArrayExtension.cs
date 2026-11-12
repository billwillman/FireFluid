using UnityEngine;

public static class RendererArrayExtension
{
	public static Bounds ComputeBounds(this Renderer[] renderers, bool global = true)
	{
		bool flag = false;
		Bounds result = default(Bounds);
		foreach (Renderer renderer in renderers)
		{
			if (renderer is SpriteRenderer || renderer is ParticleSystemRenderer || renderer is TrailRenderer || renderer is LineRenderer || (bool)renderer.GetComponent<ExcludeBounds>())
			{
				continue;
			}
			if (global)
			{
				if (!flag)
				{
					result = renderer.bounds;
					flag = true;
				}
				else
				{
					result.Encapsulate(renderer.bounds);
				}
			}
			else if (!flag)
			{
				result = renderer.localBounds;
				flag = true;
			}
			else
			{
				result.Encapsulate(renderer.localBounds);
			}
		}
		return result;
	}
}
