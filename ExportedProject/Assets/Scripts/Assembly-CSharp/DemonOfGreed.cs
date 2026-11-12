using UnityEngine;

public class DemonOfGreed : Enemy, IInteractable, IGameEventListener<OnCharacterDealingDamageParams>, IGameEventListener<OnCharacterDeathParams>
{
	public float despawnRange = 128f;

	private bool interacted;

	private float movementTime;

	private float waitingTime;

	private Vector3 currentDir;

	private float dropTimer = 0.2f;

	private float currentDropTimer;

	public GameObject currencyPrefab;

	public GameObject xpGemPrefab;

	private Vector3 targetMovePosition;

	public bool CanInteract()
	{
		return !interacted;
	}

	public int GetOrder()
	{
		return -100;
	}

	public void OnEndInteract()
	{
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (!interacted && eventParameters.receiver == GetAttributes())
		{
			return IGameEventResult.PREVENTED;
		}
		return IGameEventResult.SUCCESS;
	}

	public bool OnInteract()
	{
		return true;
	}

	public void OnStartInteract(InteractionController c)
	{
		TextParticleManager.Show(StringLocalization.Get("demonofgreed_comeandgetme"), base.transform.position, Color.white, 64f, 4f);
		interacted = true;
		SoundEffectManager.Play("smalldemon.laugh");
	}

	public override void Start()
	{
		base.Start();
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
		GameEventManager.Get().OnCharacterDeath.AddListener(this);
	}

	public override void UpdateMovementDirection()
	{
		if (!interacted || Player.Get() == null || Player.Get().GetAttributes().health.IsExhausted())
		{
			movementDirection = Vector3.zero;
			return;
		}
		waitingTime -= Time.deltaTime;
		movementTime -= Time.deltaTime;
		currentDropTimer -= Time.deltaTime;
		if (waitingTime > 0f)
		{
			movementDirection = Vector3.zero;
			return;
		}
		if (movementTime > 0f && currentDropTimer < 0f)
		{
			currentDropTimer = dropTimer;
			GameObject gameObject = ((!(Random.value < 0.01f)) ? Object.Instantiate(xpGemPrefab) : Object.Instantiate(currencyPrefab));
			gameObject.transform.position = base.transform.position + Vector3.up * 2f;
			gameObject.AddComponent<DropAnimation>();
		}
		if (movementTime < 0f && waitingTime < 0f)
		{
			if (Random.value < 0.8f)
			{
				currentDir = (base.transform.position - Player.Get().transform.position).normalized;
				currentDir += -base.transform.position / 128f;
				currentDir.y = 0f;
				currentDir += Random.insideUnitSphere * 2f;
				currentDir.y = 0f;
				currentDir.Normalize();
				movementTime = Random.Range(0.1f, 1f);
				targetMovePosition = PositionProvider.Get().GetValidPositionNear(base.transform.position + currentDir * 64f);
			}
			else
			{
				waitingTime = Random.Range(0.1f, 3f);
			}
		}
		if (agent != null && agent.enabled && movementTime > 0f && waitingTime < 0f)
		{
			agent.CalculatePath(targetMovePosition, path);
			corners = path.corners;
			if (corners.Length > 1)
			{
				Vector3 vector = corners[1];
				movementDirection = Vector3.Lerp(movementDirection, (vector - base.transform.position).normalized, Mathf.Clamp01(Time.deltaTime * 8f));
				movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;
			}
			else
			{
				movementDirection = currentDir;
			}
			agent.nextPosition = base.transform.position;
		}
		else
		{
			movementDirection = currentDir;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
		GameEventManager.Get().OnCharacterDeath.RemoveListener(this);
	}

	public override void Update()
	{
		base.Update();
		if (interacted && Player.Get() != null && !Player.Get().GetAttributes().health.IsExhausted() && (Player.Get().transform.position - base.transform.position).magnitude > despawnRange)
		{
			Object.Destroy(base.gameObject);
			DialogManager.Get().ShowDialog(StringLocalization.Get("demonofgreed_escape"), "static");
		}
	}

	public IGameEventResult OnGameEvent(OnCharacterDeathParams eventParameters)
	{
		if (eventParameters.victim == GetAttributes())
		{
			UpgradeManager.Get()?.DrawSingleUpgrade(Player.Get());
		}
		return IGameEventResult.SUCCESS;
	}
}
