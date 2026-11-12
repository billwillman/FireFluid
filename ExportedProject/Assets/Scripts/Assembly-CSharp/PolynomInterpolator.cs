using UnityEngine;

public class PolynomInterpolator : IInterpolator
{
	public float exponent = 2f;

	public PolynomInterpolator(float exponent)
	{
		this.exponent = exponent;
	}

	public Vector3 interpolate(Vector3 a, Vector3 b, float t)
	{
		float num = Mathf.Pow(t, exponent);
		return a * (1f - num) + b * num;
	}

	public Vector2 interpolate(Vector2 a, Vector2 b, float t)
	{
		float num = Mathf.Pow(t, exponent);
		return a * (1f - num) + b * num;
	}

	public float interpolate(float a, float b, float t)
	{
		float num = Mathf.Pow(t, exponent);
		return a * (1f - num) + b * num;
	}
}
