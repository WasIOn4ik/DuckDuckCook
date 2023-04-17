using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
	public enum SceneName
	{
		MainMenuScene,
		GameScene,
		LoadingScene,
		LobbyScene,
		CharacterSelectScene
	}

	private static SceneName _targetScene;

	public static void Load(SceneName targetScene)
	{
		_targetScene = targetScene;
		SceneManager.LoadScene(Loader.SceneName.LoadingScene.ToString());
	}

	public static void LoadNetwork(SceneName targetScene)
	{
		NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
	}

	public static void LoaderCallback()
	{
		SceneManager.LoadScene(_targetScene.ToString());
	}
}
