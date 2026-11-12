using UnityEngine;

public class SceneSwitchFunctions : MonoBehaviour
{
	public void LoadScene(SceneIndex scene)
	{
		SceneManager.Get().LoadScene(scene);
	}

	public void LoadOverworldScene()
	{
		if (RunManager.Get().state == RunState.Ongoing)
		{
			RunManager.Get().AbandonRun();
		}
		SceneManager.Get().LoadOverworldScene();
	}

	public void LoadStartScreenScene()
	{
		SceneManager.Get().LoadStartScreenScene();
	}

	public void RestartScene()
	{
		SceneManager.Get().RestartScene();
	}
}
