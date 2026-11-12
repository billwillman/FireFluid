using System.Collections.Generic;
using UnityEngine;

public class BookPageUI : MonoBehaviour
{
	public const int UPGRADES_PER_PAGE = 6;

	public const int UPGRADES_PER_ROW = 2;

	public BookUI book;

	public List<BookPagePartUI> pageParts = new List<BookPagePartUI>();

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		for (int i = 0; i < 12; i++)
		{
			pageParts.Add(new BookPagePartUI());
		}
		int num = 0;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Material material = (renderer.material = renderer.material);
			pageParts[num].material = material;
			pageParts[num].rendererGameObject = renderer.gameObject;
			num++;
			if (num >= 12)
			{
				break;
			}
		}
	}

	public void ResetAnimationLeft()
	{
		animator.Play("LeftToRight", 0, 0f);
		animator.speed = 0f;
	}

	public void ResetAnimationRight()
	{
		animator.Play("RightToLeft", 0, 0f);
		animator.speed = 0f;
	}

	public void UpdateUpgrades(List<Upgrade> upgrades, int offset)
	{
		for (int i = 0; i < pageParts.Count; i++)
		{
			int num = i + offset;
			if (num >= 0 && num < upgrades.Count && upgrades[num].icon != null)
			{
				pageParts[i].material.mainTexture = upgrades[num].icon.texture;
				if (upgrades[num].GetEntry().level <= 0)
				{
					pageParts[i].material.SetTexture("_TopLayerTexture", book.lockedTexture);
				}
				else if (upgrades[num].GetEntry().active)
				{
					pageParts[i].material.SetTexture("_TopLayerTexture", book.emptyTexture);
				}
				else
				{
					pageParts[i].material.SetTexture("_TopLayerTexture", book.denyTexture);
				}
				if (upgrades[num].lockedInDemo)
				{
					pageParts[i].material.SetTexture("_TopLayerTexture", book.demoLockedTexture);
				}
				pageParts[i].upgrade = upgrades[num];
				pageParts[i].rendererGameObject.SetActive(value: true);
			}
			else
			{
				pageParts[i].upgrade = null;
				pageParts[i].rendererGameObject.SetActive(value: false);
			}
		}
	}

	public void SetAnimationTimeRightToLeft(float t)
	{
		animator.Play("RightToLeft", 0, t);
	}

	public void SetAnimationTimeLeftToRight(float t)
	{
		animator.Play("LeftToRight", 0, t);
	}

	public float GetHeartbeatIntensity(float t, float bpm = 60f)
	{
		float num = bpm / 60f;
		float num2 = t * num % 1f;
		float a = 0.7f * Mathf.Exp((0f - Mathf.Pow(num2 - 0.1f, 2f)) / (2f * Mathf.Pow(0.05f, 2f)));
		float b = 1f * Mathf.Exp((0f - Mathf.Pow(num2 - 0.3f, 2f)) / (2f * Mathf.Pow(0.05f, 2f)));
		return Mathf.Max(a, b);
	}

	public void UpdateColors(float dt)
	{
		foreach (BookPagePartUI pagePart in pageParts)
		{
			Color color = pagePart.color;
			color = (pagePart.color = ((pagePart == book.GetCurrentSelectedPagePart()) ? ((!pagePart.locked) ? Color.Lerp(color, book.selectedColor * (1f + book.GetCurrentInputHoldTime() * 4f), Mathf.Clamp01(dt * 16f)) : Color.Lerp(color, book.selectedGreyColor * (1f + book.GetCurrentInputHoldTime() * 4f), Mathf.Clamp01(dt * 16f))) : ((!pagePart.locked) ? Color.Lerp(color, book.unselectedColor, Mathf.Clamp01(dt * 8f)) : Color.Lerp(color, book.greyColor, Mathf.Clamp01(dt * 8f)))));
			if (pagePart.selectable)
			{
				color += new Color(1f, 1f, 1f, 1f) * GetHeartbeatIntensity(Time.unscaledTime, 40f) * 0.7f;
			}
			else
			{
				color *= 0.8f;
			}
			pagePart.material.SetColor("_Color", color);
		}
	}

	private void Update()
	{
		UpdateColors(Time.deltaTime);
	}
}
