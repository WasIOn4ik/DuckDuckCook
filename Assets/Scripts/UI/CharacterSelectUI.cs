using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
	[SerializeField] private Button menuButton;
	[SerializeField] private Button readyButton;
	[SerializeField] private TextMeshProUGUI lobbyNameText;
	[SerializeField] private TextMeshProUGUI lobbyCodeText;

	private void Awake()
	{
		menuButton.onClick.AddListener(() =>
		{
			Debug.Log("ToMenu");
			GameLobby.Instance.LeaveLobbyAsync();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.SceneName.MainMenuScene);
		});

		readyButton.onClick.AddListener(() =>
		{
			CharacterSelectReady.Instance.SetPlayerReady();
		});
	}

	private void Start()
	{
		Lobby lobby = GameLobby.Instance.GetLobby();

		StringBuilder sbName = new StringBuilder();

		sbName.Append("Lobby name: ").Append(lobby.Name);

		StringBuilder sbCode = new StringBuilder();

		sbCode.Append("Lobby name: ").Append(lobby.LobbyCode);

		lobbyNameText.text = sbName.ToString();

		lobbyCodeText.text = sbCode.ToString();
	}
}
