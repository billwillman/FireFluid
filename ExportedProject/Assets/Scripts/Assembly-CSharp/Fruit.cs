using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class Fruit : MonoBehaviour, IFruit
{
	private int fruitIndex;

	public GameObject grownIndicator;

	public GameObject consumeEffectPrefab;

	public Transform model;

	public GameObject flower;

	public GameObject fruit;

	private float growth;

	private bool fullyGrown;

	private WorldButton worldButton;

	private Collider collider;

	private ToolTip toolTip;

	private bool mouseHover;

	private bool consumed;

	public List<AttributeModification> modificationOptions = new List<AttributeModification>();

	public string GetIdentifier()
	{
		return "Fruit_" + fruitIndex;
	}

	public void UpdateGrowth(float growth)
	{
		if (fruitIndex < 1)
		{
			this.growth = 1f;
		}
		else if (fruitIndex <= 2)
		{
			this.growth = Mathf.Clamp01(growth * 2f);
		}
		else
		{
			this.growth = growth;
		}
		if (this.growth > 0.99f)
		{
			fullyGrown = true;
		}
	}

	public void Initialize(PlantData plantData, int fruitIndex)
	{
		this.fruitIndex = fruitIndex;
	}

	public void TreeInitialized()
	{
		base.transform.localScale = Vector3.one * growth;
	}

	private void Awake()
	{
		AttributeModification attributeModification = new AttributeModification();
		attributeModification.attributePath = "CharacterAttributes.health";
		attributeModification.displayStyle = AttributeModificationDisplayStyle.Int;
		attributeModification.type = ValueModifierType.Add;
		attributeModification.value = 10f;
		modificationOptions.Add(attributeModification);
		AttributeModification attributeModification2 = new AttributeModification();
		attributeModification2.attributePath = "CharacterAttributes.damage";
		attributeModification2.displayStyle = AttributeModificationDisplayStyle.Int;
		attributeModification2.type = ValueModifierType.Add;
		attributeModification2.value = 1f;
		modificationOptions.Add(attributeModification2);
		AttributeModification attributeModification3 = new AttributeModification();
		attributeModification3.attributePath = "CharacterAttributes.luck";
		attributeModification3.displayStyle = AttributeModificationDisplayStyle.Int;
		attributeModification3.type = ValueModifierType.Add;
		attributeModification3.value = 1f;
		modificationOptions.Add(attributeModification3);
		foreach (AttributeModification modificationOption in modificationOptions)
		{
			modificationOption.EvaluateModificationValue();
		}
	}

	private void Start()
	{
		collider = GetComponent<Collider>();
		worldButton = GetComponent<WorldButton>();
		if ((bool)worldButton)
		{
			worldButton.OnEnter.AddListener(OnEnter);
			worldButton.OnExit.AddListener(OnExit);
			worldButton.OnPressed.AddListener(OnPressed);
		}
		toolTip = base.gameObject.GetComponentInChildren<ToolTip>();
		Quaternion rotation = fruit.transform.rotation;
		model.localRotation = Quaternion.Euler(Random.Range(-45f, 45f), Random.Range(0f, 360f), Random.Range(-45f, 45f)) * model.localRotation;
		model.localScale *= Random.Range(0.7f, 1.3f);
		fruit.transform.rotation = Quaternion.Slerp(fruit.transform.rotation, rotation, 0.3f);
		CheckConsumed();
	}

	public void CheckConsumed()
	{
		if (PersistentDataComponent.GetDataInstance().GetData(GetIdentifier()) != null)
		{
			Consume();
		}
	}

	private void Update()
	{
		if ((bool)toolTip)
		{
			if (fullyGrown)
			{
				toolTip.text = "<color=#aaffaa>" + StringLocalization.Get("tree_fruit_readyforharvest") + "</color>";
			}
			else
			{
				toolTip.text = StringLocalization.Get("tree_fruit_growth").Replace("{growthpercent}", Mathf.RoundToInt(growth * 100f) + "%");
			}
			toolTip.forceShowToolTip = mouseHover && UILayer.GetNumberOfOpenLayers() < 2;
		}
		if ((bool)grownIndicator)
		{
			grownIndicator.SetActive(fullyGrown && !consumed);
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one * growth, Time.unscaledDeltaTime);
	}

	public void OnEnter()
	{
		mouseHover = true;
		SoundEffectManager.Play("mysc.click");
	}

	public void OnExit()
	{
		mouseHover = false;
	}

	public void OnPressed()
	{
		if (UILayer.GetNumberOfOpenLayers() > 1)
		{
			return;
		}
		if (!fullyGrown)
		{
			SoundEffectManager.Play("mysc.locked");
			return;
		}
		SoundEffectManager.Play("tree.harvest");
		List<string> list = new List<string>();
		foreach (AttributeModification modificationOption in modificationOptions)
		{
			list.Add(modificationOption.GetDescription());
		}
		OptionsUIManager.Get().Open(StringLocalization.Get("tree_fruit_harvest"), list, delegate(int option)
		{
			OnHarvest(option);
		});
	}

	public void OnHarvest(int option)
	{
		Object.Instantiate(consumeEffectPrefab, fruit.transform.position, Quaternion.identity);
		PersistentDataComponent.GetDataInstance().SetData(GetIdentifier(), option);
		bool flag = false;
		foreach (AttributeModification treeModification in PersistentDataComponent.GetDataInstance().treeModifications)
		{
			if (treeModification.attributePath == modificationOptions[option].attributePath)
			{
				treeModification.value += modificationOptions[option].value;
				treeModification.EvaluateModificationValue();
				flag = true;
			}
		}
		if (!flag)
		{
			PersistentDataComponent.GetDataInstance().treeModifications.Add(new AttributeModification(modificationOptions[option]));
		}
		SoundEffectManager.Play("tree.applebite");
		Consume();
	}

	private void Consume()
	{
		fruit.SetActive(value: false);
		Object.Destroy(worldButton);
		Object.Destroy(toolTip);
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		consumed = true;
	}

	public bool IsFullyGrown()
	{
		return fullyGrown;
	}
}
