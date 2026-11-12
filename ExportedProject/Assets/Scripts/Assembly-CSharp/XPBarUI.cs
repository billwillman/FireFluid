using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
	public CharacterAttributesPlayer player;

	public Image xpFill;

	public Text levelLabel;

	public GameObject unspendLevelUpElement;

	private Canvas canvas;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
		xpFill.fillAmount = player.experience / player.ExperienceNeededToLevelUp();
		levelLabel.text = player.level.ToString() ?? "";
		unspendLevelUpElement.SetActive(player.unspendLevelUps > 0);
		canvas.enabled = !player.health.IsExhausted();
	}
}
