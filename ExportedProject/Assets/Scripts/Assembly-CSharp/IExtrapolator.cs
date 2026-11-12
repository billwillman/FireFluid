using UnityEngine;

public interface IExtrapolator
{
	float extrapolate(float a, float b, float dt);

	Vector2 extrapolate(Vector2 a, Vector2 b, float dt);

	Vector3 extrapolate(Vector3 a, Vector3 b, float dt);
}
