using UnityEngine;

public class CorpseExplosion : Ability, IGameEventListener<OnCharacterDeathParams>
{
	public ValueStackFloat radius = new ValueStackFloat(3f);

	public ValueStackFloat damageMultiplier = new ValueStackFloat(2f);

	public GameObject explosionPrefab;

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDeath.AddListener(this);
	}

	public int GetOrder()
	{
		return 100;
	}

	public IGameEventResult OnGameEvent(OnCharacterDeathParams eventParameters)
	{
		if (eventParameters.killer != owner)
		{
			return IGameEventResult.SUCCESS;
		}
		float num = damageMultiplier.GetCurrentValue();
		if ((bool)owner)
		{
			num *= owner.damage.GetCurrentValue();
		}
		ExplosionHelper.Spawn(explosionPrefab, eventParameters.victim.transform.position, radius.GetCurrentValue() * Mathf.Sqrt(owner.area.GetCurrentValue()), Player.Get().GetAttributes().GetElement() * 0.5f, owner, base.gameObject.layer, num);
		return IGameEventResult.SUCCESS;
	}

	private void OnDestroy()
	{
		GameEventManager.Get()?.OnCharacterDeath.RemoveListener(this);
	}
}
