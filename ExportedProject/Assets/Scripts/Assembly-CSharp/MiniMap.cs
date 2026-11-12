using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class MiniMap : MonoBehaviour
{
	public WFCGenerator wFCGenerator;

	private Segmentation segmentation;

	private Texture2D texture;

	private Color32[] pixelBuffer;

	private bool[,] revealed;

	public RawImage targetImage;

	public Image markImage;

	private static MiniMap instance;

	private readonly List<MiniMapMark> marks = new List<MiniMapMark>();

	public static Color COLOR_POINTOFINTEREST = new Color(1f, 0.5f, 0f, 0.5f);

	public static Color COLOR_ITEM = new Color(0.6f, 0.5f, 0.4f, 0.6f);

	private int width;

	private int height;

	private const int REVEAL_RADIUS = 8;

	private const int REVEAL_RADIUS_SQR = 64;

	public static MiniMap Get()
	{
		return instance;
	}

	private void Start()
	{
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		instance = this;
		markImage.gameObject.SetActive(value: false);
		segmentation = wFCGenerator.GenerateSegmentation();
		width = wFCGenerator.width * 3;
		height = wFCGenerator.height * 3;
		texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		texture.filterMode = FilterMode.Point;
		pixelBuffer = new Color32[width * height];
		revealed = new bool[width, height];
		targetImage.texture = texture;
		Mark(Player.Get().transform, new Color(1f, 0f, 0f, 0.5f));
		ClearTexture();
		texture.Apply();
	}

	private void ClearTexture()
	{
		for (int i = 0; i < pixelBuffer.Length; i++)
		{
			pixelBuffer[i] = Color.clear;
		}
		texture.SetPixels32(pixelBuffer);
	}

	private void UpdateReveal()
	{
		Vector2 miniMapPosition = GetMiniMapPosition(Player.Get().transform.position);
		int num = Mathf.RoundToInt(miniMapPosition.x + (float)width * 0.5f);
		int num2 = Mathf.RoundToInt(miniMapPosition.y + (float)height * 0.5f);
		int num3 = Mathf.Max(0, num - 8);
		int num4 = Mathf.Min(width - 1, num + 8);
		int num5 = Mathf.Max(0, num2 - 8);
		int num6 = Mathf.Min(height - 1, num2 + 8);
		bool flag = false;
		for (int i = num5; i <= num6; i++)
		{
			int num7 = i - num2;
			for (int j = num3; j <= num4; j++)
			{
				int num8 = j - num;
				if (num8 * num8 + num7 * num7 <= 64 && !revealed[j, i])
				{
					revealed[j, i] = true;
					if (!segmentation.occupied[j, i])
					{
						pixelBuffer[i * width + j] = Color.white;
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			texture.SetPixels32(pixelBuffer);
			texture.Apply(updateMipmaps: false);
		}
	}

	private Vector2 GetMiniMapPosition(Vector3 worldPos)
	{
		float num = (float)wFCGenerator.width * wFCGenerator.moduleScale * 2f;
		float num2 = (float)wFCGenerator.height * wFCGenerator.moduleScale * 2f;
		worldPos.x += wFCGenerator.moduleScale * 0.5f;
		worldPos.z += wFCGenerator.moduleScale * 0.5f;
		return new Vector2(worldPos.x / num, worldPos.z / num2) * 128f;
	}

	private void Update()
	{
		if (Time.frameCount % 8 == 0)
		{
			UpdateReveal();
		}
		for (int num = marks.Count - 1; num >= 0; num--)
		{
			MiniMapMark miniMapMark = marks[num];
			if (miniMapMark.target == null)
			{
				Object.Destroy(miniMapMark.image.gameObject);
				marks.RemoveAt(num);
			}
			else
			{
				miniMapMark.image.rectTransform.anchoredPosition = GetMiniMapPosition(miniMapMark.target.position);
			}
		}
	}

	private void OnDestroy()
	{
		if (this == instance)
		{
			instance = null;
		}
	}

	public void Mark(Transform t, Color color, float size = 4f)
	{
		Image image = Object.Instantiate(markImage, markImage.transform.parent);
		image.color = color;
		image.gameObject.SetActive(value: true);
		image.rectTransform.sizeDelta = new Vector2(size, size);
		marks.Add(new MiniMapMark
		{
			target = t,
			image = image
		});
	}

	public void Unmark(Transform t)
	{
		for (int num = marks.Count - 1; num >= 0; num--)
		{
			MiniMapMark miniMapMark = marks[num];
			if (miniMapMark.target == t)
			{
				Object.Destroy(miniMapMark.image.gameObject);
				marks.RemoveAt(num);
				break;
			}
		}
	}
}
