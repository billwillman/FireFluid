using UnityEngine;

public class DebugConsole : MonoBehaviour
{
	private static string myLog = "";

	private string output;

	private string stack;

	public bool active;

	private void OnEnable()
	{
		Application.logMessageReceived += Log;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= Log;
	}

	public void Log(string logString, string stackTrace, LogType type)
	{
		output = logString;
		stack = stackTrace;
		myLog = output + "\n" + myLog;
		if (myLog.Length > 5000)
		{
			myLog = myLog.Substring(0, 4000);
		}
	}

	private void OnGUI()
	{
		if (active)
		{
			GUI.Label(new Rect(10f, 10f, Screen.width - 10, Screen.height - 10), myLog);
		}
	}
}
