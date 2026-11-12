using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TwitchCommandDefinitions : MonoBehaviour
{
	public GameObject frostNova;

	public GameObject label;

	private Dictionary<string, int> enemyCosts;

	private Dictionary<string, UserCommandData> userCommandData = new Dictionary<string, UserCommandData>();

	private static TwitchCommandDefinitions instance;

	private TwitchPointsManager pointsManager;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		enemyCosts = new Dictionary<string, int>();
		foreach (Enemy enemy in Encyclopedia.Get().enemies)
		{
			enemyCosts.Add(enemy.GetIdentifier(), (int)enemy.GetComponent<CharacterAttributes>().health.GetCurrentBaseValue() / 5);
		}
	}

	private void Start()
	{
		pointsManager = GetComponent<TwitchPointsManager>();
		pointsManager.AddCommand(new TwitchCommandSpawn());
		pointsManager.AddCommand(new TwitchCommandCast("frostnova", frostNova, 200, "Frost Nova"));
		pointsManager.AddCommand(new TwitchCommandDamage());
		pointsManager.AddCommand(new TwitchCommandHeal());
		pointsManager.AddCommand(new TwitchCommandLevelUp());
		pointsManager.AddCommand(new TwitchCommandShake());
		pointsManager.AddCommand(new TwitchCommandBurden());
		GetComponent<TwitchManager>().GetClient().OnChatMessage += HandleChatMessage;
	}

	public void HandleChatMessage(string user, string msg)
	{
		if (!msg.StartsWith("!") && !msg.StartsWith("?"))
		{
			UnityThread.Enqueue(delegate
			{
				MakeNearestEnemyTalk(user, msg);
			});
		}
	}

	public static TwitchCommandDefinitions Get()
	{
		return instance;
	}

	public void Update()
	{
		UpdateUserCommandData();
		pointsManager.enableCommands = SceneManager.Get().IsInUnderworld();
		pointsManager.enableGatheringPoints = SceneManager.Get().IsInUnderworld();
	}

	public void UpdateUserCommandData()
	{
		List<Enemy> list = new List<Enemy>();
		List<string> list2 = new List<string>();
		foreach (string key in userCommandData.Keys)
		{
			list.Clear();
			List<Enemy> enemies = userCommandData[key].enemies;
			foreach (Enemy item in enemies)
			{
				if (item == null || item.GetAttributes().health.IsExhausted())
				{
					list.Add(item);
				}
			}
			foreach (Enemy item2 in list)
			{
				enemies.Remove(item2);
			}
			if (enemies.Count == 0)
			{
				list2.Add(key);
			}
		}
	}

	public void MakeNearestEnemyTalk(string user, string msg)
	{
		if (!userCommandData.TryGetValue(user, out var value) || value.enemies.Count <= 0)
		{
			return;
		}
		List<Enemy> enemies = value.enemies;
		Enemy enemy = null;
		float num = float.MaxValue;
		foreach (Enemy item in enemies)
		{
			float sqrMagnitude = (Player.Get().transform.position - item.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				enemy = item;
				num = sqrMagnitude;
			}
		}
		if (enemy != null)
		{
			TextParticleManager.Show("<color=#FFBB44><b>" + msg + "</b></color>", enemy.transform.position + Vector3.up * 2f, 4f);
		}
	}

	public int GetEnemyCost(string enemyId)
	{
		if (enemyCosts.ContainsKey(enemyId))
		{
			return enemyCosts[enemyId];
		}
		return 0;
	}

	public void SpawnEnemies(string user, string enemyId, int count)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> " + string.Format("sends <color={0}><b>{1} {2}</b></color> into battle!<br>", "#FF69B4", count, enemyId));
		Enemy enemy = Encyclopedia.Get().GetEnemy(enemyId);
		if (enemy == null)
		{
			Debug.LogError("ENEMY NOT FOUND IN ENCYCLOPEDIA: " + enemyId);
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = EnemyManager.Get().SpawnEnemy(enemy.GetComponent<BasicAI>());
			if (gameObject != null)
			{
				CharacterAttributes component = gameObject.GetComponent<CharacterAttributes>();
				component.health.value.Push("TWITCHCOMMAND", 2f, ValueModifierType.Multiply);
				component.health.Fill();
			}
			if (!userCommandData.TryGetValue(user, out var value))
			{
				value = new UserCommandData();
				userCommandData[user] = value;
			}
			value.enemies.Add(gameObject.GetComponent<Enemy>());
			LabelObject(gameObject, user);
		}
	}

	public void LabelObject(GameObject obj, string label)
	{
		GameObject obj2 = Object.Instantiate(this.label);
		obj2.GetComponent<FollowTransform>().target = obj.transform;
		obj2.GetComponentInChildren<TextMeshProUGUI>().text = label;
	}

	public void Heal(string user, int amount)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> " + string.Format("heals <color={0}><b>{1} Damage</b></color> from you!<br>", "#FF69B4", amount));
		Player.Get().GetComponent<CharacterAttributes>().health.Fill(amount);
	}

	public void DealDamage(string user, int dmg)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> " + string.Format("deals <color={0}><b>{1} Damage</b></color> to you!<br>", "#FF69B4", dmg));
		Player.Get().GetComponent<CharacterAttributes>().health.Spend(Mathf.Min(Player.Get().GetComponent<CharacterAttributes>().health.GetCurrentValue() - 10f, dmg));
	}

	public void Shake(string user)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> <color=#FF69B4><b>Shakes You</b></color>!<br>");
		Camera.main.GetComponent<CameraController>().Shake(3f);
	}

	public void LevelUp(string user)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> gifts a <color=#FF69B4><b>Level Up</b></color>!<br>");
		Player.Get().GetComponent<CharacterAttributesPlayer>().GainXP(Player.Get().GetComponent<CharacterAttributesPlayer>().ExperienceNeededToLevelUp() + 1f);
	}

	public void Cast(string user, GameObject prefab)
	{
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> casts <color=#FF69B4><b>Frost Nova</b></color>!<br>");
		Object.Instantiate(prefab, Player.Get().transform.position, Quaternion.identity);
	}

	public void InflictRandomBurden(string user)
	{
		if ((bool)Player.Get().GetComponentInChildren<Burden>())
		{
			TwitchManager.Instance.GetClient().SendMessage("@" + user + ", there is already a burden active! Please don't kill me like that!");
			return;
		}
		Burden burden = Burden.GenerateRandomBurden();
		burden.Finalize();
		NotificationManager.PushNotification("<b><color=#FFD700>" + user + "</color></b> inflicts <color=#FF69B4><b>" + burden.GetName() + "</b></color>!<br>");
		burden.Activate();
	}
}
