using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableShowHintPointer : InteractableShowHint
{
	public Transform target;

	public float distanceToTarget = 100f;

	private Canvas canvas;

	private TextMeshProUGUI text;

	public Image pointer;

	public void Start()
	{
		canvas = GetComponentInParent<Canvas>();
		text = GetComponentInChildren<TextMeshProUGUI>();
	}

	public override void OnStartShowHint(InteractionController c)
	{
		if (target.localPosition.x / canvas.GetComponent<RectTransform>().rect.width > 0.1f)
		{
			base.transform.localPosition = new Vector3(0f - distanceToTarget, 0f);
			text.rectTransform.parent.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
			pointer.rectTransform.anchoredPosition = new Vector2(-2f, 0f);
			pointer.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			base.transform.localPosition = new Vector3(distanceToTarget, 0f);
			text.rectTransform.parent.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
			pointer.rectTransform.anchoredPosition = new Vector2(2f, 0f);
			pointer.rectTransform.localScale = new Vector3(1f, 1f, 1f);
		}
		base.OnStartShowHint(c);
	}

	public override void OnEndShowHint()
	{
		base.OnEndShowHint();
	}
}
