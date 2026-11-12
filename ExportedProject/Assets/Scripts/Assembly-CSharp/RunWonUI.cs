using UnityEngine;
using UnityEngine.UI;

public class RunWonUI : MonoBehaviour
{
	public static bool show;

	private RunState statePreviousFrame = RunState.Ongoing;

	public void Show()
	{
		base.transform.GetComponent<Animator>().Play("Won", 0, 0f);
		base.transform.GetChild(0).gameObject.SetActive(value: true);
		base.transform.GetChild(0).GetComponentInChildren<Button>().Select();
	}

	private void Update()
	{
		if (show)
		{
			if (statePreviousFrame == RunState.Ongoing && RunManager.Get().state == RunState.Won)
			{
				Show();
			}
			statePreviousFrame = RunManager.Get().state;
		}
	}
}
