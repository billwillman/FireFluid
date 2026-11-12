public class InputUIElementLevelUp : InputUIElement
{
	private CharacterAttributesPlayer characterAttributesPlayer;

	public override void Start()
	{
		base.Start();
		characterAttributesPlayer = Player.Get().GetAttributes();
	}

	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_levelup");
	}

	public override bool IsShowing()
	{
		if (!base.IsShowing())
		{
			return false;
		}
		if ((bool)RunManager.Get() && RunManager.Get().state == RunState.Ongoing && characterAttributesPlayer.unspendLevelUps > 0)
		{
			return true;
		}
		return false;
	}
}
