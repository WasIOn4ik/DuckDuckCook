using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

	private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
	private NetworkVariable<float> burnTimer = new NetworkVariable<float>(0f);

	private FryingRecipeSO recipe;
	private BurningRecipeSO burningRecipe;

	private NetworkVariable<State> currentState = new NetworkVariable<State>(State.Idle);

	public State CurrentState
	{
		get => currentState.Value;
		set
		{
			currentState.Value = value;
		}
	}

	private void Start()
	{
		CurrentState = State.Idle;
	}

	public override void OnNetworkSpawn()
	{
		fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
		burnTimer.OnValueChanged += BurnTimer_OnValueChanged;
		currentState.OnValueChanged += CurrentState_OnValueChanged;

	}

	private void CurrentState_OnValueChanged(State previousValue, State newValue)
	{
		OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState.Value });
		if(currentState.Value == State.Burned || currentState.Value == State.Idle)
		{
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
		}
	}

	private void BurnTimer_OnValueChanged(float previousValue, float newValue)
	{
		float burnTimerMax = burningRecipe != null ? burningRecipe.burningTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burnTimer.Value / burnTimerMax });

	}

	private void FryingTimer_OnValueChanged(float previousValue, float newValue)
	{
		float fryingTimerMax = recipe != null ? recipe.fryingTimerMax : 1f;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = fryingTimer.Value / fryingTimerMax });
	}

	private void Update()
	{
		if (!IsServer)
			return;

		if (HasKitchenObject())
		{
			switch (CurrentState)
			{
				case State.Idle:
					break;
				case State.Frying:
					fryingTimer.Value += Time.deltaTime;
					if (fryingTimer.Value > recipe.fryingTimerMax)
					{
						burnTimer.Value = 0;
						CurrentState = State.Fried;
						int kitchenObjectSOIndex = GameInstanceMultiplayer.Instance.GetKitchenObjectSOIndex(KitchenObject.GetKitchenObjectSO());
						KitchenObject.DestroyKitchenObject(KitchenObject);

						KitchenObject.SpawnKitchenObject(recipe.output, this);
						SetBurningRecipeSOClientRpc(kitchenObjectSOIndex);
					}
					break;
				case State.Fried:
					burnTimer.Value += Time.deltaTime;
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burnTimer.Value / burningRecipe.burningTimerMax });
					if (burnTimer.Value > burningRecipe.burningTimerMax)
					{
						CurrentState = State.Burned;

						KitchenObject.DestroyKitchenObject(KitchenObject);

						KitchenObject.SpawnKitchenObject(burningRecipe.output, this);
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
				KitchenObject kitchenObject = player.KitchenObject;

				player.KitchenObject.KitchenObjectParent = this;

				InteractLogicPlaceObjectOnCounterServerRpc(GameInstanceMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
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
						SetStateIdleServerRpc();
					}
				}
			}
			else
			{
				KitchenObject.KitchenObjectParent = player; 
				SetStateIdleServerRpc();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetStateIdleServerRpc()
	{
		currentState.Value = State.Idle;
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

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
	{
		fryingTimer.Value = 0f;
		CurrentState = State.Frying;
		SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
	}

	[ClientRpc]
	private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = GameInstanceMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOIndex);
		recipe = GetRecipe(kitchenObjectSO);
	}

	[ClientRpc]
	private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
	{
		KitchenObjectSO kitchenObjectSO = GameInstanceMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOIndex);
		burningRecipe = GetRecipe(kitchenObjectSO).burningRecipe;
	}

	private bool ValidateKitchenObjectDrop(KitchenObjectSO kitchenObjectSO)
	{
		return GetRecipe(kitchenObjectSO) != null;
	}

	public bool IsFried()
	{
		return currentState.Value == State.Fried;
	}
}
