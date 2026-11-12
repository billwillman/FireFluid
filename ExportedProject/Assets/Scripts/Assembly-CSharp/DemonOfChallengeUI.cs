using TMPro;
using UnityEngine;

public class DemonOfChallengeUI : BlendInModelUI
{
	public TextMeshProUGUI flavorText;

	public DemonOfChallengeInteractable demonOfChallengeInteractable;

	private void InitUI()
	{
		flavorText.text = "<color=#ddb888><size=150%>~ " + StringLocalization.Get("demonofchallenge_name") + " ~</size></color><br><br><color=#bbbbbb><i>" + StringLocalization.Get("demonofchallenge_flavortext") + "</i></color>";
	}

	public override void Awake()
	{
		startingPositionOffset = -Vector3.up * 32f;
		pauseTime = true;
		enableManualClose = false;
		valueModifierId = "demonofchallenge";
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		InitUI();
	}

	public override void Update()
	{
		base.Update();
		IsResponsive();
	}

	public void Accept()
	{
		Close();
	}

	public void Decline()
	{
		Close();
	}
}
