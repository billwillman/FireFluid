using UnityEngine;

public class SpawnOnPlayer : Ability
{
	public GameObject spawnPrefab;

	public override bool Trigger(AbilityCastParameters parameters)
	{
		Player player = Player.Get();
		if ((bool)player)
		{
			Object.Instantiate(spawnPrefab, player.transform).transform.localPosition = Vector3.zero;
		}
		return base.Trigger(parameters);
	}
}
