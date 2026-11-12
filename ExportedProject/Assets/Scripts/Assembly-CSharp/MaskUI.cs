using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaskUI : BlendInModelUI
{
	public Transform maskModelContainer;

	public CharacterMask mask;

	public TextMeshProUGUI text;

	private int noInputFrames;

	public GameObject upgradeCardUIPrefab;

	public Transform layoutGroup;

	private List<Upgrade> upgrades = new List<Upgrade>();

	private List<UpgradeCardUI> upgradeCards = new List<UpgradeCardUI>();

	public override void Start()
	{
		base.Start();
		text.text = "<color=#ddb888><size=150%>~ " + mask.GetDisplayName() + " ~</size></color>";
		upgrades.Clear();
		upgrades.AddRange(mask.upgrades);
		int num = 0;
		foreach (Upgrade upgrade in upgrades)
		{
			UpgradeCardUI componentInChildren = Object.Instantiate(upgradeCardUIPrefab, layoutGroup).GetComponentInChildren<UpgradeCardUI>();
			float num2 = ((float)num - (float)(upgrades.Count - 1) * 0.5f) * 10f;
			componentInChildren.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - num2);
			componentInChildren.SetUp(upgrade);
			upgradeCards.Add(componentInChildren);
			num++;
		}
		GameObject obj = Object.Instantiate(mask.highResModel, maskModelContainer);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
	}

	public override void Update()
	{
		base.Update();
		noInputFrames--;
		if (CutSceneManager.Get().IsBusy() || close)
		{
			noInputFrames = 3;
		}
		foreach (UpgradeCardUI upgradeCard in upgradeCards)
		{
			upgradeCard.UpdateState();
		}
	}

	public void EquipMask()
	{
		Player player = Player.Get();
		player.ResetToDefault();
		mask.Apply(player);
		mask.TriggerDialog();
	}
}
