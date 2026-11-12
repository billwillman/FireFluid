using UnityEngine;

public class LinearInterpolator : IInterpolator
{
	public Vector3 interpolate(Vector3 a, Vector3 b, float t)
	{
		return a * (1f - t) + b * t;
	}

	public Vector2 interpolate(Vector2 a, Vector2 b, float t)
	{
		return a * (1f - t) + b * t;
	}

	public float interpolate(float a, float b, float t)
	{
		return a * (1f - t) + b * t;
	}
}
