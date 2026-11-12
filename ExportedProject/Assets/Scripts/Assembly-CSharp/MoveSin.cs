using System;
using UnityEngine;

public class MoveSin : MonoBehaviour
{
	public float frequency = 0.2f;

	public Vector3 amplitude = new Vector3(8f, 0f, 0f);

	private Vector3 startingPoint;

	private void Start()
	{
		startingPoint = base.transform.position;
	}

	private void Update()
	{
		base.transform.position = Vector3.Lerp(startingPoint, startingPoint + amplitude, Mathf.Sin(Time.time * frequency * MathF.PI * 2f) * 0.5f + 0.5f);
	}
}
