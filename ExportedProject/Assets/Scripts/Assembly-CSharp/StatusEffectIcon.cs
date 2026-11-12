using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class StatusEffectIcon : MonoBehaviour
{
	public StatusEffect statusEffect;

	public float maxFill;

	public Image background;

	public Image fill;

	public TextMeshProUGUI text;

	public TextMeshProUGUI stacks;

	private ToolTip toolTip;

	private void Start()
	{
	}

	private void UpdateToolTip()
	{
		if (toolTip.IsShown())
		{
			toolTip.text = statusEffect.GetShortDescription();
		}
	}

	public void Init(StatusEffect statusEffect)
	{
		this.statusEffect = statusEffect;
		if (statusEffect.GetTypeIdentifier().Contains("potion"))
		{
			text.text = "<sprite name=potion>";
			background.color = statusEffect.tint;
		}
		else if (statusEffect.GetTypeIdentifier().Contains("burden"))
		{
			text.text = "<sprite name=scale>";
			background.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else if (statusEffect.GetTypeIdentifier().Contains("sticky"))
		{
			text.text = "<sprite name=debuff>";
			background.color = statusEffect.tint;
		}
		else
		{
			text.text = "^";
			background.color = statusEffect.tint;
		}
		toolTip = base.gameObject.GetComponentInChildren<ToolTip>();
		UpdateToolTip();
	}

	private void Update()
	{
		if (statusEffect != null)
		{
			maxFill = Mathf.Max(statusEffect.duration, maxFill);
			fill.fillAmount = Mathf.Clamp01(statusEffect.duration / maxFill);
			if (statusEffect.isStackable)
			{
				stacks.transform.parent.gameObject.SetActive(value: true);
				stacks.text = statusEffect.stackCount.ToString() ?? "";
			}
			else
			{
				stacks.transform.parent.gameObject.SetActive(value: false);
			}
			UpdateToolTip();
		}
	}
}
