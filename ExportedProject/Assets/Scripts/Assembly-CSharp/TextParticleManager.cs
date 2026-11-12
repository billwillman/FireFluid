using System.Collections.Generic;
using UnityEngine;

public class TextParticleManager : MonoBehaviour
{
	public static TextParticleManager Instance;

	[Header("References")]
	public TextParticle textParticlePrefab;

	private RectTransform canvasTransform;

	public TextLabel textLabelPrefab;

	public TextLabelPointOfInterest textLabelPointOfInterestPrefab;

	private Queue<TextParticle> pool = new Queue<TextParticle>();

	private void Awake()
	{
		Instance = this;
		canvasTransform = GetComponent<RectTransform>();
	}

	public static void Show(string text, Vector3 worldPosition)
	{
		Show(text, worldPosition, Color.white);
	}

	public static void Show(string text, Vector3 worldPosition, float duration)
	{
		Show(text, worldPosition, Color.white, 64f, duration);
	}

	public static void Show(string text, Vector3 worldPosition, Color color, float size = 64f, float duration = 2f)
	{
		if (Instance == null)
		{
			Debug.LogWarning("TextParticleManager not initialized.");
			return;
		}
		TextParticle fromPool = Instance.GetFromPool();
		fromPool.transform.SetParent(Instance.canvasTransform, worldPositionStays: false);
		fromPool.transform.position = worldPosition;
		fromPool.transform.localScale = Vector3.one;
		fromPool.transform.rotation = Quaternion.LookRotation(fromPool.transform.position - Camera.main.transform.position, Camera.main.transform.up);
		fromPool.duration = duration;
		fromPool.gameObject.SetActive(value: true);
		fromPool.Play(text, color, size);
	}

	public static void Show(string text, Transform target, float size = 64f)
	{
		Show(text, target, Color.white, size);
	}

	public static void Show(string text, Transform target, Color color, float size = 64f)
	{
		if (target == null)
		{
			Debug.LogWarning("target null.");
			return;
		}
		Vector3 center = target.GetComponentsInChildren<Renderer>().ComputeBounds().center;
		Show(text, center, color, size);
	}

	public static TextLabel Label(string text, Transform target, Color background, float size = 24f)
	{
		if (target == null)
		{
			return null;
		}
		Bounds bounds = target.GetComponentsInChildren<Renderer>().ComputeBounds();
		TextLabel textLabel = Object.Instantiate(Instance.textLabelPrefab);
		textLabel.text.fontSize = size;
		textLabel.background.color = background;
		textLabel.offset = bounds.center - target.transform.position + Vector3.up * 2f;
		textLabel.target = target;
		textLabel.text.text = text;
		textLabel.UpdatePosition();
		return textLabel;
	}

	public static TextLabelPointOfInterest LabelPointOfInterest(string text, Transform target, float size = 24f)
	{
		if (target == null)
		{
			return null;
		}
		target.GetComponentsInChildren<Renderer>();
		TextLabelPointOfInterest textLabelPointOfInterest = Object.Instantiate(Instance.textLabelPointOfInterestPrefab);
		textLabelPointOfInterest.text.fontSize = size;
		textLabelPointOfInterest.target = target;
		textLabelPointOfInterest.text.text = text;
		return textLabelPointOfInterest;
	}

	private TextParticle GetFromPool()
	{
		if (pool.Count > 0)
		{
			return pool.Dequeue();
		}
		return Object.Instantiate(textParticlePrefab);
	}

	public void ReturnToPool(TextParticle particle)
	{
		particle.gameObject.SetActive(value: false);
		pool.Enqueue(particle);
	}
}
