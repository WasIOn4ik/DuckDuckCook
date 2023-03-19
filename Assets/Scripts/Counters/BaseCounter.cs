using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
	public static event EventHandler OnAnyObjectPlaced;
	public static void ResetStaticData()
	{
		OnAnyObjectPlaced = null;
	}

	[SerializeField] private Transform counterTopPoint;

	private KitchenObject kitchenObject;

	public abstract void Interact(Player player);

	public virtual void InteractAlternate(Player player)
	{

	}

	#region IKitchenObjectParent

	public KitchenObject KitchenObject
	{
		get
		{
			return kitchenObject;
		}
		set
		{
			kitchenObject = value;
			if (value != null)
				OnAnyObjectPlaced?.Invoke(this, EventArgs.Empty);
		}
	}

	public Transform GetSnapPoint()
	{
		return counterTopPoint;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}

	#endregion
}
