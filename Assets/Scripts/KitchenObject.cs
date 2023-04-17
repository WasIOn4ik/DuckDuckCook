using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class KitchenObject : NetworkBehaviour
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	private IKitchenObjectParent kitchenObjectParent;

	public static void SpawnKitchenObject(KitchenObjectSO objectSO, IKitchenObjectParent parent)
	{
		GameInstanceMultiplayer.Instance.SpawnKitchenObject(objectSO, parent);
	}

	public static void DestroyKitchenObject(KitchenObject kitchenObject)
	{
		GameInstanceMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
	}

	public IKitchenObjectParent KitchenObjectParent
	{
		get
		{
			return kitchenObjectParent;
		}
		set
		{
			CallReparentServerRpc(value.GetNetworkObject());
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void CallReparentServerRpc(NetworkObjectReference parent)
	{
		if (parent.TryGet(out var networkObjectParent))
		{
			NetworkObject.TrySetParent(networkObjectParent);
			OnParentChangedClientRpc(networkObjectParent.GetComponent<IKitchenObjectParent>().GetNetworkObject());
		}
	}

	[ClientRpc]
	private void OnParentChangedClientRpc(NetworkObjectReference networkObjectParent)
	{
		NetworkObject.transform.localPosition = transform.parent.GetComponent<IKitchenObjectParent>().GetSnapPoint().localPosition;
		
		if (kitchenObjectParent != null)
		{
			kitchenObjectParent.ClearKitchenObject();
		}

		if(networkObjectParent.TryGet(out var networkObject))
		{
			kitchenObjectParent = networkObject.GetComponent<IKitchenObjectParent>();

			if (kitchenObjectParent.HasKitchenObject())
			{
				Debug.LogError("Counter already has a kitchenObject");
				return;
			}

			kitchenObjectParent.KitchenObject = this;
		}
	}

	public KitchenObjectSO GetKitchenObjectSO()
	{
		return kitchenObjectSO;
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void ClearKitchenObjectOnParent()
	{
		KitchenObjectParent?.ClearKitchenObject();
	}

	public bool TryGetPlate(out PlateKitchenObject plate)
	{
		if (this is PlateKitchenObject)
		{
			plate = this as PlateKitchenObject;
			return true;
		}

		plate = null;
		return false;
	}
}
