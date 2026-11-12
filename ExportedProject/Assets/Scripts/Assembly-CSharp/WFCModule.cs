using System;
using UnityEngine;

[SerializeField]
public class WFCModule : MonoBehaviour
{
	public string id;

	public WFCMask up;

	public WFCMask down;

	public WFCMask left;

	public WFCMask right;

	public string middle = "0";

	public float weight = 1f;

	public bool rotatable = true;

	public bool SatisfiesConstraints(WFCModule neighbour, Direction neighbourDirection)
	{
		return neighbourDirection switch
		{
			Direction.UP => up.Fits(neighbour.down), 
			Direction.DOWN => down.Fits(neighbour.up), 
			Direction.LEFT => left.Fits(neighbour.right), 
			Direction.RIGHT => right.Fits(neighbour.left), 
			_ => false, 
		};
	}

	public static string Reverse(string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse(array);
		return new string(array);
	}

	public void RotateMasksClockwise()
	{
		string value = up.value;
		up.value = Reverse(left.value);
		left.value = down.value;
		down.value = Reverse(right.value);
		right.value = value;
	}

	public string GetMaskCode()
	{
		return up.value + left.value[1] + middle + right.value[1] + down.value;
	}
}
