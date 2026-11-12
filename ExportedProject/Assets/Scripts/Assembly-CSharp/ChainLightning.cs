using UnityEngine;

public class ChainLightning : Ability, IGameEventListener<OnCharacterDealingDamageParams>
{
	public GameObject chainLightningPrefab;

	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public float chanceToTrigger = 1f;

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.damageSource != "chainlightning" && eventParameters.damageSource.Contains("lightning") && Random.value < chanceToTrigger && (bool)chainLightningPrefab)
		{
			ChainLightningProjectile component = Object.Instantiate(chainLightningPrefab, eventParameters.receiver.transform.position, Quaternion.identity).GetComponent<ChainLightningProjectile>();
			component.owner = owner;
			component.damage = eventParameters.attacker.damage.GetCurrentValue() * damageMultiplier.GetCurrentValue();
		}
		return IGameEventResult.SUCCESS;
	}

	public void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
	}
}
