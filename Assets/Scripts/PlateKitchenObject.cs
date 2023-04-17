using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

		AddIngridientServerRpc(GameInstanceMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
		return true;
	}

	public List<KitchenObjectSO> GetIngredients()
	{
		return ingridients;
	}

	[ServerRpc(RequireOwnership = false)]
	private void AddIngridientServerRpc(int kitchenObjectSoIndex)
	{
		AddIngridientClientRpc(kitchenObjectSoIndex);
	}

	[ClientRpc]
	private void AddIngridientClientRpc(int kitchenObjectSoIndex)
	{
		KitchenObjectSO kitchenObjectSO = GameInstanceMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSoIndex);
		ingridients.Add(kitchenObjectSO);

		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
	}

}
