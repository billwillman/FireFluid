using UnityEngine;

public static class Vector3Extension
{
	private const uint p1 = 73856093u;

	private const uint p2 = 19349663u;

	private const uint p3 = 83492791u;

	public static uint GetSpatialHash(this Vector3 p)
	{
		return ((uint)p.x * 73856093) ^ ((uint)p.y * 19349663) ^ ((uint)p.x * 83492791);
	}
}
