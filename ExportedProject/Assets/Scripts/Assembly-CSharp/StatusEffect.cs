using UnityEngine;

public class StatusEffect
{
	public Color tint;

	public CharacterAttributes target;

	public CharacterAttributes owner;

	public int stackCount = 1;

	public float duration = 3f;

	public bool isStackable;

	public float tickInterval = 1f;

	public float timeSinceLastTick;

	public RendererEffects rendererEffects;

	public StatusEffect Clone()
	{
		return (StatusEffect)MemberwiseClone();
	}

	public virtual string GetTypeIdentifier()
	{
		return "undefined";
	}

	public bool IsExpired()
	{
		return duration <= 0f;
	}

	public void Expire()
	{
		target = null;
		duration = -1f;
	}

	public virtual void OnTick(float deltaTime)
	{
	}

	public virtual void OnApply(CharacterAttributes target, CharacterAttributes owner)
	{
		this.target = target;
		this.owner = owner;
		rendererEffects = RendererEffects.Get(target.gameObject);
	}

	public virtual void OnStack(int stacks)
	{
	}

	public virtual void OnRemove()
	{
	}

	public virtual string GetShortDescription()
	{
		return "";
	}
}
