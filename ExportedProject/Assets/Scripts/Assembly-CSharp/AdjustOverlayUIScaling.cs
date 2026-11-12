using UnityEngine;
using UnityEngine.UI;

public class AdjustOverlayUIScaling : MonoBehaviour
{
	private float originalScaling;

	private CanvasScaler canvasScaler;

	private void Start()
	{
		canvasScaler = GetComponent<CanvasScaler>();
		originalScaling = canvasScaler.scaleFactor;
	}

	private void Update()
	{
		canvasScaler.scaleFactor = originalScaling * Settings.overlayUIScaling;
	}
}
