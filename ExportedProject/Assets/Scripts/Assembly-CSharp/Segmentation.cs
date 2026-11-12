using System.Collections.Generic;
using UnityEngine;

public class Segmentation
{
	public Vector2Int[,] slotPosition;

	public int[,] segmentId;

	public bool[,] occupied;

	private int width;

	private int height;

	private int numberOfSegments;

	public Segmentation(int width, int height)
	{
		this.width = width;
		this.height = height;
		slotPosition = new Vector2Int[width, height];
		segmentId = new int[width, height];
		occupied = new bool[width, height];
	}

	public int GetWidth()
	{
		return width;
	}

	public int GetHeight()
	{
		return height;
	}

	public int GetNumberOfSegments()
	{
		return numberOfSegments;
	}

	public void CalculateSegmentation()
	{
		numberOfSegments = 0;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (occupied[i, j])
				{
					segmentId[i, j] = -1;
				}
				else
				{
					segmentId[i, j] = -2;
				}
			}
		}
		int num = 0;
		for (int k = 0; k < width; k++)
		{
			for (int l = 0; l < height; l++)
			{
				if (segmentId[k, l] == -2)
				{
					FloodFill(k, l, num);
					numberOfSegments++;
					num++;
				}
			}
		}
	}

	private void FloodFill(int startX, int startY, int id)
	{
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		queue.Enqueue(new Vector2Int(startX, startY));
		segmentId[startX, startY] = id;
		while (queue.Count > 0)
		{
			Vector2Int vector2Int = queue.Dequeue();
			foreach (Vector2Int neighbor in GetNeighbors(vector2Int.x, vector2Int.y))
			{
				if (segmentId[neighbor.x, neighbor.y] == -2)
				{
					segmentId[neighbor.x, neighbor.y] = id;
					queue.Enqueue(neighbor);
				}
			}
		}
	}

	private IEnumerable<Vector2Int> GetNeighbors(int x, int y)
	{
		if (x > 0)
		{
			yield return new Vector2Int(x - 1, y);
		}
		if (x < width - 1)
		{
			yield return new Vector2Int(x + 1, y);
		}
		if (y > 0)
		{
			yield return new Vector2Int(x, y - 1);
		}
		if (y < height - 1)
		{
			yield return new Vector2Int(x, y + 1);
		}
		if (x < width - 1 && y < height - 1)
		{
			yield return new Vector2Int(x + 1, y + 1);
		}
		if (x > 0 && y < height - 1)
		{
			yield return new Vector2Int(x - 1, y + 1);
		}
		if (x < width - 1 && y > 0)
		{
			yield return new Vector2Int(x + 1, y - 1);
		}
		if (x > 0 && y > 0)
		{
			yield return new Vector2Int(x - 1, y - 1);
		}
	}
}
