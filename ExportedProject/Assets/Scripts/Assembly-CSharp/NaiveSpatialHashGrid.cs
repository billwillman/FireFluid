using System.Collections.Generic;
using UnityEngine;

public class NaiveSpatialHashGrid<T>
{
	public float cellSize = 1f;

	private Dictionary<Vector3Int, List<T>> data = new Dictionary<Vector3Int, List<T>>();

	public int GetGrid(float a)
	{
		return Mathf.FloorToInt(a / cellSize);
	}

	public List<T> Get(Vector3 p)
	{
		return Get(p.x, p.y, p.z);
	}

	public List<T> Get(float x, float y, float z)
	{
		int grid = GetGrid(x);
		int grid2 = GetGrid(y);
		int grid3 = GetGrid(z);
		Vector3Int key = new Vector3Int(grid, grid2, grid3);
		if (data.TryGetValue(key, out var value))
		{
			return value;
		}
		value = new List<T>();
		data[key] = value;
		return value;
	}

	public void Add(Vector3 p, T obj)
	{
		Add(p.x, p.y, p.z, obj);
	}

	public void Add(float x, float y, float z, T obj)
	{
		Get(x, y, z).Add(obj);
	}

	public void Clear()
	{
		data.Clear();
	}
}
