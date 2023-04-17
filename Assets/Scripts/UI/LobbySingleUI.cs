using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySingleUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI lobbyNameText;

	private Lobby currentLobby;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			GameLobby.Instance.JoinLobbyByIDAsync(currentLobby.Id);
		});
	}

	public void SetLobby(Lobby lobby)
	{
		currentLobby = lobby;
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		lobbyNameText.text = currentLobby.Name;
	}
}
