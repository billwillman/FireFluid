using System.Globalization;
using UnityEngine;

public class Ability : MonoBehaviour, IAbilityStatus
{
	public string identifier = "ability";

	public Sprite icon;

	public AbilitySlot slot;

	public bool channeling;

	public float manaCost = 10f;

	public CoolDownBehaviour coolDownBehaviour;

	public float coolDown = 1f;

	protected float currentCoolDown;

	public int cooldownStacks = 1;

	private int currentCooldownStacks = 1;

	protected CharacterAttributes owner;

	private bool currentlyTriggering;

	public string triggerSound = "";

	public void Init(CharacterAttributes owner)
	{
		this.owner = owner;
		base.gameObject.layer = LayerHelper.GetAttackLayer(this.owner.gameObject.layer);
		currentCoolDown = GetCooldown();
	}

	public virtual void Start()
	{
		Init(GetComponentInParent<CharacterAttributes>());
	}

	public string GetIdentifier()
	{
		return identifier;
	}

	public string GetDisplayName()
	{
		return StringLocalization.Get("ability_" + GetIdentifier());
	}

	public virtual bool CanTrigger(AbilityCastParameters parameters)
	{
		if (CooldownIsExhausted())
		{
			return false;
		}
		if (owner != null && owner.health.IsExhausted())
		{
			return false;
		}
		if (!CanSpendMana())
		{
			return false;
		}
		return true;
	}

	public float GetCooldown()
	{
		return coolDown;
	}

	public float GetCurrentCooldown()
	{
		return currentCoolDown;
	}

	public float GetDisplayCooldown()
	{
		if (currentCooldownStacks > 0)
		{
			return 0f;
		}
		return GetCurrentCooldown();
	}

	public int GetCurrentCooldownStacks()
	{
		return currentCooldownStacks;
	}

	public CharacterAttributes GetOwner()
	{
		return owner;
	}

	public void ExhaustCooldown()
	{
		currentCooldownStacks--;
	}

	public virtual void DeductCooldown(float time)
	{
		currentCoolDown -= time;
	}

	public bool CooldownIsExhausted()
	{
		return currentCooldownStacks <= 0;
	}

	public int GetCooldownStacks()
	{
		return cooldownStacks;
	}

	public bool CooldownStacksFull()
	{
		return currentCooldownStacks == cooldownStacks;
	}

	public virtual bool CanSpendMana(bool indicateFailedSpendAttempt)
	{
		if (IsChanneling())
		{
			if ((bool)owner && !owner.mana.IsExhausted())
			{
				return true;
			}
		}
		else if (owner == null || owner.mana.CanSpend(manaCost, indicateFailedSpendAttempt))
		{
			return true;
		}
		return false;
	}

	public bool CanSpendMana()
	{
		return CanSpendMana(indicateFailedSpendAttempt: true);
	}

	public virtual void Update()
	{
		if (currentCooldownStacks < cooldownStacks)
		{
			currentCoolDown -= Time.deltaTime * owner.attackSpeed.GetCurrentValue();
			if (currentCoolDown <= 0f)
			{
				currentCoolDown += GetCooldown();
				currentCooldownStacks++;
			}
		}
		else
		{
			currentCoolDown = GetCooldown();
		}
	}

	public bool IsChanneling()
	{
		return channeling;
	}

	public virtual bool Trigger(AbilityCastParameters parameters)
	{
		if (coolDownBehaviour == CoolDownBehaviour.OnTrigger)
		{
			ExhaustCooldown();
		}
		if (!IsChanneling())
		{
			parameters.caster.mana.Spend(manaCost);
		}
		else
		{
			currentlyTriggering = true;
		}
		SoundEffectManager.Play(triggerSound, base.transform.position);
		return true;
	}

	public virtual bool TriggerUpdateChanneling(AbilityCastParameters parameters)
	{
		if (IsChanneling())
		{
			parameters.caster.mana.Spend(manaCost * Time.deltaTime, DynamicCharacterAttributeValueChangedType.gradual);
		}
		return true;
	}

	public virtual void TriggerEndChanneling()
	{
		if (coolDownBehaviour == CoolDownBehaviour.OnTriggerEnd)
		{
			ExhaustCooldown();
		}
		currentlyTriggering = false;
	}

	public bool CurrentlyTriggering()
	{
		return currentlyTriggering;
	}

	public virtual bool CurrentlyBlocking()
	{
		return false;
	}

	public Sprite GetIcon()
	{
		return icon;
	}

	public float GetSize()
	{
		if (slot != AbilitySlot.Passive)
		{
			return 1f;
		}
		return 0.5f;
	}

	public string GetToolTip()
	{
		string text = "";
		text = text + GetDisplayName() + "<br>";
		text = text + "<color=#888888>" + GetDisplayCooldown().ToString("F1", CultureInfo.InvariantCulture) + "<size=50%>s</size></color><br>";
		if (Mathf.Abs(manaCost) > 0.001f)
		{
			text = ((!IsChanneling()) ? (text + "<color=#666688><size=75%>" + StringLocalization.Get("attribute_manaCost") + ": " + Mathf.RoundToInt(manaCost) + "</size></color>") : (text + "<color=#666688><size=75%>" + StringLocalization.Get("attribute_manaCost") + ": " + Mathf.RoundToInt(manaCost) + "</size><size=50%>/s</size></color>"));
		}
		return text;
	}
}
