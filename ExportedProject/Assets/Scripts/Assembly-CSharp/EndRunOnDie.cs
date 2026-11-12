using UnityEngine;

public class EndRunOnDie : MonoBehaviour
{
	public string sourceOfDamage = "bossdeafeated";

	private CharacterAttributes attributes;

	private bool triggered;

	private void Start()
	{
		attributes = GetComponent<CharacterAttributes>();
	}

	private void Update()
	{
		if (!triggered && attributes.health.IsExhausted())
		{
			Enemy.KillAll(Player.Get().GetAttributes(), sourceOfDamage);
			Player.Get().GetAttributes().SetInvincibility(9999999f);
			triggered = true;
			RendererEffects.Highlight(base.gameObject, "EndRunOnDie", Color.red, 5f, EffectMode.Fade);
			Camera.main.GetComponent<CameraController>().Shake(2f);
			CutSceneManager.Get().PushWait(2f);
			CutSceneManager.Get().PushStopTime();
			CutSceneManager.Get().PushGrabCamera();
			CutSceneManager.Get().PushMoveCamera(base.transform.position, 32f);
			CutSceneManager.Get().PushResumeTime();
			CutSceneManager.Get().PushWait(4f);
			CutSceneManager.Get().PushMoveCameraToPlayer();
			CutSceneManager.Get().PushReleaseCamera();
			CutSceneManager.Get().cinematicMachine.PushAction(new CinematicFunction(delegate
			{
				RunWonUI.show = true;
				RunManager.Get().WinRun();
				return true;
			}));
		}
	}
}
