using System.Collections.Generic;
using UnityEngine;

public class Burden : MonoBehaviour
{
	public CharacterAttributes owner;

	private List<BurdenDebuff> debuffs = new List<BurdenDebuff>();

	private List<BurdenRequirement> requirements = new List<BurdenRequirement>();

	private List<BurdenReward> rewards = new List<BurdenReward>();

	private bool requiremensMet;

	private bool activated;

	private TextLabel textLabel;

	public new string name = "";

	public string flavorText = "";

	private StatusEffect statusEffectInstance;

	public void Finalize()
	{
		GetComponentsInChildren(debuffs);
		GetComponentsInChildren(requirements);
		GetComponentsInChildren(rewards);
		owner = GetComponentInParent<CharacterAttributes>();
	}

	public string GetName()
	{
		return name;
	}

	public string GetFlavorText()
	{
		return flavorText;
	}

	public string GetDebuffDescription()
	{
		string text = "";
		int num = 0;
		foreach (BurdenDebuff debuff in debuffs)
		{
			text += debuff.GetDescription();
			if (num != debuffs.Count - 1)
			{
				text += "<br>";
			}
			num++;
		}
		return text;
	}

	public string GetRewardDescription()
	{
		string text = "";
		int num = 0;
		foreach (BurdenReward reward in rewards)
		{
			text += reward.GetDescription();
			if (num != debuffs.Count - 1)
			{
				text += "<br>";
			}
			num++;
		}
		return text;
	}

	public string GetRequirementDescription()
	{
		string text = "";
		int num = 0;
		foreach (BurdenRequirement requirement in requirements)
		{
			text += requirement.GetDescription();
			if (num != debuffs.Count - 1)
			{
				text += "<br>";
			}
			num++;
		}
		return text;
	}

	public string GetShortDescription()
	{
		return string.Concat(string.Concat(string.Concat(string.Concat("" + "<b><size=75%><color=#aa8888>" + GetName() + "</color></b><br>", "<color=#888888>"), GetDebuffDescription(), "<br>"), StringLocalization.Get("demonofburdens_toliftthisburden").Replace("{requirement}", "<u>" + GetRequirementDescription() + "</u> "), "<br>"), "</color></size>");
	}

	public float GetProgress()
	{
		float num = 0f;
		foreach (BurdenRequirement requirement in requirements)
		{
			num += requirement.GetProgress();
		}
		return num / (float)requirements.Count;
	}

	private void Update()
	{
		requiremensMet = true;
		string text = "";
		foreach (BurdenRequirement requirement in requirements)
		{
			if (!requirement.IsMet())
			{
				requiremensMet = false;
			}
			text += requirement.GetLabelText();
		}
		if (statusEffectInstance != null)
		{
			statusEffectInstance.duration = 1f - GetProgress();
		}
		if (RequirementsMet())
		{
			Grant();
		}
		else if (owner.health.IsExhausted())
		{
			Failed();
		}
		else if (activated)
		{
			if (textLabel == null)
			{
				textLabel = TextParticleManager.Label(text, owner.transform, Color.clear);
			}
			textLabel.SetText(text);
		}
	}

	public void Activate()
	{
		foreach (BurdenDebuff debuff in debuffs)
		{
			debuff.Activate();
		}
		activated = true;
		StatusEffectBurden statusEffectBurden = new StatusEffectBurden();
		statusEffectBurden.burden = this;
		statusEffectInstance = StatusEffectManager.Apply(statusEffectBurden, Player.Get().GetAttributes(), Player.Get().GetAttributes());
	}

	public void Grant()
	{
		string text = "";
		int num = 0;
		foreach (BurdenReward reward in rewards)
		{
			reward.Grant();
			text += reward.GetDescription();
			if (num <= rewards.Count - 1)
			{
				text += "     ";
			}
		}
		SoundEffectManager.Play("demon.chimes");
		DialogManager.Get().ShowDialog(StringLocalization.Get("burden_lifted") + "  " + text, "static");
		Object.Destroy(base.gameObject);
	}

	public void Failed()
	{
		Object.Destroy(base.gameObject);
	}

	public bool RequirementsMet()
	{
		return requiremensMet;
	}

	private void OnDestroy()
	{
		if (textLabel != null)
		{
			Object.Destroy(textLabel.gameObject);
		}
		if (statusEffectInstance != null)
		{
			statusEffectInstance.Expire();
			statusEffectInstance = null;
		}
	}

	public static Burden GenerateRandomBurden()
	{
		return GenerateBurden(Random.Range(0, 5));
	}

	public static Burden GenerateBurden(int index)
	{
		GameObject gameObject = new GameObject("Burden");
		Burden burden = gameObject.AddComponent<Burden>();
		switch (index)
		{
		case 0:
			burden.name = StringLocalization.Get("burden_uncertainty_name");
			burden.flavorText = StringLocalization.Get("burden_uncertainty_flavortext");
			gameObject.AddComponent<BurdenDebuffDieOnDamage>();
			gameObject.AddComponent<BurdenRequirementKillEnemies>();
			gameObject.AddComponent<BurdenRewardUncertain>();
			break;
		case 1:
			burden.name = StringLocalization.Get("burden_numbness_name");
			burden.flavorText = StringLocalization.Get("burden_numbness_flavortext");
			gameObject.AddComponent<BurdenDebuffSlow>();
			gameObject.AddComponent<BurdenRequirementSurviveTime>();
			gameObject.AddComponent<BurdenRewardRandomAttribute>();
			break;
		case 2:
			burden.name = StringLocalization.Get("burden_ignorance_name");
			burden.flavorText = StringLocalization.Get("burden_ignorance_flavortext");
			gameObject.AddComponent<BurdenDebuffXPDamage>();
			gameObject.AddComponent<BurdenRequirementSurviveTime>();
			gameObject.AddComponent<BurdenRewardLevelUp>();
			break;
		case 3:
			burden.name = StringLocalization.Get("burden_pain_name");
			burden.flavorText = StringLocalization.Get("burden_pain_flavortext");
			gameObject.AddComponent<BurdenDebuffLifeDrain>();
			gameObject.AddComponent<BurdenRequirementSurviveTime>();
			gameObject.AddComponent<BurdenRewardLevelUp>();
			break;
		case 4:
			burden.name = StringLocalization.Get("burden_weakness_name");
			burden.flavorText = StringLocalization.Get("burden_weakness_flavortext");
			gameObject.AddComponent<BurdenDebuffNoDamage>();
			gameObject.AddComponent<BurdenRequirementSurviveTime>();
			gameObject.AddComponent<BurdenRewardCurrency>();
			break;
		}
		burden.transform.SetParent(Player.Get().transform);
		burden.transform.localPosition = Vector3.zero;
		burden.transform.localRotation = Quaternion.identity;
		return burden;
	}
}
