using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button createLobbyButton;
	[SerializeField] private Button quickJoinButton;
	[SerializeField] private TMP_InputField lobbyCodeInputField;
	[SerializeField] private TMP_InputField playerNameInpurField;
	[SerializeField] private Button joinLobbyCodeButton;
	[SerializeField] private LobbyCreateUI lobbyCreateUI;
	[SerializeField] private Transform lobbyContainer;
	[SerializeField] private LobbySingleUI lobbyTemplate;


	private void Awake()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.LeaveLobbyAsync();
			Loader.Load(Loader.SceneName.MainMenuScene);
		});

		createLobbyButton.onClick.AddListener(() =>
		{
			lobbyCreateUI.Show();
		});

		quickJoinButton.onClick.AddListener(() =>
		{
			GameLobby.Instance.QuickJoinAsync();
		});

		joinLobbyCodeButton.onClick.AddListener(() =>
		{
			string lobbyCode = lobbyCodeInputField.text;
			GameLobby.Instance.JoinLobbyByCodeAsync(lobbyCode);
		});

		lobbyTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		playerNameInpurField.text = GameInstanceMultiplayer.Instance.GetPlayerName();
		playerNameInpurField.onValueChanged.AddListener((string newText) =>
			{
				GameInstanceMultiplayer.Instance.SetPlayerName(newText);
			});

		GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
		UpdateLobbyList(new List<Lobby>());
	}

	private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
	{
		UpdateLobbyList(e.lobbyList);
	}

	private void UpdateLobbyList(List<Lobby> lobbyList)
	{
		foreach(Transform child in lobbyContainer)
		{
			if(child == lobbyTemplate.transform)
				continue;

			Destroy(child.gameObject);
		}

		foreach(Lobby lobby in lobbyList)
		{
			LobbySingleUI lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
			lobbyTransform.SetLobby(lobby);
			lobbyTransform.gameObject.SetActive(true);
		}
	}

	private void OnDestroy()
	{
		GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
	}
}
