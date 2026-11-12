using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShakeEffect : ITextEffect
{
	public string Tag => "shake";

	public void ApplyEffect(TMP_TextInfo textInfo, List<int> charIndices, float time)
	{
		float num = 8f;
		float num2 = 1.5f;
		float num3 = 1f;
		foreach (int charIndex in charIndices)
		{
			if (charIndex >= textInfo.characterCount)
			{
				continue;
			}
			TMP_CharacterInfo tMP_CharacterInfo = textInfo.characterInfo[charIndex];
			if (tMP_CharacterInfo.isVisible)
			{
				int materialReferenceIndex = tMP_CharacterInfo.materialReferenceIndex;
				int vertexIndex = tMP_CharacterInfo.vertexIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				float x = (Mathf.PerlinNoise((float)charIndex * num3, time * num) - 0.5f) * 2f;
				float y = (Mathf.PerlinNoise((float)charIndex * num3 + 100f, time * num) - 0.5f) * 2f;
				Vector3 vector = new Vector3(x, y, 0f) * num2;
				for (int i = 0; i < 4; i++)
				{
					vertices[vertexIndex + i] += vector;
				}
			}
		}
	}
}
