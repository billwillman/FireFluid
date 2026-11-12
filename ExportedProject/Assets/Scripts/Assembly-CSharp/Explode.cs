using UnityEngine;

public class Explode : Ability, IGameEventListener<OnCharacterDeathParams>
{
	public float damageFactor = 1f;

	public float impactRadius = 2f;

	public float timeOfImpact = 0.4f;

	protected bool attackTriggered;

	private float progress;

	public GameObject explosionPrefab;

	private Animator animator;

	private Vector3 animatorPosition;

	public override void Start()
	{
		base.Start();
		animator = owner.GetComponentInChildren<Animator>();
		animatorPosition = animator.transform.localPosition;
		GameEventManager.Get().OnCharacterDeath.AddListener(this);
	}

	public override bool Trigger(AbilityCastParameters parameters)
	{
		attackTriggered = true;
		progress = 0f;
		return base.Trigger(parameters);
	}

	public override void Update()
	{
		if (attackTriggered)
		{
			CustomDebug.DrawSphere(owner.transform.position, impactRadius, Color.red);
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			animator.transform.localPosition = animatorPosition + onUnitSphere * 0.1f;
			progress += Time.deltaTime * owner.attackSpeed.GetCurrentValue();
			if (progress > timeOfImpact)
			{
				attackTriggered = false;
				Damage.DoAoEDamage(owner.transform.position, impactRadius * Mathf.Sqrt(owner.area.GetCurrentValue()), owner.damage.GetCurrentValue() * damageFactor, owner, base.gameObject.layer);
				_ = Settings.showNumbers;
				owner.ReceiveDamage(1000000f, owner, GetIdentifier());
				GameObject obj = Object.Instantiate(explosionPrefab, owner.transform.position, Quaternion.Euler(90f, 0f, 0f));
				Vector3 localScale = obj.transform.localScale * impactRadius;
				obj.transform.localScale = localScale;
			}
		}
		base.Update();
	}

	public int GetOrder()
	{
		return 100;
	}

	public IGameEventResult OnGameEvent(OnCharacterDeathParams eventParameters)
	{
		if (eventParameters.victim != owner)
		{
			return IGameEventResult.SUCCESS;
		}
		if (eventParameters.killer == owner)
		{
			return IGameEventResult.SUCCESS;
		}
		Debug.Log(eventParameters.killer);
		if (!attackTriggered)
		{
			owner.health.enableRegeneration.Push(GetIdentifier(), value: false);
			owner.movementSpeed.value.Push(GetIdentifier(), 0f);
			attackTriggered = true;
			ExhaustCooldown();
			currentCoolDown = GetCooldown();
		}
		owner.health.SetDynamicValue(1f);
		return IGameEventResult.PREVENTED;
	}

	private void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDeath.RemoveListener(this);
	}
}
