using UnityEngine;

public class RunManager : MonoBehaviour, IGameEventListener<OnCharacterDealingDamageParams>
{
	public RunState state;

	private static RunManager instance;

	private RunStats currentRunStats;

	public GameObject runWonEffectPrefab;

	public static int currencyPerRunWon = 5;

	public static int maxSacrificesPerRunWon = 1;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public static RunManager Get()
	{
		return instance;
	}

	private void Update()
	{
		switch (state)
		{
		case RunState.Ongoing:
		{
			bool flag = true;
			foreach (Player item in Player.all)
			{
				if (!item.GetAttributes().health.IsExhausted())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				state = RunState.Lost;
				OnEndRun();
			}
			break;
		}
		case RunState.Won:
			if ((double)Random.value < 0.01)
			{
				Enemy.KillAll(Player.Get().GetAttributes(), "RunManager");
			}
			break;
		}
		if (TimeManager.Get() == null)
		{
			state = RunState.NotInRun;
		}
		else if (state == RunState.NotInRun)
		{
			state = RunState.Ongoing;
			OnStartRun();
		}
	}

	public void OnStartRun()
	{
		currentRunStats = new RunStats();
		currentRunStats.maskUsed = PersistentDataComponent.GetDataInstance().currentMask;
	}

	public void WinRun()
	{
		if (state != RunState.Lost)
		{
			state = RunState.Won;
			Object.Instantiate(runWonEffectPrefab, Player.Get().transform);
			OnEndRun();
			Player.Get().GetAttributes().SetInvincibility(9999999f);
		}
	}

	public RunStats GetCurrentStats()
	{
		if (currentRunStats == null)
		{
			currentRunStats = new RunStats();
		}
		return currentRunStats;
	}

	private void FinallizeCurrentRunStats()
	{
		currentRunStats.state = state;
		currentRunStats.secondsOfSurvival = TimeManager.Get().runDuration;
		foreach (string key in currentRunStats.damageSourceStatsMap.Keys)
		{
			DamageSourceStat damageSourceStat = new DamageSourceStat();
			damageSourceStat.sourceId = key;
			damageSourceStat.damage = currentRunStats.damageSourceStatsMap[key];
			currentRunStats.damageSourceStats.Add(damageSourceStat);
		}
	}

	public void AbandonRun()
	{
		state = RunState.Abandoned;
		OnEndRun();
	}

	public void OnEndRun()
	{
		Player.Get().GetComponent<CharacterController>().enableControlls.Push("RunManager", value: false);
		Player.Get().GetComponent<CollisionController>().enableCollision.Push("RunManager", value: false);
		if (state == RunState.Won)
		{
			PersistentDataComponent.GetDataInstance().CollectCurrency(currencyPerRunWon);
			PersistentDataComponent.GetDataInstance().maxSacrifices += maxSacrificesPerRunWon;
			PersistentDataComponent.GetDataInstance().UnlockLevel(PersistentDataComponent.GetDataInstance().currentlySelectedLevel + 1);
		}
		FinallizeCurrentRunStats();
		PersistentDataComponent.GetDataInstance().SaveRunStats(GetCurrentStats());
	}

	public int GetOrder()
	{
		return 1000;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.attacker is CharacterAttributesPlayer)
		{
			GetCurrentStats().highestDamageHit = Mathf.Max(GetCurrentStats().highestDamageHit, eventParameters.damage);
		}
		return IGameEventResult.SUCCESS;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
		GameEventManager.Get()?.OnCharacterDealingDamage.RemoveListener(this);
	}
}
