using UnityEngine;

public class GameEventManager : MonoBehaviour
{
	public GameEvent<OnCharacterDeathParams> OnCharacterDeath = new GameEvent<OnCharacterDeathParams>();

	public GameEvent<OnCharacterDealingDamageParams> OnCharacterDealingDamage = new GameEvent<OnCharacterDealingDamageParams>();

	public GameEvent<OnGemAttractedParams> OnGemAttracted = new GameEvent<OnGemAttractedParams>();

	public GameEvent<OnGemCollectedParams> OnGemCollected = new GameEvent<OnGemCollectedParams>();

	public GameEvent<OnEnemySpawnedParams> OnEnemySpawned = new GameEvent<OnEnemySpawnedParams>();

	private static GameEventManager instance;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
	}

	public static GameEventManager Get()
	{
		if (instance == null)
		{
			instance = new GameObject("GameEventManager").AddComponent<GameEventManager>();
		}
		return instance;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
