public class StickyProjectile : Projectile
{
	public override void End()
	{
		StatusEffectManager.Apply(StatusEffectManager.sticky, owner, base.transform.position, impactRadius * base.transform.localScale.x, base.gameObject.layer);
		base.End();
	}
}
