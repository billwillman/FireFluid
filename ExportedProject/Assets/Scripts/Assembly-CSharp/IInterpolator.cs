using UnityEngine;

public interface IInterpolator
{
	float interpolate(float a, float b, float t);

	Vector2 interpolate(Vector2 a, Vector2 b, float t);

	Vector3 interpolate(Vector3 a, Vector3 b, float t);
}
