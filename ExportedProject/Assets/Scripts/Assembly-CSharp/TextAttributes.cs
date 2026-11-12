using System.Collections.Generic;
using UnityEngine;

public static class TextAttributes
{
	public static Color COLOR_POISON = new Color(0.6f, 1f, 0.3f, 1f);

	public static Color COLOR_FROZEN = new Color(0.5f, 0.7f, 1f, 1f);

	public static Color COLOR_STICKY = new Color(0.6f, 1f, 0.2f, 1f);

	public static Color COLOR_STUNNED = new Color(0.8f, 0.5f, 0.8f, 1f);

	public static Color COLOR_FLAME = new Color(1f, 0.4f, 0.2f, 1f);

	public static Color COLOR_CRITICAL = new Color(1f, 0f, 0f, 1f);

	public static Color COLOR_XP = new Color(0.99609375f, 0.59765625f, 13f / 128f, 1f);

	public static Dictionary<string, Color> COLOR_PER_DAMAGE_SOURCE = new Dictionary<string, Color>
	{
		{ "poison", COLOR_POISON },
		{ "burn", COLOR_FLAME },
		{ "critical", COLOR_CRITICAL }
	};

	public static Color GetColorFromDamageSource(string damageSourceIdentifier)
	{
		return COLOR_PER_DAMAGE_SOURCE.GetValueOrDefault(damageSourceIdentifier, Color.white);
	}
}
