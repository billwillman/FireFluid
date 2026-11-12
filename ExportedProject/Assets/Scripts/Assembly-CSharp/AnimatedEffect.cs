using System;
using UnityEngine;

[Serializable]
public class AnimatedEffect
{
	public string id;

	public Color color;

	public float duration;

	public float elapsed;

	public float frequency;

	public EffectMode mode;

	public AnimatedEffect(string id, Color color, float duration, EffectMode mode, float frequency = 4f)
	{
		this.id = id;
		this.color = color;
		this.duration = duration;
		this.mode = mode;
		this.frequency = frequency;
		elapsed = 0f;
	}

	public bool Update(float dt)
	{
		elapsed += dt;
		return elapsed < duration;
	}

	public Color Evaluate()
	{
		Color result = color;
		Mathf.Clamp01(elapsed / Mathf.Max(duration, 0.0001f));
		switch (mode)
		{
		case EffectMode.SquareWave:
		{
			bool flag = Mathf.RoundToInt(Time.time * frequency) % 2 == 0;
			result.a *= (flag ? 1f : 0f);
			break;
		}
		case EffectMode.SawTooth:
			result.a *= 1f - Time.time * frequency % 1f;
			break;
		case EffectMode.Fade:
			result.a *= Mathf.Clamp01(1f - elapsed);
			break;
		case EffectMode.SlowFade:
			result.a *= Mathf.Clamp01(elapsed * 0.2f);
			break;
		case EffectMode.Sine:
			result.a *= Mathf.Sin(Time.time * frequency) * 0.2f + 0.8f;
			break;
		}
		return result;
	}
}
