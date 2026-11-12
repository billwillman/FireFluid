using System;
using UnityEngine;

public class CustomDebug : Debug
{
	public static void DrawSphere(Vector3 position, float radius, Color color)
	{
		DrawSphere(position, Quaternion.identity, radius, color, 6);
	}

	public static void DrawSphere(Vector3 position, Quaternion orientation, float radius, Color color, int segments = 4)
	{
		if (segments < 2)
		{
			segments = 2;
		}
		int segments2 = segments * 2;
		float num = 180f / (float)segments;
		for (int i = 0; i < segments; i++)
		{
			DrawCircle(position, orientation * Quaternion.Euler(0f, num * (float)i, 0f), radius, segments2, color);
		}
		Vector3 zero = Vector3.zero;
		float num2 = MathF.PI / (float)segments;
		float num3 = 0f;
		float num4 = 0f;
		for (int j = 1; j < segments; j++)
		{
			num4 = num2 * (float)j;
			zero = orientation * Vector3.up * Mathf.Cos(num4) * radius;
			num3 = Mathf.Sin(num4) * radius;
			DrawCircle(position + zero, orientation * Quaternion.Euler(90f, 0f, 0f), num3, segments2, color);
		}
	}

	public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, int segments, Color color)
	{
		if (!(radius <= 0f) && segments > 0)
		{
			float num = 360f / (float)segments;
			num *= MathF.PI / 180f;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			for (int i = 0; i < segments; i++)
			{
				vector.x = Mathf.Cos(num * (float)i);
				vector.y = Mathf.Sin(num * (float)i);
				vector.z = 0f;
				vector2.x = Mathf.Cos(num * (float)(i + 1));
				vector2.y = Mathf.Sin(num * (float)(i + 1));
				vector2.z = 0f;
				vector *= radius;
				vector2 *= radius;
				vector = rotation * vector;
				vector2 = rotation * vector2;
				vector += position;
				vector2 += position;
				Debug.DrawLine(vector, vector2, color);
			}
		}
	}
}
