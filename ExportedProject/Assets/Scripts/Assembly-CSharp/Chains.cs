using System.Collections.Generic;
using UnityEngine;

public class Chains : Ability, IGameEventListener<OnCharacterDealingDamageParams>
{
	private HashSet<CharacterAttributes> chained = new HashSet<CharacterAttributes>();

	private List<ChainPair> chainPairs = new List<ChainPair>();

	public GameObject chainPrefab;

	public GameObject chainEndPrefab;

	private CharacterAttributes lastHit;

	public GameObject lastHitIndicator;

	public ValueStackInt numberOfChains = new ValueStackInt(1);

	public float maxConnectionRange = 32f;

	private HashSet<ChainPair> remove = new HashSet<ChainPair>();

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.attacker == owner && eventParameters.receiver != owner && chainPairs.Count < numberOfChains.GetCurrentValue() && !chained.Contains(eventParameters.receiver))
		{
			if (lastHit == null || lastHit.health.IsExhausted() || (lastHit.transform.position - eventParameters.receiver.transform.position).sqrMagnitude > maxConnectionRange * maxConnectionRange)
			{
				lastHit = eventParameters.receiver;
			}
			else if (lastHit != eventParameters.receiver)
			{
				Chain component = Object.Instantiate(chainPrefab).GetComponent<Chain>();
				component.Initialize(lastHit, eventParameters.receiver);
				chained.Add(lastHit);
				chained.Add(eventParameters.receiver);
				ChainPair chainPair = new ChainPair();
				chainPair.attributes1 = lastHit;
				chainPair.attributes2 = eventParameters.receiver;
				chainPair.chain = component;
				chainPairs.Add(chainPair);
				lastHit = null;
			}
		}
		return IGameEventResult.SUCCESS;
	}

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public override void Update()
	{
		base.Update();
		remove.Clear();
		foreach (ChainPair chainPair in chainPairs)
		{
			if (chainPair.chain == null || chainPair.chain.destroy)
			{
				chained.Remove(chainPair.attributes1);
				chained.Remove(chainPair.attributes2);
				remove.Add(chainPair);
			}
		}
		foreach (ChainPair item in remove)
		{
			chainPairs.Remove(item);
		}
		if (lastHit != null)
		{
			lastHitIndicator.transform.position = lastHit.transform.position + Vector3.up;
			lastHitIndicator.SetActive(value: true);
		}
		else
		{
			lastHitIndicator.SetActive(value: false);
		}
	}

	public void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
	}
}
