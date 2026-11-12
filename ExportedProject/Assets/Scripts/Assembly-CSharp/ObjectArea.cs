using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArea : MonoBehaviour
{
	public float radius = 1f;

	public float chance = 0.5f;

	public List<ObjectAreaPrefab> objects = new List<ObjectAreaPrefab>();

	public Vector2 randomScale = new Vector2(1f, 1f);

	public Vector2 randomRotation = new Vector2(0f, 360f);

	public bool spawnIfFacingAwayFromCamera = true;

	private void Start()
	{
		if (spawnIfFacingAwayFromCamera || !(Vector3.Dot(Camera.main.transform.forward, base.transform.up) > 0f))
		{
			InstantiateObject();
		}
	}

	private void InstantiateObject()
	{
		if (!(UnityEngine.Random.value < chance))
		{
			return;
		}
		float num = 0f;
		foreach (ObjectAreaPrefab @object in objects)
		{
			num += @object.weight;
		}
		if (num <= 0f)
		{
			return;
		}
		float num2 = UnityEngine.Random.value * num;
		GameObject gameObject = null;
		foreach (ObjectAreaPrefab object2 in objects)
		{
			if (num2 < object2.weight)
			{
				gameObject = object2.prefab;
				break;
			}
			num2 -= object2.weight;
		}
		if (!(gameObject == null))
		{
			GameObject obj = UnityEngine.Object.Instantiate(gameObject);
			obj.transform.parent = base.transform;
			obj.transform.rotation = base.transform.rotation;
			Vector2 vector = UnityEngine.Random.insideUnitCircle * radius;
			Vector3 vector2 = base.transform.right * vector.x + base.transform.forward * vector.y;
			obj.transform.position = base.transform.position + vector2;
			obj.transform.localScale *= UnityEngine.Random.Range(randomScale.x, randomScale.y);
			obj.transform.Rotate(base.transform.up, UnityEngine.Random.Range(randomRotation.x, randomRotation.y), Space.World);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Vector3 position = base.transform.position;
		Vector3 up = base.transform.up;
		Vector3 vector = Vector3.Cross(up, Vector3.forward);
		if (vector.sqrMagnitude < 0.001f)
		{
			vector = Vector3.Cross(up, Vector3.right);
		}
		vector.Normalize();
		Vector3 vector2 = Vector3.Cross(vector, up);
		Vector3 vector3 = position + vector * radius;
		for (int i = 1; i <= 64; i++)
		{
			float f = (float)i / 64f * MathF.PI * 2f;
			Vector3 vector4 = position + (vector * Mathf.Cos(f) + vector2 * Mathf.Sin(f)) * radius;
			Gizmos.DrawLine(vector3, vector4);
			vector3 = vector4;
		}
	}
}
