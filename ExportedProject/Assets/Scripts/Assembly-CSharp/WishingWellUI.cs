using System.Collections.Generic;
using UnityEngine;

public class WishingWellUI : BlendInModelUI
{
	public GameObject eyes;

	public Animator leftArm;

	public Animator leftArmResting;

	public Animator rightArm;

	public int currencyUntilCreatureAppears = 10;

	public int currencyUntilMaskGained = 100;

	public List<string> tossCurrencyDialogs = new List<string>();

	public List<ConditionalDialogString> conditionalDialogs = new List<ConditionalDialogString>();

	private int noInputFrames;

	public List<GameObject> inputObjects;

	public const string MASK_UNLOCK_ID = "chaos";

	private bool playMaskUnlockedAnimation;

	public override void Start()
	{
		base.Start();
	}

	public void OnCurrencyToss()
	{
		if (noInputFrames > 0 || DialogManager.Get().IsInDialog())
		{
			return;
		}
		if (PersistentDataComponent.GetDataInstance().currencyTossedIntoWell >= 50)
		{
			DialogManager.Get().ShowDialog("<shake>" + StringLocalization.Get("wishingwell_demo") + "</shake>", "creatureinthewell");
		}
		else if (PersistentDataComponent.GetDataInstance().CanSpendCurrency(1))
		{
			PersistentDataComponent.GetDataInstance().currencyTossedIntoWell++;
			if (PersistentDataComponent.GetDataInstance().currencyTossedIntoWell >= currencyUntilMaskGained && PersistentDataComponent.GetDataInstance().GetData("chaos") == null)
			{
				playMaskUnlockedAnimation = true;
				PersistentDataComponent.GetDataInstance().SetData("chaos", true);
			}
			SoundEffectManager.Play("well.cointoss");
			PersistentDataComponent.GetDataInstance().SpendCurrency(1);
			TextParticleManager.Show("-1<sprite name=currency>", Player.Get().transform.position + Vector3.up * 4f);
			bool flag = false;
			foreach (ConditionalDialogString conditionalDialog in conditionalDialogs)
			{
				if (conditionalDialog.numberOfCurrencyRequired == PersistentDataComponent.GetDataInstance().currencyTossedIntoWell)
				{
					CutSceneManager.Get().PushDialog("<shake>" + StringLocalization.Parse(conditionalDialog.dialog) + "</shake>", "creatureinthewell");
					flag = true;
				}
			}
			if (flag)
			{
				InputManager.DisableInputs(2f);
				return;
			}
			DialogManager.Get().ShowDialog(tossCurrencyDialogs[Random.Range(0, tossCurrencyDialogs.Count)], "static");
			Close();
		}
		else
		{
			DialogManager.Get().ShowDialog(StringLocalization.Get("wishingwell_nothingtogive"), "static");
		}
	}

	private void UpdateCreatureParts()
	{
		if (eyes != null)
		{
			eyes.SetActive(PersistentDataComponent.GetDataInstance().currencyTossedIntoWell >= currencyUntilCreatureAppears);
		}
		float num = Mathf.Clamp01(1f * (float)(PersistentDataComponent.GetDataInstance().currencyTossedIntoWell - currencyUntilCreatureAppears) / (float)(currencyUntilMaskGained - currencyUntilCreatureAppears) - 0.05f);
		if (PersistentDataComponent.GetDataInstance().currencyTossedIntoWell >= currencyUntilMaskGained)
		{
			num = 1f;
		}
		PersistentDataComponent.GetDataInstance().GetData("chaos");
		if (num < 0f)
		{
			rightArm.gameObject.SetActive(value: false);
			leftArm.gameObject.SetActive(value: false);
			leftArmResting.gameObject.SetActive(value: false);
			return;
		}
		if (rightArm != null)
		{
			rightArm.gameObject.SetActive(value: true);
			rightArm.Play("GrabRim", 0, Mathf.Clamp01(num * 2f) + (Mathf.PerlinNoise1D(Time.unscaledTime + 77f) - 0.5f) * 0.1f + (Mathf.PerlinNoise1D(Time.unscaledTime * 8f + 77f) - 0.5f) * 0.01f);
		}
		if (PersistentDataComponent.GetDataInstance().GetData("chaos") != null && !playMaskUnlockedAnimation)
		{
			leftArm.gameObject.SetActive(value: false);
			leftArmResting.gameObject.SetActive(value: true);
			leftArmResting.Play("GrabRim", 0, Mathf.Clamp01(num * 2f) + (Mathf.PerlinNoise1D(Time.unscaledTime) - 0.5f) * 0.1f + (Mathf.PerlinNoise1D(Time.unscaledTime * 8f) - 0.5f) * 0.01f);
		}
		else
		{
			leftArmResting.gameObject.SetActive(value: false);
			leftArm.gameObject.SetActive(value: true);
			leftArm.Play("ReachUp", 0, Mathf.Clamp01(num * 2f - 1f) + (Mathf.PerlinNoise1D(Time.unscaledTime) - 0.5f) * 0.1f + (Mathf.PerlinNoise1D(Time.unscaledTime * 8f) - 0.5f) * 0.01f);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateCreatureParts();
		noInputFrames--;
		if (CutSceneManager.Get().IsBusy() || close)
		{
			noInputFrames = 3;
		}
		foreach (GameObject inputObject in inputObjects)
		{
			inputObject.SetActive(noInputFrames < 0);
		}
	}

	public override void Close()
	{
		base.Close();
		if (playMaskUnlockedAnimation)
		{
			CutSceneManager.Get().PushUnlockAnimation(StringLocalization.Get("dialog_maskunlocked"), new Vector3(1.24f, 0.3f, 34.22f));
		}
	}
}
