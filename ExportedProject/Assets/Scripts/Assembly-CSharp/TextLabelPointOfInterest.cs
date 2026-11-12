using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLabelPointOfInterest : MonoBehaviour
{
	public TextMeshProUGUI text;

	public Transform target;

	public Image arrow;

	public Canvas canvas;

	private float highlightTimer;

	public GameObject highlightPrefab;

	private void LateUpdate()
	{
		if (target == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		float num = 64f + Mathf.Sin(Time.time * 8f) * 8f + 8f;
		Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
		arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(position.y - (float)Screen.height * 0.5f, position.x - (float)Screen.width * 0.5f) / MathF.PI * 180f - 90f);
		Vector2 vector = KeepInBounds(new Vector2(position.x, position.y), new Vector2(num, num), new Vector2((float)Screen.width - num, (float)Screen.height - num));
		if (Mathf.Abs(vector.x - position.x) < 8f && Mathf.Abs(vector.y - position.y) < 8f)
		{
			canvas.enabled = false;
		}
		else
		{
			canvas.enabled = true;
		}
		position.x = vector.x;
		position.y = vector.y;
		position.z = 16f;
		position = Camera.main.ScreenToWorldPoint(position);
		base.transform.position = position;
		highlightTimer -= Time.deltaTime;
		if (highlightTimer < 0f)
		{
			highlightTimer = 10f;
			UnityEngine.Object.Instantiate(highlightPrefab, canvas.transform);
		}
	}

	public Vector2 KeepInBounds(Vector2 p, Vector2 minBounds, Vector2 maxBounds)
	{
		if (p.x >= minBounds.x && p.x <= maxBounds.x && p.y >= minBounds.y && p.y <= maxBounds.y)
		{
			return p;
		}
		Vector2 vector = (minBounds + maxBounds) * 0.5f;
		Vector2 vector2 = p - vector;
		float num = float.PositiveInfinity;
		Vector2 result = vector;
		if (vector2.x < 0f)
		{
			float num2 = (minBounds.x - vector.x) / vector2.x;
			Vector2 vector3 = vector + vector2 * num2;
			if (vector3.y >= minBounds.y && vector3.y <= maxBounds.y && num2 < num)
			{
				num = num2;
				result = vector3;
			}
		}
		if (vector2.x > 0f)
		{
			float num3 = (maxBounds.x - vector.x) / vector2.x;
			Vector2 vector4 = vector + vector2 * num3;
			if (vector4.y >= minBounds.y && vector4.y <= maxBounds.y && num3 < num)
			{
				num = num3;
				result = vector4;
			}
		}
		if (vector2.y < 0f)
		{
			float num4 = (minBounds.y - vector.y) / vector2.y;
			Vector2 vector5 = vector + vector2 * num4;
			if (vector5.x >= minBounds.x && vector5.x <= maxBounds.x && num4 < num)
			{
				num = num4;
				result = vector5;
			}
		}
		if (vector2.y > 0f)
		{
			float num5 = (maxBounds.y - vector.y) / vector2.y;
			Vector2 vector6 = vector + vector2 * num5;
			if (vector6.x >= minBounds.x && vector6.x <= maxBounds.x && num5 < num)
			{
				num = num5;
				result = vector6;
			}
		}
		return result;
	}

	public Vector2 KeepInBoundsCircle(Vector2 p, Vector2 center, float r)
	{
		float magnitude = (p - center).magnitude;
		if (magnitude > r)
		{
			p = center + (p - center) / magnitude * r;
		}
		return p;
	}

	public void SetText(string text)
	{
		this.text.text = text;
	}
}
