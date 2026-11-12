using System.Collections;
using TMPro;
using UnityEngine;

public class TextParticle : MonoBehaviour
{
	public TextMeshProUGUI textMesh;

	private RectTransform rectTransform;

	public float duration = 2f;

	private float moveDistance = 50f;

	private float size = 64f;

	public void Play(string text, Color color, float size = 64f)
	{
		textMesh.text = text;
		textMesh.color = color;
		textMesh.fontSize = size;
		this.size = size;
		rectTransform = GetComponent<RectTransform>();
		StartCoroutine(AnimateAndDeactivate());
	}

	private IEnumerator AnimateAndDeactivate()
	{
		float time = 0f;
		Vector3 startPosition = rectTransform.anchoredPosition;
		Vector3 targetPosition = startPosition + Vector3.up * moveDistance;
		while (time < duration)
		{
			float t = time / duration;
			rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
			textMesh.alpha = Mathf.Lerp(1f, 0f, t);
			textMesh.fontSize = Mathf.Lerp(size, size * 0.25f, t);
			time += Time.deltaTime;
			yield return null;
		}
		TextParticleManager.Instance.ReturnToPool(this);
	}
}
