using UnityEngine;

public class AddPointOfInterestLabel : MonoBehaviour
{
	public string text = "<sprite name=boss>";

	public bool markOnMiniMap = true;

	private void Start()
	{
		TextParticleManager.LabelPointOfInterest(text, base.transform);
		if (markOnMiniMap)
		{
			MiniMap.Get()?.Mark(base.transform, MiniMap.COLOR_POINTOFINTEREST);
		}
	}
}
