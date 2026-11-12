using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FluidEmitter : FluidEmitterRenderable
{
	[Tooltip("if the emitter is moved additional velocity of the movement is added.")]
	public bool inheritEmitterVelocity = true;

	public float emitterVelocityScaling = 8f;

	private Vector3 emitterVelocity = Vector3.zero;

	public new static List<FluidEmitter> all = new List<FluidEmitter>();

	public float outwardsForce;

	public float noiseForce;

	public float rotationalForce;

	[Range(-1f, 1f)]
	public float velocityX;

	[Range(-1f, 1f)]
	public float velocityY;

	[Range(-1f, 1f)]
	public float temperature = 1f;

	private float outwardsForceLastFrame;

	private float noiseForceLastFrame;

	private float rotationalForceLastFrame;

	private float velocityXLastFrame;

	private float velocityYLastFrame;

	private float temperatureLastFrame;

	private Vector3 reactionLastFrame = Vector3.zero;

	public Vector3 reaction = new Vector3(0.1f, 0f, 0f);

	[HideInInspector]
	public Vector3 positionLastFrame;

	private static Material reactionMaterial;

	private static Material temperatureMaterial;

	private static Material velocityMaterial;

	private GameObject temperatureEmitter;

	private GameObject velocityEmitter;

	private SpriteRenderer spriteRenderer;

	private SpriteRenderer temperatureSpriteRenderer;

	private SpriteRenderer velocitySpriteRenderer;

	private MaterialPropertyBlock reactionMPB;

	private MaterialPropertyBlock temperatureMPB;

	private MaterialPropertyBlock velocityMPB;

	private static readonly int ReactionScaleID = Shader.PropertyToID("_ReactionScale");

	private static readonly int TemperatureScaleID = Shader.PropertyToID("_TemperatureScale");

	private static readonly int OutwardsForceID = Shader.PropertyToID("_OutwardsForce");

	private static readonly int NoiseForceID = Shader.PropertyToID("_NoiseForce");

	private static readonly int RotationalForceID = Shader.PropertyToID("_RotationalForce");

	private static readonly int VelocityXID = Shader.PropertyToID("_VelocityX");

	private static readonly int VelocityYID = Shader.PropertyToID("_VelocityY");

	private void CheckRenderers()
	{
		if (!Mathf.Approximately(reaction.x, 0f) || !Mathf.Approximately(reaction.y, 0f) || !Mathf.Approximately(reaction.z, 0f))
		{
			spriteRenderer.enabled = true;
		}
		else
		{
			spriteRenderer.enabled = false;
		}
		if (!Mathf.Approximately(velocityX, 0f) || !Mathf.Approximately(velocityY, 0f) || !Mathf.Approximately(outwardsForce, 0f) || !Mathf.Approximately(rotationalForce, 0f) || !Mathf.Approximately(noiseForce, 0f))
		{
			velocitySpriteRenderer.enabled = true;
		}
		else
		{
			velocitySpriteRenderer.enabled = false;
		}
		if (!Mathf.Approximately(temperature, 0f))
		{
			temperatureSpriteRenderer.enabled = true;
		}
		else
		{
			temperatureSpriteRenderer.enabled = false;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (reactionMaterial == null)
		{
			reactionMaterial = new Material(Shader.Find("FluidSim/EmitterReaction"));
		}
		if (temperatureMaterial == null)
		{
			temperatureMaterial = new Material(Shader.Find("FluidSim/EmitterTemperature"));
		}
		if (velocityMaterial == null)
		{
			velocityMaterial = new Material(Shader.Find("FluidSim/EmitterVelocity"));
		}
		reactionMPB = new MaterialPropertyBlock();
		temperatureMPB = new MaterialPropertyBlock();
		velocityMPB = new MaterialPropertyBlock();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material = reactionMaterial;
		temperatureEmitter = new GameObject("temperatureEmitter");
		temperatureEmitter.transform.SetParent(base.transform);
		temperatureEmitter.transform.localPosition = Vector3.zero;
		temperatureEmitter.transform.localScale = Vector3.one;
		temperatureEmitter.transform.localRotation = Quaternion.identity;
		temperatureSpriteRenderer = temperatureEmitter.AddComponent<SpriteRenderer>();
		temperatureSpriteRenderer.sprite = spriteRenderer.sprite;
		temperatureSpriteRenderer.material = temperatureMaterial;
		velocityEmitter = new GameObject("velocityEmitter");
		velocityEmitter.transform.SetParent(base.transform);
		velocityEmitter.transform.localPosition = Vector3.zero;
		velocityEmitter.transform.localScale = Vector3.one;
		velocityEmitter.transform.localRotation = Quaternion.identity;
		velocitySpriteRenderer = velocityEmitter.AddComponent<SpriteRenderer>();
		velocitySpriteRenderer.sprite = spriteRenderer.sprite;
		velocitySpriteRenderer.material = velocityMaterial;
		positionLastFrame = base.transform.position;
		base.gameObject.layer = FluidSim.emitterReactionLayer;
		temperatureEmitter.layer = FluidSim.emitterTemperatureLayer;
		velocityEmitter.layer = FluidSim.emitterVelocityLayer;
		ForceSetMaterialProperties();
		all.Add(this);
	}

	public override void Start()
	{
		base.Start();
	}

	private void LateUpdate()
	{
		emitterVelocity = (base.transform.position - positionLastFrame) / Time.deltaTime;
		positionLastFrame = base.transform.position;
		CheckRenderers();
		SetMaterialProperties();
	}

	private void OnEnable()
	{
		EnsureRuntime();
		base.OnEnable();
		ForceSetMaterialProperties();
	}

	public void PushProperties()
	{
		EnsureRuntime();
		if (spriteRenderer == null || reactionMPB == null)
		{
			return;
		}
		CheckRenderers();
		ForceSetMaterialProperties();
		RefreshRenderers();
	}

	private void EnsureRuntime()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		if (spriteRenderer == null)
		{
			return;
		}
		if (reactionMaterial == null)
		{
			reactionMaterial = new Material(Shader.Find("FluidSim/EmitterReaction"));
		}
		if (temperatureMaterial == null)
		{
			temperatureMaterial = new Material(Shader.Find("FluidSim/EmitterTemperature"));
		}
		if (velocityMaterial == null)
		{
			velocityMaterial = new Material(Shader.Find("FluidSim/EmitterVelocity"));
		}
		if (reactionMPB == null)
		{
			reactionMPB = new MaterialPropertyBlock();
		}
		if (temperatureMPB == null)
		{
			temperatureMPB = new MaterialPropertyBlock();
		}
		if (velocityMPB == null)
		{
			velocityMPB = new MaterialPropertyBlock();
		}
		spriteRenderer.sharedMaterial = reactionMaterial;
		gameObject.layer = FluidSim.emitterReactionLayer;
		temperatureSpriteRenderer = FindChildRenderer("temperatureEmitter", temperatureMaterial, FluidSim.emitterTemperatureLayer, ref temperatureEmitter);
		velocitySpriteRenderer = FindChildRenderer("velocityEmitter", velocityMaterial, FluidSim.emitterVelocityLayer, ref velocityEmitter);
		if (!all.Contains(this))
		{
			all.Add(this);
		}
	}

	private SpriteRenderer FindChildRenderer(string childName, Material material, int layer, ref GameObject child)
	{
		if (child == null)
		{
			Transform found = transform.Find(childName);
			if (found != null)
			{
				child = found.gameObject;
			}
		}
		if (child == null)
		{
			child = new GameObject(childName);
			child.transform.SetParent(transform, false);
		}
		child.layer = layer;
		SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();
		if (childRenderer == null)
		{
			childRenderer = child.AddComponent<SpriteRenderer>();
		}
		childRenderer.sprite = spriteRenderer.sprite;
		childRenderer.sharedMaterial = material;
		return childRenderer;
	}

	private void ForceSetMaterialProperties()
	{
		reactionMPB.SetVector(ReactionScaleID, reaction);
		spriteRenderer.SetPropertyBlock(reactionMPB);
		temperatureMPB.SetFloat(TemperatureScaleID, temperature);
		temperatureSpriteRenderer.SetPropertyBlock(temperatureMPB);
		Vector2 vector = new Vector2(velocityX, velocityY);
		velocityMPB.SetFloat(VelocityXID, vector.x);
		velocityMPB.SetFloat(VelocityYID, vector.y);
		velocityMPB.SetFloat(OutwardsForceID, outwardsForce * base.transform.localScale.x);
		velocityMPB.SetFloat(NoiseForceID, noiseForce);
		velocityMPB.SetFloat(RotationalForceID, rotationalForce * base.transform.localScale.x);
		velocitySpriteRenderer.SetPropertyBlock(velocityMPB);
	}

	private void SetMaterialProperties()
	{
		if (!Mathf.Approximately(reaction.x, reactionLastFrame.x) || !Mathf.Approximately(reaction.y, reactionLastFrame.y) || !Mathf.Approximately(reaction.z, reactionLastFrame.z))
		{
			reactionMPB.SetVector(ReactionScaleID, reaction);
			spriteRenderer.SetPropertyBlock(reactionMPB);
			reactionLastFrame = reaction;
		}
		if (!Mathf.Approximately(temperature, temperatureLastFrame))
		{
			temperatureMPB.SetFloat(TemperatureScaleID, temperature);
			temperatureSpriteRenderer.SetPropertyBlock(temperatureMPB);
			temperatureLastFrame = temperature;
		}
		Vector2 vector = new Vector2(velocityX, velocityY);
		if (inheritEmitterVelocity && FluidSim.main != null)
		{
			vector += FluidSim.main.WorldToSimVelocity(emitterVelocity) * emitterVelocityScaling;
		}
		if (!Mathf.Approximately(vector.x, velocityXLastFrame) || !Mathf.Approximately(vector.y, velocityYLastFrame) || !Mathf.Approximately(outwardsForce, outwardsForceLastFrame) || !Mathf.Approximately(noiseForce, noiseForceLastFrame) || !Mathf.Approximately(rotationalForce, rotationalForceLastFrame))
		{
			velocityMPB.SetFloat(VelocityXID, vector.x);
			velocityMPB.SetFloat(VelocityYID, vector.y);
			velocityMPB.SetFloat(OutwardsForceID, outwardsForce * base.transform.localScale.x);
			velocityMPB.SetFloat(NoiseForceID, noiseForce);
			velocityMPB.SetFloat(RotationalForceID, rotationalForce * base.transform.localScale.x);
			velocitySpriteRenderer.SetPropertyBlock(velocityMPB);
			velocityXLastFrame = vector.x;
			velocityYLastFrame = vector.y;
			outwardsForceLastFrame = outwardsForce;
			noiseForceLastFrame = noiseForce;
			rotationalForceLastFrame = rotationalForce;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(velocityEmitter);
		Object.Destroy(temperatureEmitter);
		all.Remove(this);
	}

	public static FluidEmitter GetRandomEmitter()
	{
		if (all.Count > 0)
		{
			int index = Random.Range(0, all.Count);
			return all[index];
		}
		return null;
	}
}
