using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData()
	{
		OnAnyCut = null;
	}
	public event EventHandler OnCut;
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	[SerializeField] private CuttingRecipesSO[] recipes;

	private int cuttingProgress;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			if (player.HasKitchenObject() && ValidateKitchenObjectDrop(player.KitchenObject.GetKitchenObjectSO()))
			{
				player.KitchenObject.KitchenObjectParent = this;
				InteractLogicPlaceObjectOnCounterServerRpc();
			}
		}
		else
		{
			if (player.HasKitchenObject())
			{
				if (player.KitchenObject.TryGetPlate(out var plateKitchenObject))
				{
					if (plateKitchenObject.TryAddIngridient(KitchenObject.GetKitchenObjectSO()))
					{
						KitchenObject.DestroyKitchenObject(KitchenObject);
					}
				}
			}
			else
			{
				KitchenObject.KitchenObjectParent = player;
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceObjectOnCounterServerRpc()
	{
		InteractLogicPlaceObjectOnCounterClientRpc();
	}

	[ClientRpc]
	private void InteractLogicPlaceObjectOnCounterClientRpc()
	{
		cuttingProgress = 0;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
	}

	[ServerRpc(RequireOwnership = false)]
	private void TestCuttingProgressDoneServerRpc()
	{
		CuttingRecipesSO recipe = GetRecipe(KitchenObject.GetKitchenObjectSO());
		//Попытка порезать завершенный предмет
		if (recipe == null)
			return;

		if (cuttingProgress >= recipe.cuttingProgressMax)
		{
			KitchenObject.DestroyKitchenObject(KitchenObject);

			KitchenObject.SpawnKitchenObject(recipe.output, this);
		}
	}

	public override void InteractAlternate(Player player)
	{
		if(KitchenObject != null)
		{
			CutObjectServerRpc();
			TestCuttingProgressDoneServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void CutObjectServerRpc()
	{
		CutObjectClientRpc();
	}

	[ClientRpc]
	private void CutObjectClientRpc()
	{
		if (HasKitchenObject())
		{
			CuttingRecipesSO recipe = GetRecipe(KitchenObject.GetKitchenObjectSO());

			if (recipe == null)
				return;

			cuttingProgress++;
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = (float)cuttingProgress / recipe.cuttingProgressMax });
			OnCut?.Invoke(this, EventArgs.Empty);
			OnAnyCut?.Invoke(this, EventArgs.Empty);
		}
	}

	private CuttingRecipesSO GetRecipe(KitchenObjectSO input)
	{
		foreach (var recipe in recipes)
		{
			if (recipe.input == input)
				return recipe;
		}
		return null;
	}

	private KitchenObjectSO GetRecipeResult(KitchenObjectSO input)
	{
		var recipe = GetRecipe(input);

		return recipe == null ? null : recipe.output;
	}

	private bool ValidateKitchenObjectDrop(KitchenObjectSO kitchenObjectSO)
	{
		return GetRecipe(kitchenObjectSO) != null;
	}
}
