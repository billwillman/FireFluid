using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIValueDescriptor : MonoBehaviour
{
	protected TextMeshProUGUI text;

	protected MonoBehaviour target;

	public string field;

	public UIValueDescriptorFloatDisplay display;

	public float upperBound = 1f;

	public float lowerBound;

	public float stepSize = 0.1f;

	public bool indicateValueChange;

	public bool alwaysUpdateDescriptor;

	private bool valueChanged;

	private Type targetType;

	private FieldInfo fieldInfo;

	private object value;

	public string valueChangeSound;

	private float holdTime;

	private void Awake()
	{
		target = Settings.GetInstance();
	}

	protected void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		UpdateTargetInfo();
		UpdateDescriptor();
	}

	public virtual void Update()
	{
		if (alwaysUpdateDescriptor)
		{
			UpdateDescriptor();
		}
		if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.transform == base.transform.parent)
		{
			int num = 0;
			if (InputManager.MoveRightPressedThisFrame() || ((double)holdTime > 0.5 && InputManager.MoveHorizontal > 0.5f))
			{
				num++;
				holdTime = 0.4f;
			}
			if (InputManager.MoveLeftPressedThisFrame() || ((double)holdTime > 0.5 && InputManager.MoveHorizontal < -0.5f))
			{
				num--;
				holdTime = 0.4f;
			}
			if (((double)InputManager.MoveHorizontalLastFrame < -0.5 && InputManager.MoveHorizontal < -0.5f) || (InputManager.MoveHorizontalLastFrame > 0.5f && InputManager.MoveHorizontal > 0.5f))
			{
				holdTime += Time.unscaledDeltaTime;
			}
			else
			{
				holdTime = 0f;
			}
			ChangeValue(num);
		}
	}

	public void ChangeValue(int steps)
	{
		if (steps != 0)
		{
			valueChanged = true;
			object oldValue = value;
			UpdateTargetInfo();
			if (value is bool)
			{
				ToggleBool();
			}
			if (value is float)
			{
				ChangeFloat((float)steps * stepSize);
			}
			if (value is int)
			{
				ChangeInt(steps);
			}
			if (value is Enum)
			{
				ChangeEnum(steps);
			}
			if (valueChangeSound != null)
			{
				SoundEffectManager.Play(valueChangeSound);
			}
			UpdateDescriptor();
			IValueDescriptorChangedListener[] components = GetComponents<IValueDescriptorChangedListener>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].ValueChanged(oldValue, value);
			}
		}
	}

	protected void UpdateTargetInfo()
	{
		targetType = target.GetType();
		fieldInfo = targetType.GetField(field);
		value = fieldInfo.GetValue(target);
	}

	public void ToggleBool()
	{
		bool flag = (bool)value;
		fieldInfo.SetValue(target, !flag);
		UpdateDescriptor();
	}

	public void ChangeFloat(float delta)
	{
		float num = (float)value;
		fieldInfo.SetValue(target, Mathf.Clamp(num + delta, lowerBound, upperBound));
		UpdateDescriptor();
	}

	public void ChangeInt(int delta)
	{
		int num = (int)value;
		fieldInfo.SetValue(target, Mathf.RoundToInt(Mathf.Clamp(num + delta, lowerBound, upperBound)));
		UpdateDescriptor();
	}

	public void ChangeEnum(int delta)
	{
		int num = (int)value;
		Array values = Enum.GetValues(value.GetType());
		object obj = values.GetValue((num + delta + values.Length) % values.Length);
		fieldInfo.SetValue(target, obj);
		UpdateDescriptor();
	}

	public virtual void UpdateDescriptor()
	{
		UpdateTargetInfo();
		string text = "type not supported: " + targetType;
		if (value is bool)
		{
			text = (((bool)value) ? StringLocalization.Get("settings_on") : StringLocalization.Get("settings_off"));
		}
		if (value is float)
		{
			switch (display)
			{
			case UIValueDescriptorFloatDisplay.bars:
			{
				float num = Mathf.Clamp01(((float)value - lowerBound) / (upperBound - lowerBound));
				text = new string('|', Mathf.RoundToInt(num / stepSize)) + "<color=#888888>" + new string('|', Mathf.RoundToInt((1f - num) / stepSize)) + "</color>";
				break;
			}
			case UIValueDescriptorFloatDisplay.number:
				text = ((float)value).ToString("0.00");
				break;
			case UIValueDescriptorFloatDisplay.percent:
				text = Mathf.RoundToInt((float)value * 100f) + "%";
				break;
			}
		}
		if (value is int)
		{
			switch (display)
			{
			case UIValueDescriptorFloatDisplay.bars:
				text = "";
				text = new string('|', Mathf.RoundToInt((float)value / stepSize));
				break;
			case UIValueDescriptorFloatDisplay.number:
				text = value?.ToString() ?? "";
				break;
			case UIValueDescriptorFloatDisplay.percent:
				text = value?.ToString() + "%";
				break;
			}
		}
		if (value is Enum)
		{
			text = StringLocalization.Get("enum_" + value.GetType().ToString().ToLowerInvariant() + "_" + value.ToString().ToLowerInvariant());
		}
		if (valueChanged && indicateValueChange)
		{
			text += "•";
		}
		this.text.text = text;
	}

	public static void UpdateAll()
	{
		UIValueDescriptor[] array = (UIValueDescriptor[])UnityEngine.Object.FindObjectsOfType(typeof(UIValueDescriptor));
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateDescriptor();
		}
	}
}
