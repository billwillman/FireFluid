using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
	private static Dictionary<CharacterAttributes, Dictionary<string, StatusEffect>> targetStatusEffects = new Dictionary<CharacterAttributes, Dictionary<string, StatusEffect>>();

	private static StatusEffectManager instance = null;

	private List<(CharacterAttributes, string)> toRemove = new List<(CharacterAttributes, string)>();

	public static float secondsPerPoisonTick = 0.5f;

	public static StatusEffectFrost frost = new StatusEffectFrost();

	public static StatusEffectPoison poison = new StatusEffectPoison();

	public static StatusEffectSticky sticky = new StatusEffectSticky();

	public static StatusEffectStunned stunned = new StatusEffectStunned();

	private static Collider[] colliders = new Collider[128];

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
	}

	public static StatusEffectManager Get()
	{
		return instance;
	}

	public static bool Has(StatusEffect effect, CharacterAttributes target)
	{
		if (!targetStatusEffects.TryGetValue(target, out var value))
		{
			return false;
		}
		if (value.ContainsKey(effect.GetTypeIdentifier()))
		{
			return true;
		}
		return false;
	}

	public static StatusEffect Apply(StatusEffect effect, CharacterAttributes target, CharacterAttributes owner)
	{
		if (!targetStatusEffects.TryGetValue(target, out var value))
		{
			value = new Dictionary<string, StatusEffect>();
			targetStatusEffects[target] = value;
		}
		string typeIdentifier = effect.GetTypeIdentifier();
		if (value.TryGetValue(typeIdentifier, out var value2))
		{
			if (effect.isStackable)
			{
				value2.stackCount += effect.stackCount;
				value2.duration = Mathf.Max(value2.duration, effect.duration);
				value2.OnStack(value2.stackCount);
			}
			else
			{
				value2.duration = Mathf.Max(value2.duration, effect.duration);
			}
			return value2;
		}
		StatusEffect statusEffect = (value[typeIdentifier] = effect.Clone());
		statusEffect.OnApply(target, owner);
		return statusEffect;
	}

	public static void Apply(StatusEffect effect, CharacterAttributes attacker, Vector3 position, float radius, int attackLayer)
	{
		int num = Physics.OverlapSphereNonAlloc(position, radius, colliders);
		for (int i = 0; i < num; i++)
		{
			if (!(attacker != null) || !Physics.GetIgnoreLayerCollision(attackLayer, colliders[i].gameObject.layer))
			{
				CharacterAttributes componentInParent = colliders[i].GetComponentInParent<CharacterAttributes>();
				if (componentInParent != null)
				{
					Apply(effect, componentInParent, attacker);
				}
			}
		}
	}

	private void Update()
	{
		toRemove.Clear();
		float deltaTime = Time.deltaTime;
		Player nearestPlayer = Player.GetNearestPlayer(base.transform.position);
		poison.damageMultiplier = nearestPlayer.GetAttributes().poisonDamageMultiplier.GetCurrentValue();
		poison.tickInterval = secondsPerPoisonTick / Mathf.Max(nearestPlayer.GetAttributes().attackSpeed.GetCurrentValue(), 0.1f);
		foreach (KeyValuePair<CharacterAttributes, Dictionary<string, StatusEffect>> targetStatusEffect in targetStatusEffects)
		{
			CharacterAttributes key = targetStatusEffect.Key;
			foreach (KeyValuePair<string, StatusEffect> item in targetStatusEffect.Value)
			{
				StatusEffect value = item.Value;
				if (value.IsExpired() || value.target == null || value.target.health.IsExhausted())
				{
					value.OnRemove();
					toRemove.Add((key, item.Key));
					continue;
				}
				value.timeSinceLastTick += deltaTime;
				if (value.timeSinceLastTick >= value.tickInterval)
				{
					value.OnTick(deltaTime);
					value.duration -= value.timeSinceLastTick;
					value.timeSinceLastTick = 0f;
				}
			}
		}
		foreach (var item2 in toRemove)
		{
			if (targetStatusEffects.TryGetValue(item2.Item1, out var value2))
			{
				value2.Remove(item2.Item2);
				if (value2.Count == 0)
				{
					targetStatusEffects.Remove(item2.Item1);
				}
			}
		}
	}

	public static IEnumerable<StatusEffect> GetEffects(CharacterAttributes target)
	{
		if (targetStatusEffects.TryGetValue(target, out var value))
		{
			return value.Values;
		}
		return Enumerable.Empty<StatusEffect>();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
