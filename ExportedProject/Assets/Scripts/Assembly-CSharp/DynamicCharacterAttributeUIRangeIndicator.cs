using UnityEngine;
using UnityEngine.UI;

public class DynamicCharacterAttributeUIRangeIndicator : MonoBehaviour
{
	public AnimationCurve animation;

	private CharacterAttributePlayerUI ui;

	public string attributeName;

	public float min;

	public float max = 0.5f;

	private Image image;

	private float recentlyDamagedFactor;

	private DynamicCharacterAttribute attribute;

	public DynamicCharacterAttribute GetAttribute(object obj, string attributeName)
	{
		return (DynamicCharacterAttribute)obj.GetType().GetField(attributeName).GetValue(obj);
	}

	private void OnValueChanged(float valueDelta, DynamicCharacterAttributeValueChangedType changedType)
	{
		if (valueDelta < 0f)
		{
			recentlyDamagedFactor = 1f;
		}
	}

	private void Start()
	{
		ui = GetComponentInParent<CharacterAttributePlayerUI>();
		attribute = GetAttribute(ui.characterAttributes, attributeName);
		image = GetComponentInChildren<Image>();
		if (attribute == null)
		{
			Debug.Log("attribute " + attributeName + " not found.");
		}
		else
		{
			attribute.onDynamicValueChanged.AddListener(OnValueChanged);
		}
	}

	private void Update()
	{
		if (attribute == null)
		{
			Debug.Log("attribute " + attributeName + " not found.");
			return;
		}
		float num = 1f - Mathf.Clamp01((attribute.GetDynamicValue() - min) / (max - min));
		num += Mathf.Clamp01(recentlyDamagedFactor);
		recentlyDamagedFactor -= Time.deltaTime;
		if (num > 0.001f)
		{
			num *= animation.Evaluate(Time.time % 1f);
			num = Mathf.Clamp01(num);
		}
		if (attribute.IsExhausted())
		{
			num = 0.5f;
		}
		Color color = image.color;
		color.a = num;
		image.color = color;
	}
}
