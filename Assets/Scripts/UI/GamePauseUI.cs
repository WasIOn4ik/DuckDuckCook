using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{

	[SerializeField] private Button resumeButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button optionsMenuButton;

	private void Awake()
	{
		resumeButton.onClick.AddListener(
			() =>
			{
				GameInstance.Instance.ResumeGame();
			});
		mainMenuButton.onClick.AddListener(
			() =>
			{
				NetworkManager.Singleton.Shutdown();
				GameInstance.Instance.ResumeGame();
				Loader.Load(Loader.SceneName.MainMenuScene);
			});
		optionsMenuButton.onClick.AddListener(
			() =>
			{
				OptionsUI.Instance.Show();
			});
	}

	private void Start()
	{
		GameInstance.Instance.OnLocalGamePaused += GameInstance_OnLocalGamePaused;
		GameInstance.Instance.OnLocalGameUnpaused += GameInstance_OnLocalGameUnpaused;

		Hide();
	}

	private void GameInstance_OnLocalGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void GameInstance_OnLocalGamePaused(object sender, System.EventArgs e)
	{
		Show();
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
