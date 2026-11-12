public class PickUpAllUpgrade : Upgrade
{
	public override void Apply(Player player)
	{
		base.Apply(player);
		foreach (Gem item in Gem.all)
		{
			item.Collect();
		}
	}
}
