using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstanceMultiplayer : NetworkBehaviour
{
	public static bool bMultiplayer;

	public const int MAX_PLAYERS = 4;
	private const string PLAYER_PREFS_PLAYER_NAME = "playerName";

	[SerializeField] private KitchenObjectsListSO kitchenObjectsList;
	[SerializeField] private List<Color> playerColors;

	private NetworkList<PlayerData> playerDataNetworkList;

	private string playerName;

	public event EventHandler OnTryingTOJoinGame;

	public event EventHandler OnFailedToJoinGame;

	public event EventHandler OnPlayerDataNetworkListChanged;

	public static GameInstanceMultiplayer Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);

		playerDataNetworkList = new NetworkList<PlayerData>();
		playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

		playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, "NewPlayer" + UnityEngine.Random.Range(1, 10000).ToString());
	}

	private void Start()
	{
		if(!bMultiplayer)
		{
			StartHost();
			Loader.LoadNetwork(Loader.SceneName.GameScene);
		}
	}

	public string GetPlayerName()
	{
		return playerName;
	}

	public void SetPlayerName(string newPlayerName)
	{
		playerName = newPlayerName;

		PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, playerName);
	}

	private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
	{
		OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
	}

	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApproval;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectedCallback;
		NetworkManager.Singleton.StartHost();
	}

	private void NetworkManager_Server_OnClientDisconnectedCallback(ulong clientID)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			PlayerData playerData = playerDataNetworkList[i];
			if (playerData.clientId == clientID)
			{
				playerDataNetworkList.RemoveAt(i);
			}
		}
	}

	private void NetworkManager_OnClientConnectedCallback(ulong clientID)
	{
		playerDataNetworkList.Add(new PlayerData { clientId = clientID, colorId = GetFirstAvailableColorIndex() });
		SetPlayerNameAndIdServerRpc(GetPlayerName(), AuthenticationService.Instance.PlayerId);
	}

	private void NetworkManager_ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if (request.ClientNetworkId == NetworkManager.ServerClientId)
		{
			response.Approved = true;
			return;
		}

		if (SceneManager.GetActiveScene().name != Loader.SceneName.CharacterSelectScene.ToString())
		{
			response.Approved = false;
			response.Reason = "Game has already started";
			return;
		}

		if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS)
		{
			response.Approved = false;
			response.Reason = "Game is full";
			return;
		}
		response.Approved = true;
	}

	public void StartClient()
	{
		OnTryingTOJoinGame?.Invoke(this, EventArgs.Empty);
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectCallback;
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_Client_OnClientConnectCallback(ulong clientID)
	{
		SetPlayerNameAndIdServerRpc(GetPlayerName(), AuthenticationService.Instance.PlayerId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerNameAndIdServerRpc(string playerName, string playerId, ServerRpcParams param = default)
	{

		int playerDataIndex = GetPlayerDataIndexFromClientID(param.Receive.SenderClientId);
		PlayerData playerData = GetPlayerDataByIndex(playerDataIndex);
		playerData.playerName = playerName;
		playerData.playerId = playerId;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
	{
		OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
	}

	public bool IsPlayerIndexConnected(int index)
	{
		return index < playerDataNetworkList.Count;
	}

	public void SpawnKitchenObject(KitchenObjectSO objectSO, IKitchenObjectParent parent)
	{
		SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(objectSO), parent.GetNetworkObject());
	}

	public void DestroyKitchenObject(KitchenObject kitchenObjectToDestroy)
	{
		DestroyKitchenObjectServerRpc(kitchenObjectToDestroy.NetworkObject);
	}

	public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
	{
		return kitchenObjectsList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
	}

	public KitchenObjectSO GetKitchenObjectFromIndex(int index)
	{
		return kitchenObjectsList.kitchenObjectSOList[index];
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnKitchenObjectServerRpc(int kitchenObjectID, NetworkObjectReference kitchenObjectParentNetworkObject)
	{
		Transform prefab = GetKitchenObjectFromIndex(kitchenObjectID).prefab;
		KitchenObject kitchenObject = Instantiate(prefab).GetComponent<KitchenObject>();
		kitchenObject.GetComponent<NetworkObject>().Spawn(true);
		if (kitchenObjectParentNetworkObject.TryGet(out var networkObject))
		{
			IKitchenObjectParent kitchenObjectParent = networkObject.GetComponent<IKitchenObjectParent>();
			if (kitchenObjectParent == null)
			{
				return;
			}
			kitchenObject.KitchenObjectParent = kitchenObjectParent;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void DestroyKitchenObjectServerRpc(NetworkObjectReference networkObjectToDestroy)
	{
		if (networkObjectToDestroy.TryGet(out var networkObject))
		{
			ClearKitchenObjectOnParentClientRpc(networkObjectToDestroy);
			networkObject.GetComponent<KitchenObject>().DestroySelf();
		}
	}

	[ClientRpc]
	private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
	{
		kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
		kitchenObjectNetworkObject.GetComponent<KitchenObject>().ClearKitchenObjectOnParent();
	}

	public PlayerData GetPlayerDataByIndex(int playerIndex)
	{
		return playerDataNetworkList[playerIndex];
	}

	public Color GetPlayerColor(int colorID)
	{
		return playerColors[colorID];
	}

	public PlayerData GetPlayerData()
	{
		return GetPlayerDataFromClientID(NetworkManager.Singleton.LocalClientId);
	}

	public PlayerData GetPlayerDataFromClientID(ulong clientID)
	{
		foreach (var client in playerDataNetworkList)
		{
			if (client.clientId == clientID)
				return client;
		}
		return default;
	}
	public int GetPlayerDataIndexFromClientID(ulong clientID)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			if (playerDataNetworkList[i].clientId == clientID)
				return i;
		}
		return -1;
	}

	public void ChangePlayerColor(int colorID)
	{
		ChangePlayerColorServerRpc(colorID);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChangePlayerColorServerRpc(int colorID, ServerRpcParams param = default)
	{
		if (!IsColorAvailable(colorID))
		{
			return;
		}

		int playerDataIndex = GetPlayerDataIndexFromClientID(param.Receive.SenderClientId);
		PlayerData playerData = GetPlayerDataByIndex(playerDataIndex);
		playerData.colorId = colorID;
		playerDataNetworkList[playerDataIndex] = playerData;

	}

	private bool IsColorAvailable(int colorID)
	{
		foreach (var playerData in playerDataNetworkList)
		{
			if (playerData.colorId == colorID)
				return false;
		}
		return true;
	}

	private int GetFirstAvailableColorIndex()
	{
		for (int i = 0; i < playerColors.Count; i++)
		{
			if (IsColorAvailable(i))
				return i;
		}
		return -1;
	}

	public void KickPlayer(ulong clientID)
	{
		NetworkManager.Singleton.DisconnectClient(clientID);
		NetworkManager_Server_OnClientDisconnectedCallback(clientID);
	}
}
