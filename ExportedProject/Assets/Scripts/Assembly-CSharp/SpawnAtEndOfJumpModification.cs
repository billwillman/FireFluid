using System.Globalization;
using UnityEngine;

public class SpawnAtEndOfJumpModification : MonoBehaviour, IJumpModification, IAbilityStatus
{
	public string identifier = "jumpmodification";

	public Sprite icon;

	public float cooldown = 30f;

	public float currentCooldown;

	public GameObject prefab;

	private CharacterAttributes owner;

	private float reaction = 0.2f;

	public ValueStackFloat chanceToG = new ValueStackFloat(0f);

	public ValueStackFloat chanceToB = new ValueStackFloat(0f);

	private void Start()
	{
		owner = GetComponentInParent<CharacterAttributes>();
	}

	private void Update()
	{
		currentCooldown -= Time.deltaTime;
	}

	public void OnEndJump()
	{
		if (currentCooldown < 0f)
		{
			GameObject obj = Object.Instantiate(prefab, base.transform.position, Quaternion.identity);
			currentCooldown = cooldown;
			if (Random.value < chanceToG.GetCurrentValue())
			{
				ElementHelper.SetElement(obj, new Vector4(0f, reaction, 0f, 0f));
			}
			else if (Random.value < chanceToB.GetCurrentValue())
			{
				ElementHelper.SetElement(obj, new Vector4(0f, 0f, reaction, 0f));
			}
		}
	}

	public void OnStartJump()
	{
	}

	public string GetIdentifier()
	{
		return identifier;
	}

	public Sprite GetIcon()
	{
		return icon;
	}

	public float GetSize()
	{
		return 0.5f;
	}

	public string GetToolTip()
	{
		return string.Concat("" + StringLocalization.Get(identifier) + "<br>", "<color=#888888>", GetCurrentCooldown().ToString("F1", CultureInfo.InvariantCulture), "<size=50%>s</size></color>");
	}

	public float GetCooldown()
	{
		return cooldown;
	}

	public float GetCurrentCooldown()
	{
		return currentCooldown;
	}

	public int GetCooldownStacks()
	{
		return 1;
	}

	public int GetCurrentCooldownStacks()
	{
		if (!(currentCooldown > 0f))
		{
			return 1;
		}
		return 0;
	}

	public bool CooldownStacksFull()
	{
		return currentCooldown < 0f;
	}

	public bool CurrentlyTriggering()
	{
		return false;
	}

	public bool CooldownIsExhausted()
	{
		return currentCooldown > 0f;
	}

	public bool CanSpendMana(bool spendAttempt)
	{
		return true;
	}
}
