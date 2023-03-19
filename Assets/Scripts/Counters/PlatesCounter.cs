using System;
using System.Collections;
using System.Collections.Generic;
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
			platesCount--;
			OnCountChanged?.Invoke(this, new OnCountChangedEventArgs { count = platesCount });
		}
	}

	private void Update()
	{
		spawnPlateTimer += Time.deltaTime;
		if (GameInstance.Instance.IsGamePlaying() && spawnPlateTimer > spawnTimerMax)
		{
			spawnPlateTimer -= spawnTimerMax;

			if (platesCount < platesMax)
			{
				platesCount++;
				OnCountChanged?.Invoke(this, new OnCountChangedEventArgs { count = platesCount });
			}
		}
	}
}
