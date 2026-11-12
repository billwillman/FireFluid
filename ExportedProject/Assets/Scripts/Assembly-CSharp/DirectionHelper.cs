using UnityEngine;

public static class DirectionHelper
{
	public static readonly (Vector2Int vector, Direction direction, Direction oppositeDirection)[] Directions = new(Vector2Int, Direction, Direction)[4]
	{
		(Vector2Int.up, Direction.UP, Direction.DOWN),
		(Vector2Int.down, Direction.DOWN, Direction.UP),
		(Vector2Int.left, Direction.LEFT, Direction.RIGHT),
		(Vector2Int.right, Direction.RIGHT, Direction.LEFT)
	};
}
