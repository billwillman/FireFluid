using UnityEngine;

public class FluidToEnemyDamage : MonoBehaviour
{
	public const string DAMAGE_SOURCE = "burn";

	public CooldownTracker<Enemy> enemyTickTime = new CooldownTracker<Enemy>(0.5f);

	public CooldownTracker<Destructable> destructableTickTime = new CooldownTracker<Destructable>(0.5f);

	public float secondsPerTick = 0.5f;

	public float reactionThreshold = 0.001f;

	public float densityGThreshold = 0.02f;

	public float densityBThreshold = 0.02f;

	private static Color hitHighlightColor = new Color(16f, 0.5f, 0f, 1f);

	private static Color hitHighlightColorDestructable = new Color(16f, 0.5f, 0f, 0.5f);

	private void Update()
	{
		if ((bool)TimeManager.Get() && TimeManager.Get().time < 1f)
		{
			return;
		}
		Player nearestPlayer = Player.GetNearestPlayer(base.transform.position);
		float cooldown = secondsPerTick / Mathf.Max(nearestPlayer.GetAttributes().attackSpeed.GetCurrentValue(), 0.1f);
		enemyTickTime.cooldown = cooldown;
		destructableTickTime.cooldown = cooldown;
		foreach (Enemy item in Enemy.all)
		{
			if (item.GetAttributes().health.IsExhausted() || enemyTickTime.IsOnCooldown(item))
			{
				continue;
			}
			Vector4 reactionValue = FluidSimCPUMapperAsync.Get().GetReactionValue(item.transform.position);
			if (reactionValue.x + reactionValue.y + reactionValue.z > reactionThreshold)
			{
				enemyTickTime.ExhaustCooldown(item);
				float num = nearestPlayer.GetAttributes().damage.GetCurrentValue();
				if (StatusEffectManager.Has(StatusEffectManager.frost, item.GetAttributes()))
				{
					num *= nearestPlayer.GetAttributes().burnDamageToFrozenMultiplier.GetCurrentValue();
				}
				if (item.GetAttributes().ReceiveDamage(num, nearestPlayer.GetAttributes(), "burn") > 0.01f)
				{
					RendererEffects.Highlight(item.gameObject, "burn", hitHighlightColor, 0.075f);
				}
			}
			Vector4 densityValue = FluidSimCPUMapperAsync.Get().GetDensityValue(item.transform.position);
			if (densityValue.y > densityGThreshold)
			{
				StatusEffectManager.Apply(StatusEffectManager.poison, item.GetAttributes(), nearestPlayer.GetAttributes());
			}
			if (densityValue.z > densityBThreshold)
			{
				StatusEffectManager.Apply(StatusEffectManager.frost, item.GetAttributes(), nearestPlayer.GetAttributes());
			}
		}
		enemyTickTime.Update(Time.deltaTime);
		foreach (Destructable item2 in Destructable.all)
		{
			if (destructableTickTime.IsOnCooldown(item2))
			{
				continue;
			}
			Vector4 reactionValue2 = FluidSimCPUMapperAsync.Get().GetReactionValue(item2.transform.position);
			if (reactionValue2.x + reactionValue2.y + reactionValue2.z > reactionThreshold)
			{
				destructableTickTime.ExhaustCooldown(item2);
				float currentValue = nearestPlayer.GetAttributes().damage.GetCurrentValue();
				if (item2.ReceiveDamage(currentValue, nearestPlayer.GetAttributes(), "burn") > 0.01f)
				{
					RendererEffects.Highlight(item2.gameObject, "burn", hitHighlightColorDestructable, 0.075f);
				}
			}
		}
		destructableTickTime.Update(Time.deltaTime);
	}
}
