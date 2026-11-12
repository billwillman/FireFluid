using TMPro;
using UnityEngine;

public class MaxSacrificesGainedForRunWonLabel : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = "+ " + RunManager.maxSacrificesPerRunWon + " <sprite name=blood>";
	}
}
