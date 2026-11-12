using UnityEngine;

public class BurdenRequirementKillEnemies : BurdenRequirement, IGameEventListener<OnCharacterDeathParams>
{
	public int currentAmount;

	public int amount = 66;

	private int layerPlayer;

	private int layerPlayerSummon;

	public override string GetDescription()
	{
		return StringLocalization.Get("burden_requirement_slayenemies").Replace("{amount}", (amount - currentAmount).ToString() ?? "");
	}

	public override string GetLabelText()
	{
		return amount - currentAmount + "<sprite name=skull>";
	}

	public override float GetProgress()
	{
		return 1f * (float)currentAmount / (float)amount;
	}

	public int GetOrder()
	{
		return 100;
	}

	public override bool IsMet()
	{
		return currentAmount >= amount;
	}

	public IGameEventResult OnGameEvent(OnCharacterDeathParams eventParameters)
	{
		if (eventParameters.killer != null && (eventParameters.killer.gameObject.layer == layerPlayer || eventParameters.killer.gameObject.layer == layerPlayerSummon))
		{
			currentAmount++;
		}
		return IGameEventResult.SUCCESS;
	}

	public override void Start()
	{
		base.Start();
		layerPlayer = LayerMask.NameToLayer("Player");
		layerPlayerSummon = LayerMask.NameToLayer("PlayerSummon");
		GameEventManager.Get().OnCharacterDeath.AddListener(this);
	}

	private void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDeath.RemoveListener(this);
	}
}
