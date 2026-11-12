using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlantRenderer : MonoBehaviour
{
	private class BranchVisual
	{
		public Transform transform;

		public Vector3 startPos;

		public Vector3 endPos;

		public int branchIndex;

		public float currentWidth;
	}

	private class FruitVisual
	{
		public GameObject gameOject;

		public IFruit fruit;

		public int branchIndex;
	}

	[Header("Rendering Settings")]
	public GameObject branchPrefab;

	public GameObject fruitPrefab;

	public float branchWidthFactor = 0.05f;

	public float growSpeed = 2f;

	private List<BranchVisual> activeBranches = new List<BranchVisual>();

	private List<FruitVisual> activeFruits = new List<FruitVisual>();

	private HashSet<int> growingBranches = new HashSet<int>();

	private HashSet<int> grownBranches = new HashSet<int>();

	private ProceduralPlant proceduralPlant;

	public float GetWidth(BranchNode branch)
	{
		return Mathf.Sqrt(branch.mass) * branchWidthFactor;
	}

	public void UpdateRenderData()
	{
		PlantData plantData = proceduralPlant.plantData;
		for (int i = 0; i < plantData.branches.Count; i++)
		{
			if (!growingBranches.Contains(i) && !grownBranches.Contains(i))
			{
				BranchNode branchNode = plantData.branches[i];
				if (branchNode.parentIndex != -1 && (grownBranches.Contains(branchNode.parentIndex) || branchNode.parentIndex == 0))
				{
					GameObject gameObject = Object.Instantiate(branchPrefab, base.transform);
					float width = GetWidth(branchNode);
					Vector3 position = plantData.branches[branchNode.parentIndex].position;
					BranchVisual branchVisual = new BranchVisual
					{
						transform = gameObject.transform,
						startPos = position,
						endPos = position,
						branchIndex = i,
						currentWidth = width
					};
					activeBranches.Add(branchVisual);
					growingBranches.Add(i);
					UpdateBranchTransform(branchVisual);
				}
			}
		}
		if (!(fruitPrefab != null))
		{
			return;
		}
		int j;
		for (j = 0; j < plantData.branches.Count; j++)
		{
			BranchNode branchNode2 = plantData.branches[j];
			if (branchNode2.fruitIndex >= 0 && !activeFruits.Exists((FruitVisual f) => f.branchIndex == j) && grownBranches.Contains(j))
			{
				GameObject gameObject2 = Object.Instantiate(fruitPrefab, Vector3.zero, base.transform.rotation, base.transform);
				gameObject2.transform.localPosition = branchNode2.position;
				gameObject2.transform.localScale = Vector3.zero;
				FruitVisual fruitVisual = new FruitVisual
				{
					gameOject = gameObject2,
					fruit = gameObject2.GetComponent<IFruit>(),
					branchIndex = j
				};
				activeFruits.Add(fruitVisual);
				if (fruitVisual.fruit != null)
				{
					fruitVisual.fruit.Initialize(proceduralPlant.plantData, branchNode2.fruitIndex);
				}
			}
		}
	}

	private void Start()
	{
		proceduralPlant = GetComponent<ProceduralPlant>();
	}

	public void AdvanceAnimation(float dt)
	{
		if (proceduralPlant.plantData.branches.Count < activeBranches.Count)
		{
			Clear();
		}
		UpdateRenderData();
		UpdateBranches(dt);
		UpdateFruits(dt);
		if (activeFruits.Count > 0)
		{
			FruitVisual fruitVisual = activeFruits[Random.Range(0, activeFruits.Count)];
			if (fruitVisual.fruit != null)
			{
				fruitVisual.fruit.CheckConsumed();
			}
		}
	}

	private void Update()
	{
		float num = Mathf.Max(1f, (float)(proceduralPlant.plantData.branches.Count - grownBranches.Count) * 0.5f);
		AdvanceAnimation(Time.deltaTime * growSpeed * num);
	}

	private void UpdateBranchTransform(BranchVisual bv)
	{
		Vector3 vector = bv.endPos - bv.startPos;
		float magnitude = vector.magnitude;
		if (!(magnitude < 0.0001f))
		{
			Vector3 localPosition = bv.startPos + vector * 0.5f;
			bv.transform.localPosition = localPosition;
			bv.transform.localRotation = Quaternion.FromToRotation(Vector3.up, vector.normalized);
			bv.transform.localScale = new Vector3(bv.currentWidth, magnitude * 0.5f + bv.currentWidth, bv.currentWidth);
		}
	}

	private void UpdateBranches(float dt)
	{
		foreach (BranchVisual activeBranch in activeBranches)
		{
			BranchNode branchNode = proceduralPlant.plantData.branches[activeBranch.branchIndex];
			Vector3 position = branchNode.position;
			float width = GetWidth(branchNode);
			activeBranch.endPos = Vector3.Lerp(activeBranch.endPos, position, dt);
			activeBranch.currentWidth = Mathf.Lerp(activeBranch.currentWidth, width, dt);
			UpdateBranchTransform(activeBranch);
			if (Vector3.Distance(activeBranch.endPos, position) < 0.01f)
			{
				activeBranch.endPos = position;
				grownBranches.Add(activeBranch.branchIndex);
			}
		}
	}

	private void UpdateFruits(float dt)
	{
		foreach (FruitVisual activeFruit in activeFruits)
		{
			float fruitGrowth = proceduralPlant.plantData.branches[activeFruit.branchIndex].fruitGrowth;
			if (activeFruit.fruit != null)
			{
				activeFruit.fruit?.UpdateGrowth(fruitGrowth);
			}
			else
			{
				activeFruit.gameOject.transform.localScale = Vector3.Lerp(activeFruit.gameOject.transform.localScale, Vector3.one * fruitGrowth, dt);
			}
		}
	}

	public void Clear()
	{
		foreach (BranchVisual activeBranch in activeBranches)
		{
			if (activeBranch?.transform != null)
			{
				Object.Destroy(activeBranch.transform.gameObject);
			}
		}
		activeBranches.Clear();
		grownBranches.Clear();
		foreach (FruitVisual activeFruit in activeFruits)
		{
			if (activeFruit?.gameOject != null)
			{
				Object.Destroy(activeFruit.gameOject);
			}
		}
		activeFruits.Clear();
	}
}
