using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextEffectAnimator : MonoBehaviour
{
	private TMP_Text textComponent;

	private TMP_TextInfo textInfo;

	private string originalText;

	private Dictionary<string, ITextEffect> effects = new Dictionary<string, ITextEffect>();

	private Dictionary<string, List<int>> effectCharIndices = new Dictionary<string, List<int>>();

	private Vector3[][] originalVertices;

	private bool hasActiveEffects;

	private void Start()
	{
		ApplyEffectTags();
	}

	public void ApplyEffectTags()
	{
		textComponent = GetComponent<TMP_Text>();
		originalText = textComponent.text;
		RegisterEffects();
		ParseTags();
		if (hasActiveEffects)
		{
			StartCoroutine(AnimateText());
		}
	}

	private void RegisterEffects()
	{
		AddEffect(new ShakeEffect());
	}

	private void AddEffect(ITextEffect effect)
	{
		effects[effect.Tag] = effect;
		effectCharIndices[effect.Tag] = new List<int>();
	}

	private void ParseTags()
	{
		hasActiveEffects = false;
		string text = originalText;
		int num = 0;
		foreach (KeyValuePair<string, ITextEffect> effect in effects)
		{
			string key = effect.Key;
			List<int> list = effectCharIndices[key];
			list.Clear();
			string text2 = "<" + key + ">";
			string text3 = "</" + key + ">";
			while (true)
			{
				int num2 = text.IndexOf(text2);
				int num3 = text.IndexOf(text3);
				if (num2 == -1 || num3 == -1 || num3 < num2)
				{
					break;
				}
				hasActiveEffects = true;
				int num4 = num2 + text2.Length;
				for (int i = num2 - num; i < num3 - num - text2.Length; i++)
				{
					list.Add(i);
				}
				text = text.Remove(num3, text3.Length);
				text = text.Remove(num2, text2.Length);
				num += text2.Length + text3.Length;
			}
		}
		textComponent.text = text;
	}

	private IEnumerator AnimateText()
	{
		textComponent.ForceMeshUpdate();
		textInfo = textComponent.textInfo;
		originalVertices = new Vector3[textInfo.meshInfo.Length][];
		for (int i = 0; i < textInfo.meshInfo.Length; i++)
		{
			TMP_MeshInfo tMP_MeshInfo = textInfo.meshInfo[i];
			originalVertices[i] = new Vector3[tMP_MeshInfo.vertices.Length];
			Array.Copy(tMP_MeshInfo.vertices, originalVertices[i], tMP_MeshInfo.vertices.Length);
		}
		yield return new WaitForEndOfFrame();
		while (true)
		{
			yield return new WaitForEndOfFrame();
			float time = Time.time;
			textComponent.ForceMeshUpdate();
			textInfo = textComponent.textInfo;
			for (int j = 0; j < textInfo.meshInfo.Length; j++)
			{
				Vector3[] array = originalVertices[j];
				Vector3[] vertices = textInfo.meshInfo[j].vertices;
				for (int k = 0; k < vertices.Length; k++)
				{
					vertices[k] = array[k];
				}
			}
			foreach (KeyValuePair<string, ITextEffect> effect in effects)
			{
				string key = effect.Key;
				effect.Value.ApplyEffect(charIndices: effectCharIndices[key], textInfo: textInfo, time: time);
			}
			for (int l = 0; l < textInfo.meshInfo.Length; l++)
			{
				TMP_MeshInfo tMP_MeshInfo2 = textInfo.meshInfo[l];
				tMP_MeshInfo2.mesh.vertices = tMP_MeshInfo2.vertices;
				textComponent.UpdateGeometry(tMP_MeshInfo2.mesh, l);
			}
			yield return null;
		}
	}
}
