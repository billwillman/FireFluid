using UnityEngine;

public class LootContainer : MonoBehaviour, IInteractable
{
	private Animator animator;

	private bool interacted;

	public float dropDelay = 1f;

	private bool lootDroped;

	public string openingSound = "";

	public LootTable dropTable;

	public int guaranteedDrops = 2;

	public int chanceDrops = 3;

	private float animationTime;

	private float animationJitter = 0.2f;

	public bool CanInteract()
	{
		return !interacted;
	}

	public void OnEndInteract()
	{
	}

	public bool OnInteract()
	{
		return true;
	}

	public void OnStartInteract(InteractionController c)
	{
		if ((bool)animator)
		{
			animator.Play("Open");
		}
		if (openingSound != "")
		{
			SoundEffectManager.Play(openingSound, base.transform.position);
		}
		interacted = true;
	}

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		if ((bool)animator)
		{
			animator.Play("Idle", 0, Random.value);
		}
	}

	private void DropLoot()
	{
		for (int i = 0; i < guaranteedDrops; i++)
		{
			dropTable.DropLoot(base.transform.position + Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up) * 2f, guaranteedDop: true);
		}
		for (int j = 0; j < chanceDrops; j++)
		{
			dropTable.DropLoot(base.transform.position + Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up) * 2f);
		}
		lootDroped = true;
	}

	private void Update()
	{
		if (interacted)
		{
			dropDelay -= Time.deltaTime;
			if (!lootDroped && (bool)animator)
			{
				float num = (Mathf.PerlinNoise1D(Time.time * 16f) * 2f - 1f) * animationJitter;
				animator.Play("Open", 0, animationTime + num);
				animationTime += Time.deltaTime;
			}
		}
		if (dropDelay < 0f && !lootDroped)
		{
			DropLoot();
		}
	}
}
