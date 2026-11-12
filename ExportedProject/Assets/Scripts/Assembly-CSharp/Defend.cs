using UnityEngine;

public class Defend : Ability, IGameEventListener<OnCharacterDealingDamageParams>, IAnimatedAbility
{
	public float damageFactorWhileDefending = 0.8f;

	private FluidObstacle obstacle;

	private bool defending;

	public string switchToDefendAnimation = "";

	private float switchToDefendAnimationTime;

	private bool animating;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		defending = true;
		switchToDefendAnimationTime = 0f;
		obstacle.gameObject.SetActive(value: true);
		return base.Trigger(parameters);
	}

	public override void TriggerEndChanneling()
	{
		obstacle.gameObject.SetActive(value: false);
		defending = false;
		switchToDefendAnimationTime = 1f;
		base.TriggerEndChanneling();
	}

	public override void Start()
	{
		base.Start();
		obstacle = GetComponentInChildren<FluidObstacle>(includeInactive: true);
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
	}

	public int GetOrder()
	{
		return -1;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (defending && eventParameters.receiver == owner)
		{
			eventParameters.damage *= damageFactorWhileDefending;
		}
		return IGameEventResult.SUCCESS;
	}

	private void OnDestroy()
	{
		GameEventManager.Get()?.OnCharacterDealingDamage.RemoveListener(this);
	}

	public AnimationTakeOver Animate(Animator animator)
	{
		if (switchToDefendAnimation == "")
		{
			return AnimationTakeOver.None;
		}
		if (defending)
		{
			switchToDefendAnimationTime += Time.deltaTime;
		}
		else
		{
			switchToDefendAnimationTime -= Time.deltaTime;
		}
		if (switchToDefendAnimationTime > -0.1f && switchToDefendAnimationTime < 1.1f)
		{
			animator.speed = 0f;
			animator.Play(switchToDefendAnimation, 0, switchToDefendAnimationTime);
			animating = true;
			return AnimationTakeOver.Animation;
		}
		return AnimationTakeOver.None;
	}
}
