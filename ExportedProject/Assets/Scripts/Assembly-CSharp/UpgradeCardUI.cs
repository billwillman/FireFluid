using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
	private float blend = 1f;

	[Header("References")]
	public Sprite cardImage;

	private List<Material> materials = new List<Material>();

	public Sprite cardImageSelected;

	public Image selectionIndicator;

	public TextMeshProUGUI headline;

	public TextMeshProUGUI heatLabel;

	public Image abilityImage;

	public TextMeshProUGUI text;

	public Image uniqueImage;

	public TextMeshProUGUI uniqueLabel;

	public Transform affinitiesContainer;

	public List<Image> affinityImages = new List<Image>();

	public Image graphic_notActive;

	public Image graphic_locked;

	public Image graphic_heat;

	private InteractableShowHint hint;

	public TextMeshProUGUI hintText;

	private Image panel;

	private Canvas canvas;

	private bool selected;

	public Upgrade upgrade;

	private bool locked;

	private static readonly float[] heat_values = new float[5] { 0f, 0.3f, 0.38f, 0.45f, 0.55f };

	public void SetUp(Upgrade upgrade)
	{
		this.upgrade = upgrade;
		panel = GetComponent<Image>();
		hint = GetComponentInChildren<InteractableShowHint>();
		canvas = GetComponentInParent<Canvas>();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer obj in componentsInChildren)
		{
			Material item = (obj.material = new Material(obj.material));
			materials.Add(item);
		}
		Material material2 = new Material(panel.material);
		panel.material = material2;
		materials.Add(material2);
		Material material3 = new Material(graphic_heat.material);
		graphic_heat.material = material3;
		materials.Add(material3);
		Material material4 = new Material(uniqueImage.material);
		uniqueImage.material = material4;
		materials.Add(material4);
		UpdateAffinities();
		UpdateState();
		SetBlend(1f);
	}

	public void UpdateState()
	{
		if (!(upgrade == null))
		{
			headline.text = upgrade.GetName();
			int level = upgrade.GetLevel();
			int num = upgrade.GetLevel() - 1;
			if (level <= 0)
			{
				heatLabel.gameObject.SetActive(value: true);
				heatLabel.text = "<color=#111111><b>" + StringLocalization.Get("upgrade_locked") + "</b></color>";
			}
			else if (level == 1)
			{
				heatLabel.gameObject.SetActive(value: false);
			}
			else
			{
				heatLabel.gameObject.SetActive(value: true);
				heatLabel.text = StringLocalization.Get("upgrade_heat") + " <b>" + num + "</b>";
			}
			if (upgrade.icon != null)
			{
				abilityImage.sprite = upgrade.icon;
			}
			if (level <= 0)
			{
				text.text = "<color=#111111><size=200%><b>? ? ?</b></size></color>";
			}
			else
			{
				text.text = Upgrade.GetDescriptionAll(upgrade.gameObject);
			}
			if (upgrade.unique)
			{
				uniqueLabel.gameObject.SetActive(value: true);
				uniqueImage.enabled = true;
				uniqueLabel.text = StringLocalization.Get("upgrade_unique");
			}
			else if (!upgrade.canBeFoundDuringRun)
			{
				uniqueLabel.gameObject.SetActive(value: true);
				uniqueImage.enabled = true;
				uniqueLabel.text = StringLocalization.Get("upgrade_maskaspect");
			}
			else
			{
				uniqueLabel.gameObject.SetActive(value: false);
				uniqueImage.enabled = false;
			}
			UpgradeEntry upgradeEntry = PersistentDataComponent.GetDataInstance().GetUpgradeEntry(upgrade.GetIdentifier());
			graphic_notActive.enabled = !upgradeEntry.active;
			graphic_locked.enabled = IsLocked();
			graphic_heat.material.SetFloat("_Heat", heat_values[Mathf.Clamp(upgradeEntry.level - 1, 0, heat_values.Length - 1)]);
		}
	}

	public void UpdateAffinities()
	{
		foreach (Transform item in affinitiesContainer)
		{
			Object.Destroy(item.gameObject);
		}
		affinityImages.Clear();
		foreach (CharacterMask maskAffinity in upgrade.maskAffinities)
		{
			Image image = new GameObject("MaskAffinity_" + maskAffinity.GetIdentifier()).AddComponent<Image>();
			image.transform.SetParent(affinitiesContainer);
			image.transform.localPosition = new Vector3(0f, 0f, -1f);
			image.transform.localRotation = Quaternion.identity;
			image.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			image.sprite = maskAffinity.icon;
			image.SetNativeSize();
			affinityImages.Add(image);
		}
	}

	public float GetBlend()
	{
		return blend;
	}

	public void SetBlend(float blend)
	{
		this.blend = blend;
		foreach (Material material in materials)
		{
			if (material.HasFloat("_Blend"))
			{
				material.SetFloat("_Blend", blend);
			}
		}
		Color color = text.color;
		color.a = blend;
		text.color = color;
		color = headline.color;
		color.a = blend;
		headline.color = color;
		color = uniqueLabel.color;
		color.a = blend;
		uniqueLabel.color = color;
		color = uniqueImage.color;
		color.a = blend;
		uniqueImage.color = color;
		color = abilityImage.color;
		color.a = blend;
		abilityImage.color = color;
		color = selectionIndicator.color;
		color.a = blend;
		selectionIndicator.color = color;
		foreach (Image affinityImage in affinityImages)
		{
			color = affinityImage.color;
			color.a = blend;
			affinityImage.color = color;
		}
	}

	public bool IsSelected()
	{
		return selected;
	}

	public void OnSelect()
	{
		selected = true;
		panel.sprite = cardImageSelected;
		if (!selectionIndicator.enabled)
		{
			Color color = selectionIndicator.color;
			color.a = 1f;
			selectionIndicator.color = color;
			SoundEffectManager.Play("mysc.click");
		}
		selectionIndicator.enabled = true;
		string hintAll = Upgrade.GetHintAll(upgrade.gameObject);
		if (hintAll != "")
		{
			hintText.text = hintAll;
			hint.OnStartShowHint(Player.Get().GetComponent<InteractionController>());
		}
	}

	public void OnDeselect()
	{
		selected = false;
		panel.sprite = cardImage;
		selectionIndicator.enabled = false;
		hint.OnEndShowHint();
	}

	public void SetLocked(bool locked)
	{
		this.locked = locked;
		graphic_locked.enabled = locked;
	}

	public bool IsLocked()
	{
		return locked;
	}

	private void Update()
	{
		Color color = selectionIndicator.color;
		color.a = Mathf.Lerp(color.a, 0.25f, Time.unscaledDeltaTime * 8f);
		selectionIndicator.color = color;
		if (blend < 0.9f && hint.IsSown())
		{
			hint.OnEndShowHint();
		}
	}
}
