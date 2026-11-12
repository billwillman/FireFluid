using UnityEngine;

[DefaultExecutionOrder(103)]
public class MirrorImage : MonoBehaviour, IGameEventListener<OnCharacterDealingDamageParams>
{
	public float healthMultiplier = 5f;

	public float lifeTime = 5f;

	public bool copyMovement;

	public bool copyMainAttack = true;

	public bool copySecondary;

	public bool actAsDecoy = true;

	public GameObject spawnPrefab;

	private CharacterAttributes attributes;

	private CharacterController characterController;

	private Player player;

	private CharacterController playerCharacterController;

	private const string IDENTIFIER = "MirrorImage";

	private void Start()
	{
		attributes = GetComponent<CharacterAttributes>();
		player = Player.Get();
		characterController = GetComponent<CharacterController>();
		playerCharacterController = player.GetComponent<CharacterController>();
		attributes.health.value.Push("MirrorImage", player.GetAttributes().health.GetCurrentValue() * healthMultiplier);
		attributes.health.Fill();
		ValueModification.ShallowCopyAllValueStacks(playerCharacterController, characterController);
		if (copyMovement)
		{
			characterController.movementSpeedWhileAttackingMultiplier = playerCharacterController.movementSpeedWhileAttackingMultiplier;
			characterController.movementAcceleration = playerCharacterController.movementAcceleration;
		}
		else
		{
			characterController.movementSpeedWhileAttackingMultiplier = 0f;
			characterController.movementAcceleration = 0f;
		}
		if (copyMainAttack)
		{
			Ability ability = null;
			Ability[] componentsInChildren = player.GetComponentsInChildren<Ability>();
			foreach (Ability ability2 in componentsInChildren)
			{
				if (ability2.slot == AbilitySlot.Attack)
				{
					ability = ability2;
				}
			}
			GameObject gameObject = Object.Instantiate(ability.gameObject, base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			ValueModification.ShallowCopyAllValueStacks(ability, gameObject.GetComponent<Ability>());
		}
		if (copySecondary)
		{
			Ability ability3 = null;
			Ability[] componentsInChildren = player.GetComponentsInChildren<Ability>();
			foreach (Ability ability4 in componentsInChildren)
			{
				if (ability4.slot == AbilitySlot.SecondaryAction)
				{
					ability3 = ability4;
				}
			}
			GameObject gameObject2 = Object.Instantiate(ability3.gameObject, base.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			ValueModification.ShallowCopyAllValueStacks(ability3, gameObject2.GetComponent<Ability>());
		}
		if (actAsDecoy)
		{
			Player.decoys.Add(attributes);
		}
		GameEventManager.Get().OnCharacterDealingDamage.AddListener(this);
		Object.Instantiate(spawnPrefab, base.transform.position, Quaternion.identity);
	}

	private void Update()
	{
		bool showNumbers = Settings.showNumbers;
		Settings.showNumbers = false;
		attributes.ReceiveDamage(attributes.health.GetCurrentValue() * Time.deltaTime / lifeTime, attributes, "MirrorImage");
		Settings.showNumbers = showNumbers;
		characterController.enableAttacks.PopAll();
		characterController.enableAttacks.Push("MirrorImage", playerCharacterController.enableAttacks.GetCurrentValue());
		characterController.enableControlls.PopAll();
		characterController.enableControlls.Push("MirrorImage", playerCharacterController.enableControlls.GetCurrentValue());
	}

	private void OnDestroy()
	{
		GameEventManager.Get().OnCharacterDealingDamage.RemoveListener(this);
		Player.decoys.Remove(attributes);
	}

	public int GetOrder()
	{
		return 0;
	}

	public IGameEventResult OnGameEvent(OnCharacterDealingDamageParams eventParameters)
	{
		if (eventParameters.receiver == attributes && eventParameters.damageSource != "MirrorImage")
		{
			SoundEffectManager.Play("player.gethit", base.transform.position, base.transform);
		}
		return IGameEventResult.SUCCESS;
	}
}
