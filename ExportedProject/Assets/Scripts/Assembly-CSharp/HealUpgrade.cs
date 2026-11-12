public class HealUpgrade : Upgrade
{
	public override void Apply(Player player)
	{
		base.Apply(player);
		player.GetAttributes().health.Fill();
	}
}
