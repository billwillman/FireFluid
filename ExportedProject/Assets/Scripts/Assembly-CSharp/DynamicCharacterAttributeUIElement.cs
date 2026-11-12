using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class DynamicCharacterAttributeUIElement : MonoBehaviour
{
	public MonoBehaviour attributes;

	public string attributeName = "health";

	private DynamicCharacterAttribute attribute;

	public bool useAttributeMax;

	public float valueMin;

	public float valueMax = 1f;

	private Color normalTint;

	public Color highlightTint = Color.red;

	private float valueChangeHighlight;

	private float valueChangeHighlightGradualCooldown = 1f;

	public Color exhaustedTint = Color.red;

	public Image value;

	public Image change;

	public Image failedSpend;

	public Text valueText;

	public AttributeDisplayStyle valueTextDisplayStyle;

	public FillAmountRender fillAmountRender;

	public DynamicCharacterAttribute GetAttribute(object obj, string attributeName)
	{
		return (DynamicCharacterAttribute)obj.GetType().GetField(attributeName).GetValue(obj);
	}

	private void Start()
	{
		normalTint = value.color;
		attribute = GetAttribute(attributes, attributeName);
		attribute.onDynamicValueChanged.AddListener(OnDynamicValueChange);
		if (useAttributeMax)
		{
			valueMax = attribute.GetCurrentValue();
		}
	}

	public void HighlightChange()
	{
		valueChangeHighlight = 0.1f;
	}

	private void OnDynamicValueChange(float amount, DynamicCharacterAttributeValueChangedType type)
	{
		if (amount < 0f)
		{
			if (type == DynamicCharacterAttributeValueChangedType.instant)
			{
				HighlightChange();
			}
			if (type == DynamicCharacterAttributeValueChangedType.gradual && valueChangeHighlightGradualCooldown < 0f)
			{
				valueChangeHighlightGradualCooldown = 1f;
				HighlightChange();
			}
		}
	}

	private float GetFillValue(float v)
	{
		return Mathf.Max(0f, (v - valueMin) / (valueMax - valueMin));
	}

	private void Update()
	{
		if (attribute == null)
		{
			return;
		}
		if (useAttributeMax)
		{
			valueMax = attribute.GetCurrentValue();
		}
		valueChangeHighlight -= Time.deltaTime;
		valueChangeHighlightGradualCooldown -= Time.deltaTime;
		if (fillAmountRender == FillAmountRender.SpriteRenderer)
		{
			value.fillAmount = GetFillValue(attribute.GetDynamicValue());
			change.fillAmount = GetFillValue(attribute.GetSmoothedDynamicValue());
		}
		else
		{
			value.fillAmount = 1f;
			change.fillAmount = 1f;
			value.material.SetFloat("_FillAmount", GetFillValue(attribute.GetDynamicValue()));
			change.material.SetFloat("_FillAmount", GetFillValue(attribute.GetSmoothedDynamicValue()));
		}
		if (attribute.IsExhausted())
		{
			Color color = exhaustedTint;
			color.a = Mathf.Sin(Time.time * 8f) * 0.3f + 0.7f;
			value.color = color;
			color = change.color;
			color.a = 0f;
			change.color = color;
		}
		else
		{
			if (valueChangeHighlight > 0f)
			{
				value.color = highlightTint;
			}
			else
			{
				value.color = normalTint;
			}
			Color color = change.color;
			color.a = 1f;
			change.color = color;
		}
		if (failedSpend != null)
		{
			if (attribute.HasFailedSpendAttempt())
			{
				Color color = failedSpend.color;
				color.a = 1f;
				failedSpend.color = color;
			}
			else
			{
				Color color = failedSpend.color;
				color.a = Mathf.Clamp01(Mathf.Lerp(color.a, 0f, Time.deltaTime * 8f) - Time.deltaTime);
				failedSpend.color = color;
			}
		}
		if ((bool)valueText)
		{
			switch (valueTextDisplayStyle)
			{
			case AttributeDisplayStyle.Raw:
				valueText.text = Mathf.Max(Mathf.RoundToInt(attribute.GetDynamicValue()), 0).ToString();
				break;
			case AttributeDisplayStyle.MinMax:
				valueText.text = Mathf.RoundToInt(attribute.GetDynamicValue()) + "/" + attribute.GetCurrentValue();
				break;
			case AttributeDisplayStyle.Percent:
				valueText.text = Mathf.Max(Mathf.RoundToInt(attribute.GetDynamicValue() * 100f), 0) + "%";
				break;
			}
		}
	}
}
