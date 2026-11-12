public class GridCell<G>
{
	public int x;

	public int y;

	public int z;

	public int objCount;

	public G[] obj;

	public GridCell(uint maxObjCount)
	{
		obj = new G[maxObjCount];
	}
}
