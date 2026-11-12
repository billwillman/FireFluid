using UnityEngine;

public class SpringExtrapolator : IExtrapolator
{
	public float stiffness = 2f;

	public float damping = 0.1f;

	private bool initialized;

	private Vector3 previousPos3 = Vector3.zero;

	private Vector2 previousPos2 = Vector2.zero;

	private float previousPos1;

	private float previousDt;

	public SpringExtrapolator(float stiffness, float damping)
	{
		this.stiffness = stiffness;
		this.damping = damping;
	}

	public void AddVelocity(Vector3 v)
	{
		previousPos3 -= v;
		previousPos2.x -= v.x;
		previousPos2.y -= v.y;
		previousPos1 -= v.x;
	}

	public Vector3 extrapolate(Vector3 a, Vector3 b, float dt)
	{
		if (!initialized)
		{
			initialized = true;
			previousPos3 = a;
			previousDt = dt;
		}
		Vector3 vector = (a - previousPos3) / previousDt;
		Vector3 vector2 = b - a;
		previousPos3 = a;
		previousDt = dt;
		float num = stiffness * dt * dt;
		if ((double)num > 1.0)
		{
			num = 1f;
		}
		float num2 = damping * dt;
		if ((double)num2 > 1.0)
		{
			num2 = 1f;
		}
		return a + vector2 * num + vector * (1f - num2) * dt;
	}

	public Vector2 extrapolate(Vector2 a, Vector2 b, float dt)
	{
		if (!initialized)
		{
			initialized = true;
			previousPos2 = a;
			previousDt = dt;
		}
		Vector2 vector = (a - previousPos2) / previousDt;
		Vector2 vector2 = b - a;
		previousPos2 = a;
		previousDt = dt;
		float num = stiffness * dt * dt;
		if ((double)num > 1.0)
		{
			num = 1f;
		}
		float num2 = damping * dt;
		if ((double)num2 > 1.0)
		{
			num2 = 1f;
		}
		return a + vector2 * num + vector * (1f - num2) * dt;
	}

	public float extrapolate(float a, float b, float dt)
	{
		if (!initialized)
		{
			initialized = true;
			previousPos1 = a;
			previousDt = dt;
		}
		float num = (a - previousPos1) / previousDt;
		float num2 = b - a;
		previousPos1 = a;
		previousDt = dt;
		float num3 = stiffness * dt * dt;
		if ((double)num3 > 1.0)
		{
			num3 = 1f;
		}
		float num4 = damping * dt;
		if ((double)num4 > 1.0)
		{
			num4 = 1f;
		}
		return a + num2 * num3 + num * (1f - num4) * dt;
	}
}
