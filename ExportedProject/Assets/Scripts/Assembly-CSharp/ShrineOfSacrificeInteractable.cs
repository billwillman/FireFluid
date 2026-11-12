public class ShrineOfSacrificeInteractable : InstantiatePrefabInteractable
{
	private void Start()
	{
	}

	public override void OnStartInteract(InteractionController c)
	{
		DialogManager.Get().ShowDialog(StringLocalization.Get("dialog_lockedindemo"), "static");
	}
}
