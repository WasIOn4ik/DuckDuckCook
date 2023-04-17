using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
	[SerializeField] private TMP_InputField lobbyNameInputField;
	[SerializeField] private Toggle lobbyIsPrivateToggle;
	[SerializeField] private Button backBUtton;
	[SerializeField] private Button createLobbybutton;

	private void Awake()
	{
		createLobbybutton.onClick.AddListener(() =>
		{
			string lobbyName = lobbyNameInputField.text;
			bool isPrivate = lobbyIsPrivateToggle.isOn;
			GameLobby.Instance.CreateLobbyAsync(lobbyName, isPrivate);
		});

		backBUtton.onClick.AddListener(() =>
		{
			Hide();
		});

		Hide();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		lobbyIsPrivateToggle.SetIsOnWithoutNotify(false);
		lobbyNameInputField.text = "";
		gameObject.SetActive(false);
	}
}
