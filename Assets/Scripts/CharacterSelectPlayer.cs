using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
	[SerializeField] private int playerIndex;
	[SerializeField] private GameObject readyMarker;
	[SerializeField] private PlayerVisual playerVisual;
	[SerializeField] private Button kickPlayerButton;
	[SerializeField] private TextMeshPro playerNameText;

	private void Awake()
	{
		kickPlayerButton.onClick.AddListener(() =>
		{
			ulong ownerClientID = GameInstanceMultiplayer.Instance.GetPlayerDataByIndex(playerIndex).clientId;
			if (ownerClientID == NetworkManager.ServerClientId)
				return;

			Debug.Log("Trying kick");
			PlayerData playerData = GameInstanceMultiplayer.Instance.GetPlayerDataByIndex(playerIndex);
			GameLobby.Instance.KickPlayerAsync(playerData.playerId.ToString());
			GameInstanceMultiplayer.Instance.KickPlayer(playerData.clientId);
		});
	}

	private void Start()
	{
		GameInstanceMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameInstanceMultiplayer_OnPlayerDataNetworkListChanged;
		CharacterSelectReady.Instance.OnPlayerReadyChanged += CharacterSelectReady_OnPlayerReadyChanged;

		kickPlayerButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

		UpdatePlayer();
	}

	private void CharacterSelectReady_OnPlayerReadyChanged(object sender, System.EventArgs e)
	{
		UpdatePlayer();
	}

	private void GameInstanceMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
	{
		UpdatePlayer();
	}

	private void UpdatePlayer()
	{
		if (GameInstanceMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
		{
			Show();

			PlayerData playerData = GameInstanceMultiplayer.Instance.GetPlayerDataByIndex(playerIndex);
			readyMarker.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

			playerVisual.SetPlayerColor(GameInstanceMultiplayer.Instance.GetPlayerColor(playerData.colorId));

			playerNameText.text = playerData.playerName.ToString();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
