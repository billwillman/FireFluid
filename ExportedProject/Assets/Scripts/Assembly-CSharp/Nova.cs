using System;
using System.Collections.Generic;
using UnityEngine;

public class Nova : MonoBehaviour
{
	public int numberOfPositions = 32;

	public float currentRadius = 1f;

	public float radiusRateOfChange = 8f;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private Vector3[] positions;

	public AnimationCurve widthOverTime = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public float widthMultiplier = 1f;

	public float lifeTime = 2f;

	public float currentLifeTime;

	public void SetLineRendererPositions()
	{
		if (positions == null || positions.Length != numberOfPositions)
		{
			positions = new Vector3[numberOfPositions];
		}
		for (int i = 0; i < numberOfPositions; i++)
		{
			positions[i].y = 0f;
			float f = MathF.PI * 2f * (float)i / (float)numberOfPositions;
			positions[i].x = Mathf.Sin(f) * currentRadius + base.transform.position.x;
			positions[i].z = Mathf.Cos(f) * currentRadius + base.transform.position.z;
		}
		foreach (LineRenderer lineRenderer in lineRenderers)
		{
			lineRenderer.positionCount = numberOfPositions;
			lineRenderer.SetPositions(positions);
		}
	}

	private void Start()
	{
		GetComponentsInChildren(lineRenderers);
		SetLineRendererPositions();
	}

	private void Update()
	{
		currentRadius += radiusRateOfChange * Time.deltaTime;
		foreach (LineRenderer lineRenderer in lineRenderers)
		{
			lineRenderer.startWidth = widthOverTime.Evaluate(currentLifeTime / lifeTime) * widthMultiplier;
			lineRenderer.endWidth = lineRenderer.startWidth;
		}
		SetLineRendererPositions();
		currentLifeTime += Time.deltaTime;
		if (currentLifeTime > lifeTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
