public class IncreaseProjectilesModification : Upgrade
{
	public int increase = 1;

	public override bool RequirementsMet(Player player)
	{
		ShootProjectile componentInChildren = player.GetComponentInChildren<ShootProjectile>();
		if (componentInChildren == null)
		{
			return false;
		}
		if (componentInChildren.numberOfProjectiles.GetCurrentValue() > 6)
		{
			return false;
		}
		return true;
	}

	public override void Apply(Player player)
	{
		base.Apply(player);
		player.GetComponentInChildren<ShootProjectile>().numberOfProjectiles.Push("Upgrade", increase, ValueModifierType.Add);
	}
}
