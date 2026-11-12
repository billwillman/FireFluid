using UnityEngine;

public class Shield : Ability, IGameEventListener<OnCharacterDealingDamageParams>
{
	public Material shieldMaterial;

	private float intensity;

	public GameObject onHitPrefab;

	public float impactRadius = 0.5f;

	public float damageReflect = 0.5f;

	public ValueStackFloat chanceToG = new ValueStackFloat(0f);

	public ValueStackFloat chanceToB = new ValueStackFloat(0f);

	public Color redColor;

	public Color greenColor;

	public Color blueColor;

	public const string DAMAGE_SOURCE = "shieldreflect";

	public ValueStackFloat damageMultiplier = new ValueStackFloat(2f);

	public ValueStackBool damageOnTouch = new ValueStackBool(defaultValue: false);

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public void Destroy()
	{
		GameEventManager.Get()?.OnCharacterDealingDamage.RemoveListener(this);
	}

	public int GetOrder()
	{
		return -100;
	}

	public Vector3 GetElement()
	{
		Vector4 element = Player.Get().GetAttributes().GetElement();
		float currentValue = chanceToG.GetCurrentValue();
		float currentValue2 = chanceToB.GetCurrentValue();
		float x = Mathf.Clamp01(1f - currentValue - currentValue2);
		Vector4 infuse = new Vector4(x, currentValue, currentValue2, 0f);
		infuse = ElementHelper.InfuseElement(element, infuse);
		return infuse;
	}

	public void SpawnOnHitObject(Vector3 position)
	{
		ExplosionHelper.Spawn(onHitPrefab, position, impactRadius * base.transform.localScale.x * Mathf.Sqrt(owner.area.GetCurrentValue()), GetElement(), owner, LayerMask.NameToLayer("PlayerAttack"), damageMultiplier.GetCurrentValue());
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.receiver != owner)
		{
			return IGameEventResult.SUCCESS;
		}
		if (eventParameters.attacker == eventParameters.receiver)
		{
			return IGameEventResult.SUCCESS;
		}
		if (CurrentlyTriggering() && eventParameters.damageSource != "shieldreflect")
		{
			if (eventParameters.damage > 0.01f)
			{
				eventParameters.attacker.ReceiveDamage(eventParameters.damage * damageReflect, owner, "shieldreflect");
			}
			SpawnOnHitObject(eventParameters.attacker.transform.position);
			intensity = 1f;
			return IGameEventResult.PREVENTED;
		}
		return IGameEventResult.SUCCESS;
	}

	public override void Update()
	{
		base.Update();
		if (CurrentlyTriggering())
		{
			intensity = Mathf.Lerp(intensity, 0.2f, Time.deltaTime * 4f);
			if (!owner.mana.enableRegeneration.Has(GetIdentifier()))
			{
				owner.mana.enableRegeneration.Push(GetIdentifier(), value: false);
			}
		}
		else
		{
			intensity = Mathf.Lerp(intensity, 0f, Time.deltaTime * 4f);
			owner.mana.enableRegeneration.Pop(GetIdentifier());
		}
		if ((bool)shieldMaterial)
		{
			Vector3 element = GetElement();
			Color color = element.x * redColor + element.y * greenColor + element.z * blueColor;
			Color color2 = shieldMaterial.GetColor("_Color");
			color2.r = color.r;
			color2.g = color.g;
			color2.b = color.b;
			color2.a = intensity * 0.7f;
			shieldMaterial.SetColor("_Color", color2);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (CurrentlyTriggering() && damageOnTouch.GetCurrentValue())
		{
			Vector3 vector = collider.transform.position - base.transform.position;
			SpawnOnHitObject(base.transform.position + vector.normalized * 2f);
		}
	}
}
