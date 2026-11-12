using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
	public delegate void Function();

	public Event fuseEvolingsEventPrefab;

	public Event showEvopediaEventPrefab;

	public Event showHallOfFameEventPrefab;

	private static CinematicMachine cinematicMachine = new CinematicMachine();

	public float transitionDuration = 0.5f;

	private List<Function> functionsToExecuteBeforeSceneSwitch = new List<Function>();

	public SceneIndex currentlyLoadedScene = SceneIndex.TRANSITIONING;

	private static SceneManager instance = null;

	private bool transitionIn = true;

	private bool transitionOut = true;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		currentlyLoadedScene = (SceneIndex)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static SceneManager Get()
	{
		return instance;
	}

	public bool IsBusy()
	{
		return cinematicMachine.IsBusy();
	}

	public void Update()
	{
		cinematicMachine.UpdateActions(Time.unscaledDeltaTime);
	}

	public void ClearGameState()
	{
		transitionOut = true;
		transitionIn = true;
		functionsToExecuteBeforeSceneSwitch.Clear();
	}

	public void QueueBeforeSceneSwitch(Function f)
	{
		functionsToExecuteBeforeSceneSwitch.Add(f);
	}

	public void ExecuteFunctionsBeforeSceneSwitch()
	{
		foreach (Function item in functionsToExecuteBeforeSceneSwitch)
		{
			item();
		}
		functionsToExecuteBeforeSceneSwitch.Clear();
	}

	public void QueueTransitionOut(float transitionDuration)
	{
		bool init = false;
		InputManager.enableInputs.Push("transition", value: false);
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			if (!transitionOut)
			{
				CameraShutter.fade = 1f;
				return true;
			}
			if (!init)
			{
				init = true;
			}
			currentlyLoadedScene = SceneIndex.TRANSITIONING;
			if (transitionDuration > 0.01f)
			{
				CameraShutter.fade += dt / transitionDuration;
			}
			else
			{
				CameraShutter.fade = 1.1f;
			}
			if (CameraShutter.fade > 1f)
			{
				CameraShutter.fade = 1f;
				return true;
			}
			return false;
		}));
	}

	public void QueueTransitionIn(float transitionDuration)
	{
		bool init = false;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			if (!init)
			{
				init = true;
				InputManager.enableInputs.Pop("transition");
			}
			if (!transitionIn)
			{
				CameraShutter.fade = 1f;
				return true;
			}
			currentlyLoadedScene = SceneIndex.TRANSITIONING;
			if (transitionDuration > 0.01f)
			{
				CameraShutter.fade -= dt / transitionDuration;
			}
			else
			{
				CameraShutter.fade = -0.1f;
			}
			if (CameraShutter.fade < 0f)
			{
				CameraShutter.fade = 0f;
				return true;
			}
			return false;
		}));
	}

	public void LoadScene(SceneIndex scene)
	{
		switch (scene)
		{
		case SceneIndex.STARTSCREEN:
			LoadStartScreenScene();
			break;
		case SceneIndex.OVERWORLD:
			LoadOverworldScene();
			break;
		case SceneIndex.UNDERWORLD_1:
			LoadUnderworldScene(1);
			break;
		case SceneIndex.UNDERWORLD_2:
			LoadUnderworldScene(2);
			break;
		case SceneIndex.UNDERWORLD_3:
			LoadUnderworldScene(3);
			break;
		default:
			Debug.Log("Scene not found: " + scene);
			break;
		case SceneIndex.TRANSITIONING:
			break;
		}
	}

	private void Reset()
	{
		TimeManager.Get()?.paused.PopAll();
		Time.timeScale = 1f;
		UILayer.CloseAll();
	}

	public void LoadStartScreenScene()
	{
		QueueTransitionOut(transitionDuration);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ExecuteFunctionsBeforeSceneSwitch();
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			currentlyLoadedScene = SceneIndex.STARTSCREEN;
			Debug.Log("Loading Start Screen...");
			UnityEngine.SceneManagement.SceneManager.LoadScene(1);
			Reset();
			return true;
		}));
		QueueTransitionIn(transitionDuration);
	}

	public void LoadOverworldScene()
	{
		QueueTransitionOut(transitionDuration);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ExecuteFunctionsBeforeSceneSwitch();
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			currentlyLoadedScene = SceneIndex.OVERWORLD;
			Debug.Log("Loading Meta World...");
			UnityEngine.SceneManagement.SceneManager.LoadScene(2);
			Reset();
			return true;
		}));
		QueueTransitionIn(transitionDuration);
	}

	public void LoadUnderworldScene(int level)
	{
		QueueTransitionOut(transitionDuration);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ExecuteFunctionsBeforeSceneSwitch();
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			switch (level)
			{
			case 1:
				currentlyLoadedScene = SceneIndex.UNDERWORLD_1;
				break;
			case 2:
				currentlyLoadedScene = SceneIndex.UNDERWORLD_2;
				break;
			case 3:
				currentlyLoadedScene = SceneIndex.UNDERWORLD_3;
				break;
			}
			Debug.Log("Loading Run...");
			UnityEngine.SceneManagement.SceneManager.LoadScene((int)currentlyLoadedScene);
			RunManager.Get().state = RunState.NotInRun;
			Reset();
			return true;
		}));
		QueueTransitionIn(transitionDuration);
	}

	public bool IsInUnderworld()
	{
		if (currentlyLoadedScene == SceneIndex.UNDERWORLD_1)
		{
			return true;
		}
		if (currentlyLoadedScene == SceneIndex.UNDERWORLD_2)
		{
			return true;
		}
		if (currentlyLoadedScene == SceneIndex.UNDERWORLD_3)
		{
			return true;
		}
		return false;
	}

	public void RestartScene()
	{
		QueueTransitionOut(transitionDuration);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			ExecuteFunctionsBeforeSceneSwitch();
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
			return true;
		}));
		QueueTransitionIn(transitionDuration);
	}

	public void QuitApplication()
	{
		QuitApplication(savePersistantData: false, saveSettings: false);
	}

	public void QuitApplication(bool savePersistantData, bool saveSettings)
	{
		Application.Quit();
	}
}
