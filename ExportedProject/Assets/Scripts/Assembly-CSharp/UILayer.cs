using System.Collections.Generic;

public class UILayer
{
	public static SortedList<int, object> layers = new SortedList<int, object>();

	private static int layerGen = int.MaxValue;

	public static bool AnyLayerOpen()
	{
		return layers.Count > 0;
	}

	public static void Open(object caller)
	{
		foreach (int key in layers.Keys)
		{
			if (layers[key] == caller)
			{
				return;
			}
		}
		layers.Add(--layerGen, caller);
	}

	public static void Close(object caller)
	{
		foreach (int key in layers.Keys)
		{
			if (layers[key] == caller)
			{
				layers.Remove(key);
				break;
			}
		}
		if (layers.Count == 0)
		{
			layerGen = int.MaxValue;
		}
	}

	public static void CloseAll()
	{
		layers.Clear();
		layerGen = int.MaxValue;
	}

	public static bool IsActive(object caller)
	{
		return layers.IndexOfValue(caller) == 0;
	}

	public static int GetNumberOfOpenLayers()
	{
		return layers.Count;
	}
}
