using UnityEngine;

[DefaultExecutionOrder(100)]
public class CharacterAttributesPlayer : CharacterAttributes, IGameEventListener<OnCharacterDealingDamageParams>
{
	public CharacterAttribute luck = new CharacterAttribute(10f, hidden: true);

	public CharacterAttribute poisonDamageMultiplier = new CharacterAttribute(0.5f, hidden: true);

	public CharacterAttribute burnDamageToFrozenMultiplier = new CharacterAttribute(1f, hidden: true);

	public CharacterAttribute chanceToG = new CharacterAttribute(0f, hidden: true);

	public CharacterAttribute chanceToB = new CharacterAttribute(0f, hidden: true);

	[Header("Critical Damage")]
	public CharacterAttribute criticalHitChance = new CharacterAttribute(0f, hidden: true);

	public CharacterAttribute criticalHitDamage = new CharacterAttribute(1f, hidden: true);

	public CharacterAttribute maxMultiCrits = new CharacterAttribute(1f, hidden: true);

	public const string DAMAGE_SOURCE_CRIT = "critical";

	[Header("Leveling")]
	public int level = 1;

	public float experience;

	public CharacterAttribute pickUpRadius = new CharacterAttribute(4f);

	public GameObject levelUpPrefab;

	public LevelUpMethod levelUpMethod;

	public int unspendLevelUps;

	[Header("Mysc")]
	public CharacterAttribute invincibilityTimeAfterReceivingDamage = new CharacterAttribute(0.5f, hidden: true);

	private float currentInvincibilityTime;

	private Vector4 element;

	private RendererEffects rendererEffects;

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
		rendererEffects = RendererEffects.Get(base.gameObject);
	}

	public override void Update()
	{
		UpdateHighlighting();
		base.Update();
		if (experience > ExperienceNeededToLevelUp())
		{
			experience -= ExperienceNeededToLevelUp();
			level++;
			unspendLevelUps++;
			Debug.Log("Level Up!");
			if (levelUpPrefab != null)
			{
				Object.Instantiate(levelUpPrefab, base.transform.position, Quaternion.identity).transform.SetParent(base.transform);
			}
			SetInvincibility(2f);
			rendererEffects.PlayEffect("levelup", new Color(1f, 0.6f, 0.1f, 1f), 2f, EffectMode.SquareWave, 8f);
			SoundEffectManager.Play("player.levelup");
			ILevelUpable[] componentsInChildren = GetComponentsInChildren<ILevelUpable>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnLevelUp(level);
			}
		}
		currentInvincibilityTime -= Time.deltaTime;
		UpdateElement();
	}

	public void UpdateHighlighting()
	{
		if (mana.HasFailedSpendAttempt() || mana.IsExhausted())
		{
			rendererEffects.PlayEffect("manaexhausted", new Color(0f, 0.1f, 0.7f), 0.5f, EffectMode.SawTooth);
		}
	}

	public void UpdateElement()
	{
		float currentValue = chanceToG.GetCurrentValue();
		float currentValue2 = chanceToB.GetCurrentValue();
		element = Vector4.Lerp(element, new Vector4(Mathf.Max(0f, 1f - currentValue - currentValue2), currentValue, currentValue2, 0f).normalized, Time.deltaTime * 2f);
	}

	public override float ReceiveDamage(float dmg, CharacterAttributes attacker, string causeOfDamage)
	{
		if (currentInvincibilityTime > 0f)
		{
			return 0f;
		}
		if (health.IsExhausted())
		{
			return base.ReceiveDamage(dmg, attacker);
		}
		float num = base.ReceiveDamage(dmg, attacker);
		if (num > 0.1f)
		{
			currentInvincibilityTime = invincibilityTimeAfterReceivingDamage.GetCurrentValue();
			rendererEffects.PlayEffect("receivedamage", Color.red, currentInvincibilityTime, EffectMode.SquareWave, 16f);
		}
		if (health.IsExhausted())
		{
			TimeManager.Get().Freeze(0.3f);
		}
		else
		{
			TimeManager.Get().Freeze(0.1f);
		}
		SoundEffectManager.Play("player.gethit", base.transform.position, base.transform);
		return num;
	}

	public void SetInvincibility(float time)
	{
		currentInvincibilityTime = time;
	}

	public void GainXP(float xp)
	{
		experience += xp;
	}

	public float ExperienceNeededToLevelUp()
	{
		return 10f * (float)level;
	}

	public Vector4 GetElement()
	{
		return element;
	}

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.damageSource == "burn")
		{
			int num = Mathf.RoundToInt(maxMultiCrits.GetCurrentValue());
			int num2 = 0;
			while (Random.value < criticalHitChance.GetCurrentValue() && num2 < num)
			{
				eventParameters.damage *= 1f + criticalHitDamage.GetCurrentValue();
				eventParameters.damageSource = "critical";
				num2++;
			}
		}
		return IGameEventResult.SUCCESS;
	}

	public override void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
		base.OnDestroy();
	}
}
