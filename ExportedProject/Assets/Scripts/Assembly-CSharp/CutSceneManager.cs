using UnityEngine;

[DefaultExecutionOrder(10)]
public class CutSceneManager : MonoBehaviour
{
	public CinematicMachine cinematicMachine = new CinematicMachine();

	private CharacterController playerCharacterController;

	private static CutSceneManager instance;

	private int bufferFrames = 3;

	public const string valueModifierId = "CutSceneManager";

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public static CutSceneManager Get()
	{
		return instance;
	}

	private void Start()
	{
		playerCharacterController = Player.Get().GetComponent<CharacterController>();
	}

	public void PushWait(float time)
	{
		cinematicMachine.PushAction(new CinematicWait(time));
	}

	public void PushDialog(string text, string dialogStyleId = "", AudioClip audioClip = null)
	{
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			DialogManager.Get().ShowDialog(text, dialogStyleId, audioClip);
			return true;
		}));
		cinematicMachine.PushAction(new CinematicFunction((float dt) => !DialogManager.Get().IsInDialog()));
	}

	public void PushGrabCamera()
	{
		CameraController cameraController = Camera.main.GetComponent<CameraController>();
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			cameraController.enableController.Push("CutSceneManager", value: false);
			return true;
		}));
	}

	public void PushUnlockAnimation(string unlockDialog, Vector3 position)
	{
		Get().PushWait(2f);
		Get().PushGrabCamera();
		Get().PushMoveCamera(position);
		Get().PushDialog(unlockDialog, "static");
		Get().PushMoveCameraToPlayer();
		Get().PushReleaseCamera();
	}

	public void PushReleaseCamera()
	{
		CameraController cameraController = Camera.main.GetComponent<CameraController>();
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			cameraController.enableController.PopAll("CutSceneManager");
			return true;
		}));
	}

	public void PushMoveCamera(Vector3 target, float speed = 16f)
	{
		CameraController cameraController = Camera.main.GetComponent<CameraController>();
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			Vector3 targetCameraPosition = cameraController.GetTargetCameraPosition(target);
			Vector3 vector = targetCameraPosition - cameraController.transform.position;
			float magnitude = vector.magnitude;
			if (magnitude > speed * dt)
			{
				cameraController.transform.position += vector / magnitude * speed * dt;
				return false;
			}
			cameraController.transform.position = targetCameraPosition;
			return true;
		}));
	}

	public void PushMoveCameraToPlayer(float speed = 16f)
	{
		CameraController cameraController = Camera.main.GetComponent<CameraController>();
		cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			Vector3 targetCameraPosition = cameraController.GetTargetCameraPosition(Player.Get().transform.position);
			Vector3 vector = targetCameraPosition - cameraController.transform.position;
			float magnitude = vector.magnitude;
			if (magnitude > speed * dt)
			{
				cameraController.transform.position += vector / magnitude * speed * dt;
				return false;
			}
			cameraController.transform.position = targetCameraPosition;
			return true;
		}));
	}

	public void PushStopTime()
	{
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			TimeManager.Get().paused.Push("CutSceneManager", value: true);
			return true;
		}));
	}

	public void PushResumeTime()
	{
		cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			TimeManager.Get().paused.PopAll("CutSceneManager");
			return true;
		}));
	}

	private void Update()
	{
		if (bufferFrames > 0)
		{
			cinematicMachine.UpdateActions(0f);
		}
		else
		{
			cinematicMachine.UpdateActions(Time.unscaledDeltaTime);
		}
		bufferFrames--;
		playerCharacterController.enableControlls.PopAll("CutSceneManager");
		if (cinematicMachine.IsBusy())
		{
			CameraShutter.bars += Time.deltaTime * 0.2f;
			playerCharacterController.enableControlls.Push("CutSceneManager", value: false);
		}
		else
		{
			CameraShutter.bars -= Time.deltaTime * 0.2f;
		}
		CameraShutter.bars = Mathf.Clamp(CameraShutter.bars, 0f, 0.15f);
	}

	public bool IsBusy()
	{
		return cinematicMachine.IsBusy();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
