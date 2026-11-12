public class InputUIElementMove : InputUIElement
{
	public override void Update()
	{
		base.Update();
		actionText.text = StringLocalization.Get("inputaction_move");
		if (InputManager.MovePressedThisFrame())
		{
			inputUsed = true;
		}
	}

	public override bool IsShowing()
	{
		if (!base.IsShowing())
		{
			return false;
		}
		if (SceneManager.Get().currentlyLoadedScene == SceneIndex.OVERWORLD && PersistentDataComponent.GetDataInstance().GetNumberOfRuns() < 1)
		{
			return true;
		}
		return false;
	}
}
