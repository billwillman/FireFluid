public class InputUIElementInteract : InputUIElement
{
	private InteractionController interactionController;

	public override void Start()
	{
		base.Start();
		interactionController = Player.Get().GetComponent<InteractionController>();
	}

	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_interact");
	}

	public override bool IsShowing()
	{
		if (!base.IsShowing())
		{
			return false;
		}
		if (interactionController.GetNearestInteractable() != null)
		{
			return true;
		}
		return false;
	}
}
