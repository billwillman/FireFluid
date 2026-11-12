using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
	public float cooldown;

	private static List<Notification> notifications = new List<Notification>();

	public GameObject notificationDisplay;

	public TextMeshProUGUI text;

	public Vector3 outOfScreenOffset = Vector3.down * 64f;

	public AudioSource notificationSound;

	public float animationTime = 0.5f;

	public void Start()
	{
		List<Notification> list = new List<Notification>();
		foreach (Notification notification in notifications)
		{
			if (notification.displayTimer > 0.5f)
			{
				list.Add(notification);
			}
		}
		foreach (Notification item in list)
		{
			notifications.Remove(item);
		}
		foreach (Notification notification2 in notifications)
		{
			notification2.displayTimer = 0f;
		}
	}

	public static void PushNotification(string message, float displayTime = 3f)
	{
		Notification notification = new Notification();
		notification.displayTimer = 0f;
		notification.displayTime = displayTime;
		notification.message = message;
		notifications.Add(notification);
	}

	private void Update()
	{
		if (!SceneManager.Get().IsBusy())
		{
			cooldown -= Time.unscaledDeltaTime;
		}
		RectTransform component = notificationDisplay.GetComponent<RectTransform>();
		if (notifications.Count > 0 && cooldown < 0f && (!SceneManager.Get().IsBusy() || notifications[0].displayTimer > 0.1f))
		{
			notificationDisplay.SetActive(value: true);
			Notification notification = notifications[0];
			text.text = notification.message;
			float num = animationTime;
			float num2 = animationTime;
			if (!notification.soundPlayed)
			{
				if (notificationSound != null)
				{
					notificationSound.Play();
				}
				notification.soundPlayed = true;
			}
			if (notification.displayTimer < num)
			{
				component.anchoredPosition = outOfScreenOffset * (num - notification.displayTimer) / num;
			}
			else if (notification.displayTimer < notification.displayTime - num2)
			{
				component.anchoredPosition = Vector3.zero;
			}
			else if (notification.displayTimer < notification.displayTime)
			{
				component.anchoredPosition = outOfScreenOffset * (1f - (notification.displayTime - notification.displayTimer) / num2);
			}
			else
			{
				notifications.Remove(notification);
			}
			if (notifications.Count > 1)
			{
				notification.displayTime += Time.unscaledDeltaTime * 2f;
			}
			else
			{
				notification.displayTimer += Time.unscaledDeltaTime;
			}
		}
		else
		{
			notificationDisplay.SetActive(value: false);
		}
	}
}
