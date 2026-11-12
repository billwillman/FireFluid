using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundEffect : MonoBehaviour
{
	public string soundEffectId = "mysc.click";

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate
		{
			SoundEffectManager.Play(soundEffectId);
		});
	}
}
