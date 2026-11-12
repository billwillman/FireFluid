public interface IInteractable
{
	bool CanInteract();

	void OnStartInteract(InteractionController c);

	bool OnInteract();

	void OnEndInteract();
}
