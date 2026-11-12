using UnityEngine;

[DefaultExecutionOrder(102)]
public class StartRunInteractable : MonoBehaviour, IInteractable
{
	public float animationTime = 5f;

	private float timer;

	private bool started;

	private bool switched;

	private InteractionController interactionController;

	public FluidEmitter emitter;

	private static bool runStarted;

	public bool CanInteract()
	{
		return true;
	}

	public void OnEndInteract()
	{
	}

	public bool OnInteract()
	{
		return false;
	}

	public void OnStartInteract(InteractionController c)
	{
		interactionController = c;
		Animator componentInChildren = c.GetComponentInChildren<Animator>();
		CharacterController componentInChildren2 = c.GetComponentInChildren<CharacterController>();
		componentInChildren2.enableControlls.Push("StartRunAnim", value: false);
		componentInChildren2.enabled = false;
		componentInChildren2.GetComponentInChildren<Rigidbody>().isKinematic = true;
		componentInChildren.speed = 1f;
		componentInChildren.Play("Meditate");
		started = true;
		runStarted = true;
	}

	private void AnimateEmitter()
	{
		if ((bool)emitter)
		{
			if (!started)
			{
				emitter.gameObject.SetActive(value: false);
				return;
			}
			float t = 1f - timer / animationTime;
			emitter.gameObject.SetActive(value: true);
			emitter.reaction = Vector3.Lerp(Vector3.zero, new Vector3(0.3f, 0f, 0f), t);
			emitter.noiseForce = Mathf.Lerp(0f, 0.5f, t);
			emitter.temperature = Mathf.Lerp(0f, 0.03f, t);
			emitter.rotationalForce = Mathf.Lerp(0f, 0.1f, t);
		}
	}

	private void Start()
	{
		timer = animationTime;
		if (!runStarted)
		{
			return;
		}
		runStarted = false;
		Player.Get().SetToPosition(base.transform.position + Vector3.up * 0.5f, Quaternion.Euler(0f, 180f, 0f));
		Player.Get().GetComponent<Hover>().UpdateSpring(100f);
		CharacterController cc = Player.Get().GetComponentInChildren<CharacterController>();
		Animator animator = Player.Get().GetComponentInChildren<Animator>();
		cc.animate.Push("wakeingup", value: false);
		animator.speed = 0f;
		animator.Play("Meditate", 0, 1f);
		CutSceneManager.Get().PushWait(1f);
		float t = 0f;
		CutSceneManager.Get().cinematicMachine.PushAction(new CinematicFunction(delegate(float dt)
		{
			t += dt * 0.5f;
			if (t > 1f)
			{
				return true;
			}
			animator.speed = 0f;
			animator.Play("Meditate", 0, Mathf.Clamp01(1f - t));
			return false;
		}));
		CutSceneManager.Get().cinematicMachine.PushAction(new CinematicFunction(delegate
		{
			cc.animate.PopAll("wakeingup");
			return true;
		}));
	}

	private void Update()
	{
		if (started)
		{
			timer -= Time.deltaTime;
		}
		AnimateEmitter();
		if (!switched && timer < 0f)
		{
			switched = true;
			SceneManager.Get().LoadUnderworldScene(PersistentDataComponent.GetDataInstance().currentlySelectedLevel);
		}
	}
}
