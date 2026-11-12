using System.Collections.Generic;
using UnityEngine;

public class AfterRunDialogs : MonoBehaviour
{
	public List<string> winDialogs = new List<string>();

	public List<string> lostDialogs = new List<string>();

	private void Start()
	{
		RunStats lastRun = PersistentDataComponent.GetDataInstance().GetLastRun();
		if (lastRun != null)
		{
			if (lastRun.state == RunState.Won)
			{
				CutSceneManager.Get().PushWait(1f);
				CutSceneManager.Get().PushDialog(StringLocalization.Get("demo_endingcall"), "demo");
			}
			else if (lastRun.state == RunState.Lost && lostDialogs.Count > 0)
			{
				CutSceneManager.Get().PushWait(1f);
				CutSceneManager.Get().PushDialog(StringLocalization.Parse(lostDialogs[Random.Range(0, lostDialogs.Count)]), "failure");
			}
		}
	}
}
