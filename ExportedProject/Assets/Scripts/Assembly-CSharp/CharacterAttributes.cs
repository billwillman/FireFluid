using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributes : MonoBehaviour, IDamageable
{
	public static List<CharacterAttributes> all = new List<CharacterAttributes>();

	public DynamicCharacterAttribute health = new DynamicCharacterAttribute(100f);

	public DynamicCharacterAttribute mana = new DynamicCharacterAttribute(100f);

	public CharacterAttribute damage = new CharacterAttribute(100f);

	public CharacterAttribute attackSpeed = new CharacterAttribute(1f, hidden: false, 0f);

	public CharacterAttribute movementSpeed = new CharacterAttribute(2f, hidden: false, 0f, 24f);

	public CharacterAttribute area = new CharacterAttribute(1f);

	private static float averagePlayerDamage = 16f;

	public virtual void Start()
	{
		mana.Fill();
		health.Fill();
		all.Add(this);
	}

	public virtual void Update()
	{
		if (!health.IsExhausted())
		{
			mana.Update();
			health.Update();
		}
	}

	public virtual void OnDestroy()
	{
		all.Remove(this);
	}

	public virtual float ReceiveDamage(float dmg, CharacterAttributes attacker, string damageSource = "")
	{
		if (health.IsExhausted())
		{
			return 0f;
		}
		if (attacker == null)
		{
			Debug.LogError("attacker is null for damage source [" + damageSource + "]. That should never be the case!");
		}
		OnCharacterDealingDamageParams onCharacterDealingDamageParams = new OnCharacterDealingDamageParams();
		onCharacterDealingDamageParams.attacker = attacker;
		onCharacterDealingDamageParams.receiver = this;
		onCharacterDealingDamageParams.damage = dmg;
		onCharacterDealingDamageParams.damageSource = damageSource;
		if (GameEventManager.Get().OnCharacterDealingDamage.Invoke(onCharacterDealingDamageParams) == IGameEventResult.SUCCESS)
		{
			bool num = health.IsExhausted();
			health.Spend(onCharacterDealingDamageParams.damage);
			if (!num && health.IsExhausted())
			{
				OnCharacterDeathParams onCharacterDeathParams = new OnCharacterDeathParams();
				onCharacterDeathParams.killer = onCharacterDealingDamageParams.attacker;
				onCharacterDeathParams.victim = this;
				if (GameEventManager.Get().OnCharacterDeath.Invoke(onCharacterDeathParams) == IGameEventResult.SUCCESS)
				{
					IKillable[] componentsInChildren = GetComponentsInChildren<IKillable>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].OnKill();
					}
				}
			}
			if (Settings.showNumbers)
			{
				if (this is CharacterAttributesPlayer)
				{
					float size = 64f;
					TextParticleManager.Show(Mathf.RoundToInt(onCharacterDealingDamageParams.damage).ToString("0.##"), base.transform, Color.red, size);
				}
				else if (attacker is CharacterAttributesPlayer)
				{
					averagePlayerDamage = Mathf.Lerp(averagePlayerDamage, onCharacterDealingDamageParams.damage, 0.1f);
					float value = 64f;
					if ((bool)RunManager.Get())
					{
						value = Mathf.LerpUnclamped(16f, 32f, onCharacterDealingDamageParams.damage / averagePlayerDamage);
					}
					value = Mathf.Clamp(value, 4f, 128f);
					TextParticleManager.Show(Mathf.RoundToInt(onCharacterDealingDamageParams.damage).ToString("0.##"), base.transform, TextAttributes.GetColorFromDamageSource(damageSource), value);
				}
			}
		}
		else
		{
			onCharacterDealingDamageParams.damage = 0f;
		}
		return onCharacterDealingDamageParams.damage;
	}
}
