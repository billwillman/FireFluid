using System;
using UnityEngine;

public class GhostLine : MonoBehaviour
{
	private GameObject connectionGraphic;

	private LineRenderer lr;

	private Vector3[] points;

	public Vector3 start;

	public Vector3 end;

	public Color startColor;

	public Color endColor;

	private void Awake()
	{
		CreateConnectionGraphic();
	}

	private void Update()
	{
		if (connectionGraphic.activeInHierarchy)
		{
			UpdateConnectionGraphic();
		}
	}

	private void CreateConnectionGraphic()
	{
		connectionGraphic = new GameObject("Bond");
		connectionGraphic.transform.SetParent(base.transform);
		connectionGraphic.transform.position = Vector3.zero;
		connectionGraphic.AddComponent<ExcludeBounds>();
		lr = connectionGraphic.AddComponent<LineRenderer>();
		lr.startWidth = 1f;
		lr.endWidth = lr.startWidth;
		lr.startColor = startColor;
		lr.endColor = endColor;
		lr.positionCount = 16;
		lr.loop = false;
		Material material = new Material(Shader.Find("Custom/GhostLine"));
		lr.material = material;
		lr.numCapVertices = 3;
		points = new Vector3[lr.positionCount];
		UpdateConnectionGraphic();
	}

	private void UpdateConnectionGraphic()
	{
		float num = 2f;
		for (int i = 0; i < points.Length; i++)
		{
			float num2 = 1f * (float)i / (float)(points.Length - 1);
			points[i] = start * (1f - num2) + end * num2;
			float num3 = Mathf.Pow(Mathf.Abs(Mathf.Sin(num2 * MathF.PI)), 0.5f);
			points[i] += Vector3.up * num3 * num;
			points[i] += Vector3.up * Mathf.Sin(num2 * 6f - 6f * Time.time) * 0.2f * num3;
			points[i] += Vector3.up * Mathf.Sin(num2 * 17f - 14f * Time.time) * 0.05f * num3;
			points[i] += Vector3.up * Mathf.Sin(num2 * 8f + 3f * Time.time) * 0.1f * num3;
		}
		lr.SetPositions(points);
		lr.startColor = startColor;
		lr.endColor = endColor;
	}

	public void Show(bool show = true)
	{
		if (show)
		{
			connectionGraphic.SetActive(value: true);
		}
		else
		{
			connectionGraphic.SetActive(value: false);
		}
	}
}
