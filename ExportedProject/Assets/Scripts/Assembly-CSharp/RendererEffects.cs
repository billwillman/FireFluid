using System.Collections.Generic;
using UnityEngine;

public class RendererEffects : MonoBehaviour
{
	[Header("Renderer Cache")]
	public bool updateRendererListEveryFrame;

	[Header("Visibility")]
	public bool visible = true;

	private static readonly int ColorID = Shader.PropertyToID("_HighlightColor");

	private readonly Dictionary<string, ColorModifier> modifiers = new Dictionary<string, ColorModifier>();

	private readonly Dictionary<string, AnimatedEffect> effects = new Dictionary<string, AnimatedEffect>();

	private readonly List<Renderer> renderers = new List<Renderer>();

	private MaterialPropertyBlock block;

	public Color modifierColor = Color.clear;

	public Color effectColor = Color.clear;

	public Color currentColor = Color.clear;

	private bool initialized;

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		if (!initialized)
		{
			Init();
		}
	}

	private void Init()
	{
		if (!initialized)
		{
			block = new MaterialPropertyBlock();
			RefreshRenderers();
			initialized = true;
		}
	}

	private void Update()
	{
		if (initialized)
		{
			if (updateRendererListEveryFrame)
			{
				RefreshRenderers();
			}
			UpdateEffects();
			EvaluateColor();
			Apply();
			ApplyVisibility();
		}
	}

	private void EvaluateColor()
	{
		Color color = Color.clear;
		foreach (KeyValuePair<string, ColorModifier> modifier in modifiers)
		{
			ColorModifier value = modifier.Value;
			switch (value.blendMode)
			{
			case ColorBlendMode.Blend:
				color = Color.Lerp(color, value.color, value.color.a);
				break;
			case ColorBlendMode.Add:
				color += value.color;
				break;
			case ColorBlendMode.Override:
				color = value.color;
				break;
			}
		}
		modifierColor = Color.Lerp(modifierColor, color, Time.deltaTime * 2f);
		effectColor = Color.clear;
		foreach (KeyValuePair<string, AnimatedEffect> effect in effects)
		{
			effectColor = Color.Lerp(effectColor, effect.Value.Evaluate(), effect.Value.color.a);
		}
		currentColor = Color.Lerp(modifierColor, effectColor, effectColor.a);
	}

	private void UpdateEffects()
	{
		List<string> list = null;
		foreach (KeyValuePair<string, AnimatedEffect> effect in effects)
		{
			if (!effect.Value.Update(Time.deltaTime))
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(effect.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (string item in list)
		{
			effects.Remove(item);
		}
	}

	private void Apply()
	{
		for (int num = renderers.Count - 1; num >= 0; num--)
		{
			Renderer renderer = renderers[num];
			if (renderer == null)
			{
				renderers.RemoveAt(num);
			}
			else
			{
				block.Clear();
				_ = renderer.sharedMaterial;
				block.SetColor(ColorID, currentColor);
				renderer.SetPropertyBlock(block);
			}
		}
	}

	private void ApplyVisibility()
	{
		for (int num = renderers.Count - 1; num >= 0; num--)
		{
			Renderer renderer = renderers[num];
			if (renderer == null)
			{
				renderers.RemoveAt(num);
			}
			else
			{
				renderer.enabled = visible;
			}
		}
	}

	private void RefreshRenderers()
	{
		renderers.Clear();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!(renderer is SpriteRenderer) && renderer != null && renderer.sharedMaterial != null)
			{
				renderers.Add(renderer);
			}
		}
	}

	public void AddModifier(string id, Color color, ColorBlendMode mode = ColorBlendMode.Blend)
	{
		if (!modifiers.TryGetValue(id, out var _))
		{
			modifiers[id] = new ColorModifier(id, color, mode);
		}
	}

	public void RemoveModifier(string id)
	{
		modifiers.Remove(id);
	}

	public static void Tint(GameObject obj, string id, Color color, ColorBlendMode mode = ColorBlendMode.Blend)
	{
		Get(obj).AddModifier(id, color, mode);
	}

	public static void Untint(GameObject obj, string id)
	{
		Get(obj).RemoveModifier(id);
	}

	public void PlayEffect(string id, Color color, float duration, EffectMode mode, float frequency = 4f)
	{
		if (effects.TryGetValue(id, out var value))
		{
			value.color = color;
			value.duration = Mathf.Max(value.duration, duration);
			value.frequency = frequency;
			value.mode = mode;
		}
		else
		{
			effects[id] = new AnimatedEffect(id, color, duration, mode, frequency);
		}
	}

	public void StopEffect(string id)
	{
		effects.Remove(id);
	}

	public static void Highlight(GameObject obj, string id, Color color, float duration, EffectMode mode = EffectMode.Solid, float frequency = 4f)
	{
		Get(obj).PlayEffect(id, color, duration, mode, frequency);
	}

	public static void StopHighlight(GameObject obj, string id)
	{
		Get(obj).StopEffect(id);
	}

	public static RendererEffects Get(GameObject obj)
	{
		RendererEffects rendererEffects = obj.GetComponent<RendererEffects>();
		if (!rendererEffects)
		{
			rendererEffects = obj.AddComponent<RendererEffects>();
		}
		return rendererEffects;
	}
}
