using System.Collections;
using System.Collections.Generic;
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
		GameInstance.Instance.OnGamePaused += GameInstance_OnGamePaused;
		GameInstance.Instance.OnGameUnpaused += GameInstance_OnGameUnpaused;

		Hide();
	}

	private void GameInstance_OnGameUnpaused(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void GameInstance_OnGamePaused(object sender, System.EventArgs e)
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
