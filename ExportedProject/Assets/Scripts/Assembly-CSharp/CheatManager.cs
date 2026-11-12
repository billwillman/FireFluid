using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
	private List<Cheat> cheats;

	public bool enabledCheats;

	public string currentInput;

	private static CheatManager instance;

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		if (base.transform.parent == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
		cheats = new List<Cheat>();
		cheats.Add(new Cheat("showconsole", delegate
		{
			DebugConsole debugConsole = Object.FindFirstObjectByType<DebugConsole>();
			if ((bool)debugConsole)
			{
				debugConsole.active = !debugConsole.active;
			}
		}));
	}

	private void OnEnable()
	{
		Keyboard.current.onTextInput += GetKeyInput;
	}

	private void OnDisable()
	{
		Keyboard.current.onTextInput -= GetKeyInput;
	}

	private void GetKeyInput(char key)
	{
		currentInput += key;
	}

	private void Update()
	{
		if (currentInput.Length > 32)
		{
			currentInput = currentInput.Substring(currentInput.Length - 32, 32);
		}
		string text = "enablecheatsnowpls!thx";
		if (currentInput.Length >= text.Length && currentInput.Substring(currentInput.Length - text.Length, text.Length) == text)
		{
			Debug.Log("CHEATS ENABLED!");
			enabledCheats = true;
			currentInput = "";
		}
		if (!enabledCheats)
		{
			return;
		}
		foreach (Cheat cheat in cheats)
		{
			if (currentInput.Length >= cheat.cmd.Length && currentInput.Substring(currentInput.Length - cheat.cmd.Length, cheat.cmd.Length) == cheat.cmd)
			{
				cheat.action();
				currentInput = "";
				Debug.Log("CHEAT ACTIVATED: " + cheat.cmd);
				NotificationManager.PushNotification("<color=#ff2222>CHEAT ACTIVATED:</color> " + cheat.cmd);
			}
		}
	}
}
