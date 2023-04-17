using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	[SerializeField] private Button playMultiplayerButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private Button playSingleplayerButton;

	private void Awake()
	{
		playMultiplayerButton.onClick.AddListener(
			() =>
			{
				GameInstanceMultiplayer.bMultiplayer = true;
				Loader.Load(Loader.SceneName.LobbyScene);
			});
		quitButton.onClick.AddListener(
			() =>
			{
				Application.Quit();
			});

		playSingleplayerButton.onClick.AddListener(()=>
		{
			GameInstanceMultiplayer.bMultiplayer = false;
			Loader.Load(Loader.SceneName.LobbyScene);
		});
	}
}
