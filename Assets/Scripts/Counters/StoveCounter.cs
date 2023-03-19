using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public class OnStateChangedEventArgs : EventArgs
	{
		public State state;
	}
	public enum State
	{
		Idle,
		Frying,
		Fried,
		Burned
	}


	[SerializeField] private FryingRecipeSO[] fryingRecipeSOs;

	private float fryingTimer;
	private float burnTimer;

	private FryingRecipeSO recipe;
	private BurningRecipeSO burningRecipe;

	private State currentState;

	public State CurrentState
	{
		get => currentState;
		set
		{
			currentState = value;
			OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
		}
	}

	private void Start()
	{
		CurrentState = State.Idle;
	}

	private void Update()
	{
		if (HasKitchenObject())
		{
			switch (CurrentState)
			{
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer += Time.deltaTime;
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = fryingTimer / recipe.fryingTimerMax });
					if (fryingTimer > recipe.fryingTimerMax)
					{
						burnTimer = 0;
						CurrentState = State.Fried;

						KitchenObject.DestroySelf();

						KitchenObject.SpawnKitchenObject(recipe.output, this);
						burningRecipe = recipe.burningRecipe;
					}
					break;
				case State.Fried:
					burnTimer += Time.deltaTime;
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burnTimer / burningRecipe.burningTimerMax });
					if (burnTimer > burningRecipe.burningTimerMax)
					{
						CurrentState = State.Burned;

						KitchenObject.DestroySelf();

						KitchenObject.SpawnKitchenObject(burningRecipe.output, this);
						OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
					}
					break;
				case State.Burned:
					break;
			}
		}
	}
	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			if (player.HasKitchenObject() && ValidateKitchenObjectDrop(player.KitchenObject.GetKitchenObjectSO()))
			{
				player.KitchenObject.KitchenObjectParent = this;

				recipe = GetRecipe(KitchenObject.GetKitchenObjectSO());

				CurrentState = State.Frying;
				fryingTimer = 0f;
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
						CurrentState = State.Idle;
						OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
					}
				}
			}
			else
			{
				KitchenObject.KitchenObjectParent = player;
				CurrentState = State.Idle;
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
			}
		}
	}
	private FryingRecipeSO GetRecipe(KitchenObjectSO input)
	{
		foreach (var recipe in fryingRecipeSOs)
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

	public bool IsFried()
	{
		return currentState == State.Fried;
	}
}
