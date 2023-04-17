using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectReady : NetworkBehaviour
{
	public static CharacterSelectReady Instance { get; private set; }

	public event EventHandler OnPlayerReadyChanged;

	private Dictionary<ulong, bool> readyPlayersDictionary = new Dictionary<ulong, bool>();

	private void Awake()
	{
		Instance = this;
	}

	public void SetPlayerReady()
	{
		SendReadyServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SendReadyServerRpc(ServerRpcParams param = default)
	{
		SetPlayerReadyClientRpc(param.Receive.SenderClientId);
		readyPlayersDictionary[param.Receive.SenderClientId] = true;

		foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (!readyPlayersDictionary.ContainsKey(player) || !readyPlayersDictionary[player])
			{
				return;
			}
		}

		GameLobby.Instance.DeleteLobbyAsync();
		Loader.LoadNetwork(Loader.SceneName.GameScene);
	}

	[ClientRpc]
	private void SetPlayerReadyClientRpc(ulong clientID)
	{
		readyPlayersDictionary[clientID] = true;

		OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool IsPlayerReady(ulong clientID)
	{
		return readyPlayersDictionary.ContainsKey(clientID) && readyPlayersDictionary[clientID];
	}
}
