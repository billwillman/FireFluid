using UnityEngine;

[DefaultExecutionOrder(11)]
public class CutSceneIntro : MonoBehaviour
{
	private bool cutScenePlayed;

	public string dialogStyleId = "";

	[TextArea(3, 32)]
	public string dialog = "";

	public AudioClip audioClip;

	private void Awake()
	{
		cutScenePlayed = PersistentDataComponent.GetDataInstance().GetData("CutSceneIntro.cutScenePlayed") != null;
		if (!cutScenePlayed)
		{
			CameraShutter.fade = 1f;
		}
	}

	private void Start()
	{
		if (cutScenePlayed)
		{
			return;
		}
		CameraShutter.fade = 1f;
		CinematicMachine cinematicMachine = CutSceneManager.Get().cinematicMachine;
		float waitTime = 3f;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			CameraShutter.fade = 1f;
			waitTime -= dt;
			return waitTime < 0f;
		}));
		float fade = 1f;
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			fade -= dt * 0.2f;
			CameraShutter.fade = fade;
			if (CameraShutter.fade < 0.01f)
			{
				CameraShutter.fade = 0f;
				return true;
			}
			return false;
		}));
		CutSceneManager.Get().PushWait(2f);
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			DialogManager.Get().ShowDialog(dialog, dialogStyleId, audioClip);
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			PersistentDataComponent.GetDataInstance().SetData("CutSceneIntro.cutScenePlayed", true);
			return true;
		}));
	}

	private void Update()
	{
	}
}
