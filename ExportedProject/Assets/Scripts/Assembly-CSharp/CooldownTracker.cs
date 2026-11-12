using System.Collections.Generic;

public class CooldownTracker<T>
{
	public float cooldown;

	private readonly Dictionary<T, float> timers;

	private readonly List<T> keys = new List<T>();

	public CooldownTracker(float cooldownLength)
	{
		cooldown = cooldownLength;
		timers = new Dictionary<T, float>();
	}

	public bool IsOnCooldown(T obj)
	{
		if (timers.TryGetValue(obj, out var value))
		{
			return value > 0f;
		}
		return false;
	}

	public void Update(float deltaTime)
	{
		keys.Clear();
		keys.AddRange(timers.Keys);
		foreach (T key in keys)
		{
			timers[key] -= deltaTime;
			if (timers[key] <= 0f)
			{
				timers.Remove(key);
			}
		}
	}

	public void ExhaustCooldown(T obj)
	{
		timers[obj] = cooldown;
	}
}
