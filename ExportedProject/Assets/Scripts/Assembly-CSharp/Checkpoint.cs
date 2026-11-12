using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(101)]
public class Checkpoint : MonoBehaviour
{
	private static int highestPriority;

	public int priority;

	public UnityEvent onCheckPointLoad;

	public UnityEvent onFirstEnter;

	private void Awake()
	{
		if (IsSet())
		{
			highestPriority = Mathf.Max(highestPriority, priority);
		}
	}

	private void Start()
	{
		if (IsSet() && priority == highestPriority)
		{
			Player player = Player.Get();
			if ((bool)player)
			{
				player.SetToPosition(base.transform.position);
			}
			onCheckPointLoad?.Invoke();
		}
	}

	private void Update()
	{
	}

	public string GetCheckPointKey()
	{
		Vector3Int vector3Int = new Vector3Int(Mathf.RoundToInt(base.transform.position.x), Mathf.RoundToInt(base.transform.position.y), Mathf.RoundToInt(base.transform.position.z));
		return "checkpoint(" + vector3Int.x + "," + vector3Int.y + "," + vector3Int.z + "):" + priority;
	}

	public bool IsSet()
	{
		return PersistentDataComponent.GetDataInstance().GetData(GetCheckPointKey()) != null;
	}

	public void Set()
	{
		PersistentDataComponent.GetDataInstance().SetData(GetCheckPointKey(), true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponentInParent<Player>())
		{
			if (!IsSet())
			{
				onFirstEnter?.Invoke();
			}
			Set();
		}
	}
}
