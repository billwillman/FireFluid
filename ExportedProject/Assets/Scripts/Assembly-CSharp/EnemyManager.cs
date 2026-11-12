using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public SpawnTable spawnTable;

	private SpawnTable spawnTableInstance;

	public int maxNumberOfEnemies = 500;

	private static EnemyManager instance;

	public bool enableSpawning = true;

	public bool enableRepositioning = true;

	public float maxEnemyCollisionRadius = 1.5f;

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		spawnTableInstance = Object.Instantiate(spawnTable, base.transform);
		if (Settings.performanceMode)
		{
			maxNumberOfEnemies /= 2;
		}
	}

	public static EnemyManager Get()
	{
		return instance;
	}

	private void UpdateSpawning(EnemyWave wave)
	{
		if (!enableSpawning || RunManager.Get().state != RunState.Ongoing || wave.currentCount >= (float)wave.count)
		{
			return;
		}
		wave.currentCoolDown -= Time.deltaTime;
		if (wave.currentCoolDown > 0f)
		{
			return;
		}
		BasicAI component = wave.prefab.GetComponent<BasicAI>();
		if ((bool)component)
		{
			if ((bool)SpawnEnemy(component))
			{
				wave.currentCoolDown = wave.duration / (float)wave.count;
				wave.currentCount += 1f;
			}
		}
		else if ((bool)SpawnObject(wave.prefab))
		{
			wave.currentCoolDown = wave.duration / (float)wave.count;
			wave.currentCount += 1f;
		}
	}

	public GameObject SpawnEnemy(BasicAI enemy)
	{
		if (enemy.boss || Enemy.all.Count < maxNumberOfEnemies)
		{
			GameObject gameObject = Object.Instantiate(enemy.gameObject, PositionProvider.Get().GetSpawnPosition(), Quaternion.identity);
			EnemySpawned(gameObject);
			return gameObject;
		}
		return null;
	}

	public GameObject SpawnObject(GameObject prefab)
	{
		if (Enemy.all.Count < maxNumberOfEnemies)
		{
			return Object.Instantiate(prefab, PositionProvider.Get().GetSpawnPosition(), Quaternion.identity);
		}
		return null;
	}

	public void EnemySpawned(GameObject enemy)
	{
		OnEnemySpawnedParams onEnemySpawnedParams = new OnEnemySpawnedParams();
		onEnemySpawnedParams.enemy = enemy.GetComponent<CharacterAttributes>();
		GameEventManager.Get().OnEnemySpawned.Invoke(onEnemySpawnedParams);
		EnemyColliderUtility.RestrictColliderRadius(enemy, maxEnemyCollisionRadius);
		EnemyColliderUtility.RestrictNavAgentRadius(enemy, maxEnemyCollisionRadius * 1.2f);
	}

	private void UpdateRepositioning()
	{
		if (Enemy.all.Count < 1)
		{
			return;
		}
		int num = 4;
		float num2 = 4096f;
		for (int i = 0; i < num; i++)
		{
			Enemy enemy = Enemy.all[Random.Range(0, Enemy.all.Count)];
			if (((object)enemy == null || !enemy.boss) && !(enemy is DemonOfGreed) && (enemy.transform.position - Player.Get().transform.position).sqrMagnitude > num2)
			{
				enemy.transform.position = PositionProvider.Get().GetSpawnPosition();
				if ((object)enemy != null)
				{
					enemy.positionIsDirty = true;
				}
			}
		}
	}

	private void Update()
	{
		foreach (EnemyWave wave in spawnTableInstance.waves)
		{
			if (TimeManager.Get().time >= wave.startTime)
			{
				UpdateSpawning(wave);
			}
		}
		if (enableRepositioning)
		{
			UpdateRepositioning();
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.fontSize = 18;
		gUIStyle.normal.textColor = Color.white;
		if (Settings.showFps)
		{
			GUI.Label(new Rect(172f, 0f, 512f, 64f), "Enemies:" + Enemy.all.Count, gUIStyle);
		}
	}
}
