using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BranchNode
{
	public Vector3 position;

	public Vector3 direction;

	public int parentIndex;

	public List<int> children = new List<int>();

	public int depth;

	public float mass;

	public int fruitIndex = -1;

	public float fruitGrowth;
}
