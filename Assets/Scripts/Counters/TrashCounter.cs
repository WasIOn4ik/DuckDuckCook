using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlatesCounter;

public class TrashCounter : BaseCounter
{
	public static event EventHandler OnAnyObjectTrashed;
	new public static void ResetStaticData()
	{
		OnAnyObjectTrashed = null;
	}

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
		{
			KitchenObject.DestroyKitchenObject(player.KitchenObject);
			InteractLogicServerRpc();
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
		OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
	}
}
