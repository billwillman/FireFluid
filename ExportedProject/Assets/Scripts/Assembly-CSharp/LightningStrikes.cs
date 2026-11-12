using UnityEngine;

public class LightningStrikes : Ability, IGameEventListener<OnCharacterDealingDamageParams>
{
	public GameObject lightingStrikePrefab;

	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public float chanceToStrike = 0.1f;

	public const string DAMAGE_SOURCE = "lightningstrike";

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
		if (eventParameters.damageSource == "burn" && Random.value < chanceToStrike)
		{
			if ((bool)lightingStrikePrefab)
			{
				LightningStrike component = Object.Instantiate(lightingStrikePrefab, eventParameters.receiver.transform.position, Quaternion.identity).GetComponent<LightningStrike>();
				component.start = eventParameters.receiver.transform.position + Vector3.up * 32f;
				component.end = eventParameters.receiver.transform.position;
			}
			float dmg = eventParameters.attacker.damage.GetCurrentValue() * damageMultiplier.GetCurrentValue();
			eventParameters.receiver.ReceiveDamage(dmg, eventParameters.attacker, "lightningstrike");
		}
		return IGameEventResult.SUCCESS;
	}

	public void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
	}
}
