using UnityEngine;

public class SpatialHashGrid<T>
{
	private class HashBucket<J>
	{
		public int cellCount;

		public int allocatedCells;

		public GridCell<J>[] cells;

		public HashBucket(uint maxCellCollisions)
		{
			cells = new GridCell<J>[maxCellCollisions];
		}
	}

	private HashBucket<T>[] buckets;

	private uint slots;

	public float cellSize = 1f;

	public const int maxObjPerCell = 16;

	public const int maxNumOfCollisions = 4;

	private const uint p1 = 73856093u;

	private const uint p2 = 19349663u;

	private const uint p3 = 83492791u;

	public SpatialHashGrid(float cellSize = 1f, uint slots = 10007u, bool useNextPrime = true)
	{
		if (useNextPrime)
		{
			this.slots = GetNextPrimes(slots);
		}
		else
		{
			this.slots = slots;
		}
		this.cellSize = cellSize;
		buckets = new HashBucket<T>[this.slots];
	}

	public int GetGrid(float a)
	{
		return Mathf.FloorToInt(a / cellSize);
	}

	public float RoundToGrid(float a)
	{
		return (float)Mathf.FloorToInt(a / cellSize) * cellSize;
	}

	public Vector2 RoundToGrid(float x, float y, float z)
	{
		return new Vector3(RoundToGrid(x), RoundToGrid(y), RoundToGrid(z));
	}

	public GridCell<T> Get(Vector3 p)
	{
		return Get(p.x, p.y, p.z);
	}

	public GridCell<T> Get(float x, float y, float z)
	{
		int grid = GetGrid(x);
		int grid2 = GetGrid(y);
		int grid3 = GetGrid(z);
		uint spatialHash = GetSpatialHash(grid, grid2, grid3);
		HashBucket<T> hashBucket = buckets[spatialHash];
		if (hashBucket == null)
		{
			hashBucket = new HashBucket<T>(4u);
			buckets[spatialHash] = hashBucket;
		}
		for (int i = 0; i < hashBucket.cellCount; i++)
		{
			if (hashBucket.cells[i].x == grid && hashBucket.cells[i].y == grid2 && hashBucket.cells[i].z == grid3)
			{
				return hashBucket.cells[i];
			}
		}
		GridCell<T> gridCell = hashBucket.cells[hashBucket.cellCount];
		if (gridCell == null)
		{
			gridCell = new GridCell<T>(16u);
			hashBucket.cells[hashBucket.cellCount] = gridCell;
		}
		hashBucket.cellCount++;
		gridCell.x = grid;
		gridCell.y = grid2;
		gridCell.z = grid3;
		gridCell.objCount = 0;
		if (hashBucket.cellCount >= hashBucket.cells.Length)
		{
			GridCell<T>[] cells = hashBucket.cells;
			hashBucket.cells = new GridCell<T>[hashBucket.cells.Length * 2];
			for (int j = 0; j < cells.Length; j++)
			{
				hashBucket.cells[j] = cells[j];
			}
		}
		return gridCell;
	}

	public void Add(Vector3 p, T obj)
	{
		Add(p.x, p.y, p.z, obj);
	}

	public void Remove(Vector3 p, T obj)
	{
		Remove(p.x, p.y, p.z, obj);
	}

	public bool Contains(Vector3 p, T obj)
	{
		return Contains(p.x, p.y, p.z, obj);
	}

	public bool Occupied(Vector3 p)
	{
		return Occupied(p.x, p.y, p.z);
	}

	public void Clear(Vector3 p)
	{
		Clear(p.x, p.y, p.z);
	}

	public void Add(float x, float y, float z, T obj)
	{
		GridCell<T> gridCell = Get(x, y, z);
		if (gridCell.objCount >= gridCell.obj.Length - 1)
		{
			T[] obj2 = gridCell.obj;
			gridCell.obj = new T[gridCell.obj.Length * 2];
			for (int i = 0; i < obj2.Length; i++)
			{
				gridCell.obj[i] = obj2[i];
			}
		}
		gridCell.obj[gridCell.objCount] = obj;
		gridCell.objCount++;
	}

	public void Remove(float x, float y, float z, T obj)
	{
		GridCell<T> gridCell = Get(x, y, z);
		int i;
		for (i = 0; i < gridCell.objCount && !gridCell.obj[i].Equals(obj); i++)
		{
		}
		if (gridCell.obj[i].Equals(obj))
		{
			for (int j = i; j < gridCell.objCount - 1; j++)
			{
				gridCell.obj[j] = gridCell.obj[j + 1];
			}
			gridCell.objCount--;
		}
	}

	public bool Contains(float x, float y, float z, T obj)
	{
		GridCell<T> gridCell = Get(x, y, z);
		for (int i = 0; i < gridCell.objCount; i++)
		{
			if (gridCell.obj[i].Equals(obj))
			{
				return true;
			}
		}
		return false;
	}

	public T GetFirst(Vector3 p)
	{
		return GetFirst(p.x, p.y, p.z);
	}

	public T GetFirst(float x, float y, float z)
	{
		GridCell<T> gridCell = Get(x, y, z);
		if (gridCell.objCount > 0)
		{
			return gridCell.obj[0];
		}
		return default(T);
	}

	public bool Occupied(float x, float y, float z)
	{
		return Get(x, y, z).objCount > 0;
	}

	public void Clear()
	{
		for (int i = 0; i < slots; i++)
		{
			if (buckets[i] != null)
			{
				buckets[i].cellCount = 0;
			}
		}
	}

	public void Clear(float x, float y, float z)
	{
		Get(x, y, z).objCount = 0;
	}

	public uint GetSpatialHash(int x, int y, int z)
	{
		return (uint)((x * 73856093) ^ (y * 19349663) ^ (x * 83492791)) % slots;
	}

	private uint GetNextPrimes(uint n)
	{
		uint num;
		for (num = n; !IsPrime(num); num++)
		{
		}
		return num;
	}

	private bool IsPrime(uint n)
	{
		switch (n)
		{
		case 0u:
		case 1u:
			return false;
		case 2u:
		case 3u:
			return true;
		default:
		{
			for (int i = 2; (float)i <= Mathf.Sqrt(n); i++)
			{
				if (n % i == 0L)
				{
					return false;
				}
			}
			return true;
		}
		}
	}
}
