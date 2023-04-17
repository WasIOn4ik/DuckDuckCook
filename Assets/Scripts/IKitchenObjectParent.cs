using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    public KitchenObject KitchenObject { get; set; }

    public Transform GetSnapPoint();

    public void ClearKitchenObject();

    public bool HasKitchenObject();

    public NetworkObject GetNetworkObject();
}
