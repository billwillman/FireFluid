using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundEffectManager : MonoBehaviour
{
	private struct SoundEffectTracker
	{
		public float lastPlayTime;

		public float cooldown;

		public int currentInstances;
	}

	[Header("Sound Effect Definitions")]
	[SerializeField]
	private List<SoundEffect> soundEffects;

	private Dictionary<string, SoundEffect> soundDict;

	private Dictionary<string, Dictionary<Transform, SoundEffectTracker>> entityCooldowns;

	private Dictionary<string, int> activeSoundInstances;

	private Dictionary<string, List<AudioSource>> currentlyPlayingAudioSources;

	private ObjectPool<GameObject> audioPool;

	[Header("AudioSource Settings")]
	[SerializeField]
	private GameObject audioPrefab;

	public const int MAX_INSTANCES_PER_SOUND = 4;

	public static SoundEffectManager instance { get; private set; }

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		soundDict = new Dictionary<string, SoundEffect>();
		entityCooldowns = new Dictionary<string, Dictionary<Transform, SoundEffectTracker>>();
		activeSoundInstances = new Dictionary<string, int>();
		currentlyPlayingAudioSources = new Dictionary<string, List<AudioSource>>();
		foreach (SoundEffect soundEffect in soundEffects)
		{
			if (!soundDict.ContainsKey(soundEffect.id))
			{
				soundDict.Add(soundEffect.id, soundEffect);
			}
			else
			{
				Debug.LogWarning("Duplicate sound effect ID: " + soundEffect.id);
			}
		}
		audioPool = new ObjectPool<GameObject>(() => Object.Instantiate(audioPrefab), delegate(GameObject go)
		{
			if (go == null)
			{
				go = Object.Instantiate(audioPrefab);
			}
			go.SetActive(value: true);
		}, delegate(GameObject go)
		{
			if (go != null)
			{
				go.SetActive(value: false);
			}
		}, Object.Destroy, collectionCheck: false, 64);
	}

	public static void Play(string soundId)
	{
		Play(soundId, Vector3.zero, null, spatial: false);
	}

	public static void Play(string soundId, float volumeModifier)
	{
		Play(soundId, Vector3.zero, null, spatial: false, volumeModifier);
	}

	private static string GetKey(string soundId, Transform entity)
	{
		if (entity == null)
		{
			return soundId;
		}
		return soundId + entity.GetInstanceID();
	}

	public static void Play(string soundId, Vector3 position, Transform entity = null, bool spatial = true, float volumeModifier = 1f)
	{
		if (soundId == "" || !TryGetSFX(soundId, out var sfx) || !CanPlay(soundId, sfx, entity))
		{
			return;
		}
		if (sfx.clips == null || sfx.clips.Count == 0)
		{
			Debug.LogWarning("No audio clips assigned to sound ID: " + soundId);
		}
		else if (!(Random.value >= sfx.chanceToPlay))
		{
			if (!instance.activeSoundInstances.ContainsKey(soundId))
			{
				instance.activeSoundInstances[soundId] = 0;
			}
			else if (instance.activeSoundInstances[soundId] > 4)
			{
				return;
			}
			Dictionary<string, int> dictionary = instance.activeSoundInstances;
			int value = dictionary[soundId] + 1;
			dictionary[soundId] = value;
			GameObject gameObject = instance.audioPool.Get();
			if (gameObject == null)
			{
				gameObject = Object.Instantiate(instance.audioPrefab);
			}
			gameObject.transform.position = position;
			AudioSource component = gameObject.GetComponent<AudioSource>();
			AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Count)];
			component.clip = clip;
			component.volume = sfx.volume * Settings.fxVolume * volumeModifier;
			component.pitch = Random.Range(sfx.pitchRange.x, sfx.pitchRange.y);
			if (!spatial)
			{
				component.spatialBlend = 0f;
			}
			component.Play();
			string key = GetKey(soundId, entity);
			if (!instance.currentlyPlayingAudioSources.TryGetValue(key, out var value2))
			{
				value2 = new List<AudioSource>();
				instance.currentlyPlayingAudioSources[key] = value2;
			}
			value2.Add(component);
			instance.StartCoroutine(instance.ReturnToPoolAfter(component.clip.length / component.pitch, component, soundId, key));
		}
	}

	public static bool IsPlaying(string soundId, Transform entity)
	{
		string key = GetKey(soundId, entity);
		if (!instance.currentlyPlayingAudioSources.TryGetValue(key, out var value))
		{
			return false;
		}
		return value.Count > 0;
	}

	public static void Choke(string soundId, Transform entity)
	{
		string key = GetKey(soundId, entity);
		if (!instance.currentlyPlayingAudioSources.TryGetValue(key, out var value))
		{
			return;
		}
		foreach (AudioSource item in value)
		{
			item.Stop();
		}
		value.Clear();
	}

	private static bool TryGetSFX(string soundId, out SoundEffect sfx)
	{
		if (instance == null)
		{
			Debug.LogWarning("SoundEffectManager instance not found.");
			sfx = null;
			return false;
		}
		if (!instance.soundDict.TryGetValue(soundId, out sfx))
		{
			Debug.LogWarning("SoundEffect ID not found: " + soundId);
			return false;
		}
		return true;
	}

	private static bool CanPlay(string soundId, SoundEffect sfx, Transform entity)
	{
		float unscaledTime = Time.unscaledTime;
		if (!instance.entityCooldowns.TryGetValue(soundId, out var value))
		{
			value = new Dictionary<Transform, SoundEffectTracker>();
			instance.entityCooldowns[soundId] = value;
		}
		Transform key = entity ?? instance.transform;
		if (value.TryGetValue(key, out var value2))
		{
			if (unscaledTime - value2.lastPlayTime < value2.cooldown)
			{
				return false;
			}
			if (value2.currentInstances > 4)
			{
				return false;
			}
		}
		value2.lastPlayTime = unscaledTime;
		value2.cooldown = Random.Range(sfx.cooldownRange.x, sfx.cooldownRange.y);
		value[key] = value2;
		return true;
	}

	private IEnumerator ReturnToPoolAfter(float delay, AudioSource audioSource, string soundId, string audioSourceId)
	{
		yield return new WaitForSecondsRealtime(delay);
		if (instance.activeSoundInstances.TryGetValue(soundId, out var value))
		{
			instance.activeSoundInstances[soundId] = value - 1;
		}
		if (instance.currentlyPlayingAudioSources.TryGetValue(audioSourceId, out var value2))
		{
			value2.Remove(audioSource);
		}
		if (audioSource != null)
		{
			audioPool.Release(audioSource.gameObject);
		}
	}
}
