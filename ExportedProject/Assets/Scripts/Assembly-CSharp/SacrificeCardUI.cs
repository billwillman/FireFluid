using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SacrificeCardUI : MonoBehaviour
{
	private float blend = 1f;

	[Header("References")]
	public Image mainPanel;

	private List<Material> materials = new List<Material>();

	public GameObject selection;

	public TextMeshProUGUI headline;

	public TextMeshProUGUI text;

	public Image progressBar;

	private bool selected;

	public Sacrifice sacrifice;

	public void SetUp(Sacrifice sacrifice)
	{
		this.sacrifice = sacrifice;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer obj in componentsInChildren)
		{
			Material item = (obj.material = new Material(obj.material));
			materials.Add(item);
		}
		UpdateState();
		SetBlend(1f);
	}

	public void UpdateState()
	{
		if (!(sacrifice == null))
		{
			SacrificeEntry sacrificeEntry = PersistentDataComponent.GetDataInstance().GetSacrificeEntry(sacrifice.GetIdentifier());
			headline.text = sacrifice.GetName();
			text.text = Sacrifice.GetDescriptionAll(sacrifice.gameObject);
			progressBar.fillAmount = 1f * (float)sacrificeEntry.level / (float)sacrifice.GetMaxLevel();
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
	}

	public bool IsSelected()
	{
		return selected;
	}

	public void OnSelect()
	{
		selected = true;
		selection.SetActive(value: true);
	}

	public void OnDeselect()
	{
		selected = false;
		selection.SetActive(value: false);
	}
}
