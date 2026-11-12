using System.Collections.Generic;

public class UILanguageValueDiscriptor : UIValueDescriptor
{
	private List<string> languageList = new List<string>();

	public int languageIndex;

	private new void Start()
	{
		target = this;
		int num = 0;
		StringLocalization.ValidateTable();
		foreach (string key in StringLocalization.localizationTable.Keys)
		{
			languageList.Add(key);
			if (key == Settings.language)
			{
				languageIndex = num;
			}
			num++;
		}
		upperBound = languageList.Count - 1;
		base.Start();
	}

	public override void Update()
	{
		base.Update();
		languageIndex = (languageIndex + languageList.Count) % languageList.Count;
		if (StringLocalization.currentLanguage != languageList[languageIndex])
		{
			Settings.language = languageList[languageIndex];
			StringLocalization.currentLanguage = Settings.language;
			UpdateDescriptor();
			TextFieldLocalization.UpdateAll();
			UIValueDescriptor.UpdateAll();
		}
	}

	public override void UpdateDescriptor()
	{
		UpdateTargetInfo();
		string text = StringLocalization.Get("language_name");
		base.text.text = text;
	}
}
