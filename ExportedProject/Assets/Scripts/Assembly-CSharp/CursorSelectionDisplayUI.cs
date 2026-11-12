using UnityEngine;

public class CursorSelectionDisplayUI : MonoBehaviour
{
	public GameObject selectionGameObject;

	public IUpgradeSelection upgradeSelectionUI;

	public UpgradeCardUI upgradeCardUIPrefab;

	private UpgradeCardUI upgradeCardUIInstance;

	private CinematicMachine cinematicMachine = new CinematicMachine();

	private void Start()
	{
		upgradeSelectionUI = selectionGameObject.GetComponent<IUpgradeSelection>();
		GetComponentInParent<Canvas>().worldCamera = Camera.main;
		GetComponentInParent<Canvas>().planeDistance = 2f;
		GetComponentInParent<Canvas>().sortingLayerName = "UI";
	}

	private void Update()
	{
		cinematicMachine.UpdateActions(Time.unscaledDeltaTime);
		if (upgradeSelectionUI.Show())
		{
			if (upgradeCardUIInstance == null || upgradeCardUIInstance.upgrade != upgradeSelectionUI.GetSelected())
			{
				if (upgradeCardUIInstance != null)
				{
					Object.Destroy(upgradeCardUIInstance.gameObject);
				}
				if (upgradeSelectionUI.GetSelected() != null)
				{
					upgradeCardUIInstance = Object.Instantiate(upgradeCardUIPrefab.gameObject, base.transform).GetComponent<UpgradeCardUI>();
					upgradeCardUIInstance.SetUp(upgradeSelectionUI.GetSelected());
					cinematicMachine.PushAction(CardUIAnimations.CreateRevealAnimationDiagonalFlip(upgradeCardUIInstance.transform, 0f));
				}
			}
		}
		else if (upgradeCardUIInstance != null)
		{
			Object.Destroy(upgradeCardUIInstance.gameObject);
			upgradeCardUIInstance = null;
		}
		if (upgradeCardUIInstance != null)
		{
			upgradeCardUIInstance.UpdateState();
		}
	}
}
