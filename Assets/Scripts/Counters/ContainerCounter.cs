using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

		InteractLogicServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc()
	{
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc()
	{
		OnPlayerGrabObject?.Invoke(this, EventArgs.Empty);
	}
}
