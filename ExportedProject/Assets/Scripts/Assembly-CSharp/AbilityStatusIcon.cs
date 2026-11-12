using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class AbilityStatusIcon : MonoBehaviour
{
	public IAbilityStatus abilityStatus;

	public Image fill;

	public Image icon;

	public Image highlight;

	public Color triggerReadyColor;

	public Color triggerColor;

	public TextMeshProUGUI stacks;

	private ToolTip toolTip;

	private void UpdateToolTip()
	{
		if (toolTip.IsShown())
		{
			toolTip.text = abilityStatus.GetToolTip();
		}
	}

	public void Init(IAbilityStatus abilityStatus)
	{
		this.abilityStatus = abilityStatus;
		if (abilityStatus.GetIcon() != null)
		{
			icon.sprite = abilityStatus.GetIcon();
		}
		GetComponent<RectTransform>().sizeDelta *= abilityStatus.GetSize();
		toolTip = base.gameObject.GetComponentInChildren<ToolTip>();
		UpdateToolTip();
	}

	private void Update()
	{
		if (abilityStatus != null)
		{
			if (abilityStatus.CooldownStacksFull())
			{
				fill.fillAmount = 0f;
			}
			else
			{
				fill.fillAmount = Mathf.Clamp01(abilityStatus.GetCurrentCooldown() / abilityStatus.GetCooldown());
			}
			if (abilityStatus.GetCooldownStacks() > 1 && abilityStatus.GetCurrentCooldownStacks() > 0)
			{
				stacks.transform.parent.gameObject.SetActive(value: true);
				stacks.text = abilityStatus.GetCurrentCooldownStacks().ToString() ?? "";
			}
			else
			{
				stacks.transform.parent.gameObject.SetActive(value: false);
			}
			if (abilityStatus.CurrentlyTriggering())
			{
				highlight.color = triggerColor;
			}
			else if (!abilityStatus.CooldownIsExhausted() && abilityStatus.CanSpendMana(spendAttempt: false))
			{
				highlight.color = triggerReadyColor;
			}
			else
			{
				Color color = highlight.color;
				color.a = Mathf.Lerp(color.a, 0f, Time.unscaledDeltaTime * 4f);
				highlight.color = color;
			}
			UpdateToolTip();
		}
	}
}
