using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter
{
	public event EventHandler OnPlayerGrabObject;

	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
			return;

		KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
		OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
	}
}
