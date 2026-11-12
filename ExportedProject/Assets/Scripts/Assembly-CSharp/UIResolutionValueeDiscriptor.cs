using System.Collections.Generic;
using UnityEngine;

public class UIResolutionValueeDiscriptor : UIValueDescriptor
{
	private List<ResolutionSetting> availableResolutions = new List<ResolutionSetting>();

	private HashSet<string> uniqueResolutions = new HashSet<string>();

	public int resolutionIndex;

	private new void Start()
	{
		target = this;
		int num = 0;
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			ResolutionSetting resolutionSetting = new ResolutionSetting(resolution.width, resolution.height);
			if (!uniqueResolutions.Contains(resolutionSetting.GetDescription()))
			{
				availableResolutions.Add(resolutionSetting);
				uniqueResolutions.Add(resolutionSetting.GetDescription());
				if (resolution.width == Settings.resolution.width && resolution.height == Settings.resolution.height)
				{
					resolutionIndex = num;
				}
				num++;
			}
		}
		upperBound = availableResolutions.Count - 1;
		base.Start();
	}

	public override void Update()
	{
		base.Update();
		resolutionIndex = (resolutionIndex + availableResolutions.Count) % availableResolutions.Count;
		if (Settings.resolution != availableResolutions[resolutionIndex])
		{
			Settings.resolution = availableResolutions[resolutionIndex];
			UpdateDescriptor();
		}
	}

	public override void UpdateDescriptor()
	{
		UpdateTargetInfo();
		string description = availableResolutions[resolutionIndex].GetDescription();
		text.text = description;
	}
}
