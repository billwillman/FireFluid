using System.Collections.Generic;
using TMPro;

public interface ITextEffect
{
	string Tag { get; }

	void ApplyEffect(TMP_TextInfo textInfo, List<int> charIndices, float time);
}
