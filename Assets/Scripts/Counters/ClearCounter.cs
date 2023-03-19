using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			if (player.HasKitchenObject())
			{
				player.KitchenObject.KitchenObjectParent = this;
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
					}
				}
				else
				{
					if (KitchenObject.TryGetPlate(out var plateKitchenObject2))
					{
						if (plateKitchenObject2.TryAddIngridient(player.KitchenObject.GetKitchenObjectSO()))
						{
							player.KitchenObject.DestroySelf();
						}
					}
				}
			}
			else
			{
				KitchenObject.KitchenObjectParent = player;
			}
		}
	}
}
