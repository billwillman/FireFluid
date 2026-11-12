using System.Collections.Generic;
using UnityEngine;

public class SoundTrackScheduler : MonoBehaviour
{
	public List<SoundTrackScheduleEntry> schedule = new List<SoundTrackScheduleEntry>();

	private void Update()
	{
		int num = -1;
		SoundTrackScheduleEntry soundTrackScheduleEntry = null;
		foreach (SoundTrackScheduleEntry item in schedule)
		{
			num++;
			if (item.time < TimeManager.Get().time && !item.scheduled)
			{
				soundTrackScheduleEntry = item;
				break;
			}
		}
		if (soundTrackScheduleEntry != null)
		{
			soundTrackScheduleEntry.scheduled = true;
			if ((bool)SoundTrack.Get())
			{
				SoundTrackTransitionMode transitionMode = SoundTrack.Get().transitionMode;
				SoundTrack.Get().transitionMode = soundTrackScheduleEntry.transitionMode;
				SoundTrack.Get().SetSoundTrack(soundTrackScheduleEntry.audioClip, loop: true, soundTrackScheduleEntry.volume);
				SoundTrack.Get().transitionMode = transitionMode;
			}
		}
	}
}
