public static class LayerHelper
{
	public static int LAYER_PLAYER = 6;

	public static int LAYER_PLAYERSUMMON = 19;

	public static int LAYER_ENEMY = 7;

	public static int LAYER_ENEMYPASSTHROUGH = 20;

	public static int LAYER_PLAYERATTACK = 9;

	public static int LAYER_ENEMYATTACK = 10;

	public static int LAYER_ATTACKGOESTHROUGH = 15;

	public static int LAYER_PLAYERATTACKPIERCING = 16;

	public static int LAYER_ENVIRONMENT = 13;

	public static int LAYER_ENVIRONMENTDESCTRUCTABLE = 17;

	public static int LAYERMASK_PLAYERATTACKS = (1 << LAYER_PLAYERATTACK) | (1 << LAYER_PLAYERATTACKPIERCING);

	public static int LAYERMASK_ENEMYATTACKS = 1 << LAYER_ENEMYATTACK;

	public static int LAYERMASK_ENEMY = (1 << LAYER_ENEMY) | (1 << LAYER_ENEMYPASSTHROUGH);

	public static int LAYERMASK_PLAYER = 1 << LAYER_PLAYER;

	public static int LAYERMASK_PLAYERSUMMON = 1 << LAYER_PLAYERSUMMON;

	public static int LAYERMASK_ENVIRONMENT = 1 << LAYER_ENVIRONMENT;

	public static int LAYERMASK_ENVIRONMENTDESTRUCTABLE = 1 << LAYER_ENVIRONMENTDESCTRUCTABLE;

	public static int LAYERMASK_NONPASSABLEENVIRONMENT = (1 << LAYER_ENVIRONMENT) | (1 << LAYER_ATTACKGOESTHROUGH);

	public static int LAYERMASK_PLAYEROBJECTS = LAYERMASK_PLAYER | LAYERMASK_PLAYERATTACKS | LAYERMASK_PLAYERSUMMON;

	public static int LAYERMASK_ENEMYOBJECTS = LAYERMASK_ENEMY | LAYERMASK_ENEMYATTACKS;

	public static int GetAttackLayer(int ownerLayer)
	{
		if (ownerLayer == LAYER_PLAYER)
		{
			return LAYER_PLAYERATTACK;
		}
		return LAYER_ENEMYATTACK;
	}

	public static int GetAttackTargetLayerMask(int layer)
	{
		if (((1 << layer) & LAYERMASK_PLAYEROBJECTS) > 0)
		{
			return LAYERMASK_ENEMY;
		}
		return LAYERMASK_PLAYER;
	}

	public static int GetOpponentObjectsLayerMask(int layer)
	{
		if (((1 << layer) & LAYERMASK_PLAYERATTACKS) > 0)
		{
			return LAYERMASK_ENEMYOBJECTS;
		}
		return LAYERMASK_PLAYEROBJECTS;
	}
}
