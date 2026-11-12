using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InteractionController : MonoBehaviour
{
	public float interactionRange = 2f;

	private IInteractable currentlyInteracting;

	private Collider[] hits = new Collider[32];

	private GameObject nearestInteractableGameObject;

	private IInteractable nearestInteractable;

	private List<IInteractable> interactablesInRange = new List<IInteractable>();

	private CharacterController characterController;

	private InteractableHighlighter interactableHighlighter;

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
		interactableHighlighter = GetComponent<InteractableHighlighter>();
	}

	private void UpdateInteractablesInRange()
	{
		interactablesInRange.Clear();
		nearestInteractable = null;
		nearestInteractableGameObject = null;
		int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, interactionRange, hits, layerMask);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IInteractable componentInParent = hits[i].transform.GetComponentInParent<IInteractable>();
			int layerMask2 = 1 << LayerMask.NameToLayer("Environment");
			bool flag = !Physics.Raycast(base.transform.position, hits[i].transform.position - base.transform.position, (hits[i].transform.position - base.transform.position).magnitude, layerMask2);
			if (componentInParent != null && flag && componentInParent.CanInteract())
			{
				interactablesInRange.Add(componentInParent);
				float sqrMagnitude = (hits[i].transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					nearestInteractable = componentInParent;
					nearestInteractableGameObject = hits[i].gameObject;
				}
			}
		}
	}

	public IInteractable GetCurrentlyInteracting()
	{
		return currentlyInteracting;
	}

	public IInteractable GetNearestInteractable()
	{
		return nearestInteractable;
	}

	private void Update()
	{
		IInteractable interactable = nearestInteractable;
		GameObject gameObject = nearestInteractableGameObject;
		if (!characterController.enableControlls.GetCurrentValue() || UILayer.AnyLayerOpen() || DialogManager.Get().IsInDialog())
		{
			if (GetCurrentlyInteracting() != null)
			{
				currentlyInteracting.OnEndInteract();
				currentlyInteracting = null;
			}
		}
		else
		{
			UpdateInteractablesInRange();
			if (interactableHighlighter != null && nearestInteractable is MonoBehaviour && currentlyInteracting == null)
			{
				interactableHighlighter.DrawInteractableHighlight((MonoBehaviour)nearestInteractable);
			}
			if (InputManager.Action == ButtonState.Down)
			{
				if (currentlyInteracting == null)
				{
					if (nearestInteractable != null)
					{
						currentlyInteracting = nearestInteractable;
						currentlyInteracting.OnStartInteract(this);
					}
				}
				else
				{
					currentlyInteracting.OnEndInteract();
					currentlyInteracting = null;
				}
			}
			if (currentlyInteracting != null && currentlyInteracting.OnInteract())
			{
				currentlyInteracting.OnEndInteract();
				currentlyInteracting = null;
			}
		}
		if ((CutSceneManager.Get() != null && CutSceneManager.Get().IsBusy()) || ((bool)DialogManager.Get() && DialogManager.Get().IsInDialog()) || UILayer.AnyLayerOpen())
		{
			nearestInteractable = null;
		}
		if (currentlyInteracting != null)
		{
			IInteractableHint[] componentsInChildren = nearestInteractableGameObject.GetComponentsInChildren<IInteractableHint>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEndShowHint();
			}
		}
		else
		{
			if (nearestInteractable == interactable)
			{
				return;
			}
			if (nearestInteractableGameObject != null && nearestInteractable != null)
			{
				IInteractableHint[] componentsInChildren = nearestInteractableGameObject.GetComponentsInChildren<IInteractableHint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnStartShowHint(this);
				}
			}
			if (gameObject != null && interactable != null)
			{
				IInteractableHint[] componentsInChildren = gameObject.GetComponentsInChildren<IInteractableHint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnEndShowHint();
				}
			}
		}
	}
}
