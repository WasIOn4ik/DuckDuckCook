using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
			player.KitchenObject.DestroySelf();

			OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
		}
	}
}