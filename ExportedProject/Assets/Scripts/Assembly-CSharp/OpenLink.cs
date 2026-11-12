using UnityEngine;

public class OpenLink : MonoBehaviour
{
	public string link = "https://store.steampowered.com/app/3845350/Ignitement/";

	public void Open()
	{
		Application.OpenURL(link);
	}
}
