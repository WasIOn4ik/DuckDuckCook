using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
	public enum SceneName
	{
		MainMenuScene,
		GameScene,
		LoadingScene
	}

	private static SceneName _targetScene;

	public static void Load(SceneName targetScene)
	{
		_targetScene = targetScene;
		SceneManager.LoadScene(Loader.SceneName.LoadingScene.ToString());
	}

	public static void LoaderCallback()
	{
		SceneManager.LoadScene(_targetScene.ToString());
	}
}
