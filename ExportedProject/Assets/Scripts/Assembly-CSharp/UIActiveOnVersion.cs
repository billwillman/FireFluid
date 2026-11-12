using UnityEngine.UI;

public class UIActiveOnVersion : UIActiveOnPlatform
{
	public bool demo = true;

	public bool fullversion;

	public override void Awake()
	{
		selectable = GetComponent<Selectable>();
		SetActive(demo);
	}

	private void Update()
	{
	}
}
