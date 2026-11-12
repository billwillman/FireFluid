using UnityEngine;

public class BuildInformation
{
	public static string version = Application.version;

	public static bool fullVersion = true;

	public static string GetVersionString()
	{
		string text = version;
		text += " (demo)";
		if (!fullVersion)
		{
			text += " (demo)";
		}
		return text;
	}
}
