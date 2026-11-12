using UnityEngine;
using UnityEngine.UI;

public class RunFailureUI : MonoBehaviour
{
	private RunState statePreviousFrame = RunState.Ongoing;

	private void Update()
	{
		if (statePreviousFrame == RunState.Ongoing && RunManager.Get().state == RunState.Lost)
		{
			base.transform.GetComponent<Animator>().Play("Death", 0, 0f);
			base.transform.GetChild(0).gameObject.SetActive(value: true);
			base.transform.GetChild(0).GetComponentInChildren<Button>().Select();
		}
		statePreviousFrame = RunManager.Get().state;
	}
}
