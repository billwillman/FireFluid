using UnityEngine;

public class DisableCharacterControlls : MonoBehaviour
{
	private void Start()
	{
		Player.Get().GetComponent<CharacterController>().enableControlls.Push("DisableCharacterControlls", value: false);
	}

	private void OnDestroy()
	{
		Player.Get().GetComponent<CharacterController>().enableControlls.PopAll("DisableCharacterControlls");
	}
}
