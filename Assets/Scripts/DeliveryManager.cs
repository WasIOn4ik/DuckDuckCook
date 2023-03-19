using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
	public event EventHandler OnWaitingRecipesChanged;
	public event EventHandler OnRecipeSuccess;
	public event EventHandler OnRecipeFailure;

	public static DeliveryManager Instance { get; private set; }
	[SerializeField] private RecipesListSO availableRecipesListSO;
	private List<RecipeSO> waitingRecipeSOList;
	private float spawnRecipeTimer;
	private float spawnRecipeTimerMax = 4f;
	private int maximumRecipes = 4;
	private int successfulRecipesAmount = 0;

	private void Awake()
	{
		Instance = this;
		waitingRecipeSOList = new();
	}

	private void Update()
	{
		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0f)
		{
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (GameInstance.Instance.IsGamePlaying() && waitingRecipeSOList.Count < maximumRecipes)
			{
				RecipeSO upcomingRecipe = availableRecipesListSO.recipeSOList[UnityEngine.Random.Range(0, availableRecipesListSO.recipeSOList.Count)];
				waitingRecipeSOList.Add(upcomingRecipe);
				OnWaitingRecipesChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
	{
		for (int i = 0; i < waitingRecipeSOList.Count; i++)
		{
			var recipe = waitingRecipeSOList[i];

			if (recipe.ingredients.Count == plateKitchenObject.GetIngredients().Count)
			{
				bool plateMatchesRecipe = true;

				foreach (var ingredient in recipe.ingredients)
				{
					bool match = false;
					foreach (var ingredientOnPlate in plateKitchenObject.GetIngredients())
					{
						if (ingredient == ingredientOnPlate)
						{
							match = true;
							break;
						}
					}

					if (!match)
					{
						plateMatchesRecipe = false;
					}
				}
				if (plateMatchesRecipe)
				{
					waitingRecipeSOList.RemoveAt(i);
					OnWaitingRecipesChanged?.Invoke(this, EventArgs.Empty);
					OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
					successfulRecipesAmount++;
					return;
				}

			}
		}

		OnRecipeFailure?.Invoke(this, EventArgs.Empty);
	}

	public List<RecipeSO> GetWaitingRecipes()
	{
		return waitingRecipeSOList;
	}

	public int GetSuccessRecipesAmount()
	{
		return successfulRecipesAmount;
	}
}
