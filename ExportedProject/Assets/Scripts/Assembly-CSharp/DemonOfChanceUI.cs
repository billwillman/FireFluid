using TMPro;
using UnityEngine;

public class DemonOfChanceUI : BlendInModelUI
{
	public TextMeshProUGUI flavorText;

	public TextMeshProUGUI questionText;

	public TextMeshProUGUI rewardText;

	public TextMeshProUGUI punishmentText;

	public GameObject interactions;

	public DemonOfChanceInteractable demonOfChanceInteractable;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private Animator animator;

	private BurdenReward reward;

	private BurdenReward punishment;

	private bool doubleOrNothing;

	public float chanceOfDoubleOrNothing = 0.5f;

	private void InitUI()
	{
		flavorText.text = "<color=#ddb888><size=150%>~ " + StringLocalization.Get("demonofchance_name") + " ~</size></color><br><br><color=#bbbbbb><i>" + StringLocalization.Get("demonofchance_flavortext") + "</i></color>";
		rewardText.text = reward.GetDescription();
		punishmentText.text = punishment.GetDescription();
	}

	public override void Awake()
	{
		startingPositionOffset = -Vector3.up * 32f;
		pauseTime = true;
		animator = GetComponentInChildren<Animator>();
		animator.SetInteger("state", 0);
		animator.Play("Idle");
		enableManualClose = false;
		valueModifierId = "demonofchance";
		switch (Random.Range(0, 4))
		{
		case 0:
		{
			BurdenRewardCurrency burdenRewardCurrency = base.gameObject.AddComponent<BurdenRewardCurrency>();
			burdenRewardCurrency.amount = 3;
			BurdenRewardCurrency burdenRewardCurrency2 = base.gameObject.AddComponent<BurdenRewardCurrency>();
			burdenRewardCurrency2.amount = -3;
			reward = burdenRewardCurrency;
			punishment = burdenRewardCurrency2;
			break;
		}
		case 1:
		{
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement5 = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement5.contains = "damageMultiplier";
			burdenRewardRandomAttributeWithRequirement5.value = 0.15f;
			burdenRewardRandomAttributeWithRequirement5.GenerateAttributeModifications(includeHidden: true);
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement6 = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement6.contains = "damageMultiplier";
			burdenRewardRandomAttributeWithRequirement6.value = -0.15f;
			burdenRewardRandomAttributeWithRequirement6.GenerateAttributeModifications(includeHidden: true);
			reward = burdenRewardRandomAttributeWithRequirement5;
			punishment = burdenRewardRandomAttributeWithRequirement6;
			break;
		}
		case 2:
		{
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement3 = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement3.contains = "health";
			burdenRewardRandomAttributeWithRequirement3.value = 0.3f;
			burdenRewardRandomAttributeWithRequirement3.GenerateAttributeModifications(includeHidden: true);
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement4 = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement4.contains = "health";
			burdenRewardRandomAttributeWithRequirement4.value = -0.3f;
			burdenRewardRandomAttributeWithRequirement4.GenerateAttributeModifications(includeHidden: true);
			reward = burdenRewardRandomAttributeWithRequirement3;
			punishment = burdenRewardRandomAttributeWithRequirement4;
			break;
		}
		case 3:
		{
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement.contains = "luck";
			burdenRewardRandomAttributeWithRequirement.value = 7f;
			burdenRewardRandomAttributeWithRequirement.GenerateAttributeModifications(includeHidden: true, ValueModifierType.Add, AttributeModificationDisplayStyle.Int);
			BurdenRewardRandomAttributeWithRequirement burdenRewardRandomAttributeWithRequirement2 = base.gameObject.AddComponent<BurdenRewardRandomAttributeWithRequirement>();
			burdenRewardRandomAttributeWithRequirement2.contains = "luck";
			burdenRewardRandomAttributeWithRequirement2.value = -7f;
			burdenRewardRandomAttributeWithRequirement2.GenerateAttributeModifications(includeHidden: true, ValueModifierType.Add, AttributeModificationDisplayStyle.Int);
			reward = burdenRewardRandomAttributeWithRequirement;
			punishment = burdenRewardRandomAttributeWithRequirement2;
			break;
		}
		}
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		InitUI();
	}

	public override void Update()
	{
		cinematicMachine.UpdateActions(Time.unscaledDeltaTime);
		interactions.SetActive(!cinematicMachine.IsBusy() && !DialogManager.Get().IsInDialog());
		base.Update();
		IsResponsive();
	}

	[ContextMenu("Throw Coin")]
	public void ThrowCoin()
	{
		float num = Mathf.Clamp01(0.6f + Player.Get().GetAttributes().luck.GetCurrentValue() * 0.01f) - (doubleOrNothing ? 0.3f : 0f);
		bool heads = Random.value < num;
		float t = 0f;
		bool coinSoundPlayed = false;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			t += dt;
			if (!coinSoundPlayed && t > 0.85f)
			{
				SoundEffectManager.Play("demon.cointoss");
				coinSoundPlayed = true;
			}
			return coinSoundPlayed;
		}, blocking: false));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (heads)
			{
				animator.SetInteger("state", 1);
			}
			else
			{
				animator.SetInteger("state", 2);
			}
			return true;
		}));
		cinematicMachine.PushAction(new CinematicWait(3f));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (heads)
			{
				reward.Grant();
				TextParticleManager.Show("<color=#aaffaa>" + reward.GetDescription() + "</color>", Player.Get().transform.position + Vector3.up * 2f + Random.insideUnitSphere);
				SoundEffectManager.Play("mysc.success");
			}
			else
			{
				punishment.Grant();
				TextParticleManager.Show("<color=#ffaaaa>" + punishment.GetDescription() + "</color>", Player.Get().transform.position + Vector3.up * 2f + Random.insideUnitSphere);
				SoundEffectManager.Play("mysc.fail");
			}
			return true;
		}));
		cinematicMachine.PushAction(new CinematicWait(2f));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			if (heads && !doubleOrNothing && Random.value < chanceOfDoubleOrNothing)
			{
				animator.SetInteger("state", 0);
				animator.Play("Idle");
				DialogManager.Get().ShowDialog("<shake>" + StringLocalization.Parse(StringLocalization.Get("demonofchance_doubleornothing")) + "</shake>", "demonofchance");
				questionText.text = StringLocalization.Get("demonofchance_doubleornothing");
				doubleOrNothing = true;
			}
			else
			{
				demonOfChanceInteractable.Despawn();
				Close();
			}
			return true;
		}));
	}

	public void Decline()
	{
		demonOfChanceInteractable.Despawn();
		Close();
	}
}
