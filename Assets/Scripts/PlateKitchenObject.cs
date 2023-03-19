using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

	public class OnIngredientAddedEventArgs : EventArgs
	{
		public KitchenObjectSO kitchenObjectSO;
	}

	[SerializeField] private List<KitchenObjectSO> validObjects;

	private List<KitchenObjectSO> ingridients = new();
	public bool TryAddIngridient(KitchenObjectSO kitchenObjectSO)
	{
		if (ingridients.Contains(kitchenObjectSO))
			return false;

		if (!validObjects.Contains(kitchenObjectSO))
			return false;

		ingridients.Add(kitchenObjectSO);

		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
		return true;
	}

	public List<KitchenObjectSO> GetIngredients()
	{
		return ingridients;
	}

}
