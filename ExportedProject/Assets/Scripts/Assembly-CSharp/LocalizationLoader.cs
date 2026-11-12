using TMPro;
using UnityEngine;

public class LocalizationLoader : MonoBehaviour
{
	public bool switchSceneOnDoneLoading = true;

	private bool transitionQueued;

	public TextMeshProUGUI progressDisplay;

	private void Start()
	{
		StringLocalization.ValidateTable();
	}

	private void Update()
	{
		progressDisplay.text = (int)(StringLocalization.GetLoadingProgress() * 100f) + "%";
	}
}
