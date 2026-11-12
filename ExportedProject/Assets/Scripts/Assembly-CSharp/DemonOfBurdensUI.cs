using TMPro;
using UnityEngine;

public class DemonOfBurdensUI : BlendInModelUI
{
	public TextMeshProUGUI flavorText;

	public TextMeshProUGUI debuffText;

	public TextMeshProUGUI rewardText;

	public TextMeshProUGUI requirementText;

	private Burden burden;

	public DemonOfBurdensInteractable demonOfBurdensInteractable;

	public override void Awake()
	{
		valueModifierId = "demonofburdens";
		startingPositionOffset = -Vector3.up * 32f;
		startingRotationOffset = Quaternion.identity;
		pauseTime = true;
		enableManualClose = false;
		base.Awake();
	}

	private void GenerateBurden()
	{
		burden = Burden.GenerateRandomBurden();
	}

	private void InitUI()
	{
		flavorText.text = "<color=#ddb888><size=150%>~ " + burden.GetName() + " ~</size></color><br><color=#bbbbbb><i>" + burden.GetFlavorText() + "</i></color>";
		debuffText.text = burden.GetDebuffDescription();
		rewardText.text = burden.GetRewardDescription();
		requirementText.text = StringLocalization.Get("demonofburdens_toliftthisburden").Replace("{requirement}", "<u>" + burden.GetRequirementDescription() + "</u> ");
	}

	public override void Start()
	{
		base.Start();
		GenerateBurden();
		burden.Finalize();
		InitUI();
	}

	public override void Close()
	{
		base.Close();
		SoundEffectManager.Play("mysc.brush");
	}

	public void Accept()
	{
		burden.Activate();
		demonOfBurdensInteractable.Despawn();
		SoundEffectManager.Play("demon.laugh");
		Close();
	}

	public void Decline()
	{
		Object.Destroy(burden.gameObject);
		demonOfBurdensInteractable.Despawn();
		Close();
	}
}
