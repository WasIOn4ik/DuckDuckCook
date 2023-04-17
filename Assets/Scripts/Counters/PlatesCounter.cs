using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
	public event EventHandler<OnCountChangedEventArgs> OnCountChanged;
	public class OnCountChangedEventArgs : EventArgs
	{
		public int count;
	}

	[SerializeField] private KitchenObjectSO plateKitchenObjectSO;
	private float spawnPlateTimer;
	private float spawnTimerMax = 4f;
	private int platesCount;
	private int platesMax = 4;

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
			return;

		if (platesCount > 0)
		{
			KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

			InteractLogicServerRpc();
		}
	}

	private void Update()
	{
		if(!IsServer)
			return;

		spawnPlateTimer += Time.deltaTime;
		if (GameInstance.Instance.IsGamePlaying() && spawnPlateTimer > spawnTimerMax)
		{
			SpawnPlateClientRpc();
		}
	}

	[ClientRpc]
	private void SpawnPlateClientRpc()
	{
		spawnPlateTimer -= spawnTimerMax;

		if (platesCount < platesMax)
		{
			platesCount++;
			OnCountChanged?.Invoke(this, new OnCountChangedEventArgs { count = platesCount });
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc()
	{
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc()
	{
		platesCount--;
		OnCountChanged?.Invoke(this, new OnCountChangedEventArgs { count = platesCount });
	}
}
