using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

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
				cuttingProgress = 0;
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
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
						KitchenObject.DestroySelf();
					}
				}
			}
			else
			{
				KitchenObject.KitchenObjectParent = player;
			}
		}
	}

	public override void InteractAlternate(Player player)
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
			if (cuttingProgress >= recipe.cuttingProgressMax)
			{
				KitchenObject.DestroySelf();

				KitchenObject.SpawnKitchenObject(recipe.output, this);
			}
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
