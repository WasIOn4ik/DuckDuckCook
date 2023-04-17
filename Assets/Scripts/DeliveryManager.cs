using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
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
		spawnRecipeTimer = spawnRecipeTimerMax;
		Instance = this;
		waitingRecipeSOList = new();
	}

	private void Update()
	{
		if (!IsServer)
			return;

		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0f)
		{
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (GameInstance.Instance.IsGamePlaying() && waitingRecipeSOList.Count < maximumRecipes)
			{
				int upcomingRecipeID = UnityEngine.Random.Range(0, availableRecipesListSO.recipeSOList.Count);
				SpawnNewDeliveryRecipeClientRpc(upcomingRecipeID);
			}
		}
	}

	[ClientRpc(Delivery = RpcDelivery.Reliable)]
	private void SpawnNewDeliveryRecipeClientRpc(int upcomingRecipeID)
	{
		RecipeSO upcomingRecipe = availableRecipesListSO.recipeSOList[upcomingRecipeID];
		waitingRecipeSOList.Add(upcomingRecipe);
		OnWaitingRecipesChanged?.Invoke(this, EventArgs.Empty);
	}

	[ServerRpc(RequireOwnership = false)]
	private void DeliverCorrectRecipeServerRpc(int waitingRecipesListIndex)
	{
		DeliverCorrectRecipeClientRpc(waitingRecipesListIndex);
	}

	[ClientRpc]
	private void DeliverCorrectRecipeClientRpc(int waitingRecipesListIndex)
	{
		waitingRecipeSOList.RemoveAt(waitingRecipesListIndex);
		OnWaitingRecipesChanged?.Invoke(this, EventArgs.Empty);
		OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
		successfulRecipesAmount++;
	}

	[ServerRpc(RequireOwnership = false)]
	private void DeliverIncorrectRecipeServerRpc()
	{
		DeliverIncorrectRecipeClientRpc();
	}

	[ClientRpc]
	private void DeliverIncorrectRecipeClientRpc()
	{
		OnRecipeFailure?.Invoke(this, EventArgs.Empty);
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
					DeliverCorrectRecipeServerRpc(i);
					return;
				}

			}
		}

		DeliverIncorrectRecipeServerRpc();
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
