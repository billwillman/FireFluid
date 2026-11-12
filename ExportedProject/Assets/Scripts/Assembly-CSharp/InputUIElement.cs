using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputUIElement : MonoBehaviour
{
	public Image inputGraphic;

	public TextMeshProUGUI actionText;

	public List<Sprite> mouseAndKeySprites = new List<Sprite>();

	public List<Sprite> gamePadSprites = new List<Sprite>();

	public List<Sprite> nintendoSwitchSprites = new List<Sprite>();

	private float showDuration = 5f;

	private float timeShownSinceUse;

	protected bool inputUsed;

	protected CharacterController characterController;

	public float animationSpeed = 2f;

	public virtual void Start()
	{
		characterController = Player.Get().GetComponent<CharacterController>();
	}

	public virtual void Update()
	{
		int num = (int)(Time.unscaledTime * animationSpeed);
		List<Sprite> list = mouseAndKeySprites;
		if (InputManager.inputMode == InputMode.Gamepad)
		{
			list = gamePadSprites;
		}
		if (list.Count != 0)
		{
			num %= list.Count;
			inputGraphic.sprite = list[num];
			inputGraphic.SetNativeSize();
			if (inputUsed)
			{
				timeShownSinceUse += Time.unscaledDeltaTime;
			}
		}
	}

	public virtual bool IsShowing()
	{
		if (inputUsed && timeShownSinceUse > showDuration)
		{
			return false;
		}
		if ((bool)characterController && !characterController.enableControlls.GetCurrentValue())
		{
			return false;
		}
		if ((bool)DialogManager.Get() && DialogManager.Get().IsInDialog())
		{
			return false;
		}
		if (UILayer.AnyLayerOpen())
		{
			return false;
		}
		return true;
	}

	public virtual float GetProgress()
	{
		return timeShownSinceUse / showDuration;
	}
}
