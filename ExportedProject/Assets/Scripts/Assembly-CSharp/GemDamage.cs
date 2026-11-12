using UnityEngine;

public class GemDamage : Ability, IGameEventListener<OnGemAttractedParams>
{
	public GameObject damageTriggerPrefab;

	public ValueStackFloat damageMultiplier = new ValueStackFloat(1f);

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnGemAttracted.AddListener(this);
	}

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnGemAttractedParams eventParameters)
	{
		DamageTrigger component = Object.Instantiate(damageTriggerPrefab, eventParameters.gem.transform).GetComponent<DamageTrigger>();
		component.transform.localPosition = Vector3.zero;
		component.owner = eventParameters.collector;
		component.damage = eventParameters.collector.damage.GetCurrentValue() * damageMultiplier.GetCurrentValue();
		component.damageSource = GetIdentifier();
		return IGameEventResult.SUCCESS;
	}

	public void OnDestroy()
	{
		GameEventManager.Get().OnGemAttracted.RemoveListener(this);
	}
}
