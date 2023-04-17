using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameInstance : NetworkBehaviour
{
	#region Variables

	public event EventHandler OnStateChanged;
	public event EventHandler OnLocalGamePaused;
	public event EventHandler OnLocalGameUnpaused;
	public event EventHandler OnMultiplayerGamePaused;
	public event EventHandler OnMultiplayerGameUnpaused;
	public event EventHandler OnLocalPlayerReadyChanged;


	private Dictionary<ulong, bool> readyPlayersDictionary = new Dictionary<ulong, bool>();
	private Dictionary<ulong, bool> pausedPlayersDictionary = new Dictionary<ulong, bool>();

	private bool isLocalGamePaused = false;
	private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

	private NetworkVariable<State> state = new NetworkVariable<State>();
	private bool isLocalPlayerReady;
	[SerializeField] private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
	[SerializeField] private float gamePlayingTimerMax = 10f;
	[SerializeField] private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(60);

	[SerializeField] private Transform playerPrefab;

	public static GameInstance Instance { get; private set; }

	#endregion

	#region Difinitions

	private enum State
	{
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	#endregion

	#region UnityCallbacks

	private void Awake()
	{
		Instance = this;
		state.Value = State.WaitingToStart;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		state.OnValueChanged += State_OnValueChanged;
		isGamePaused.OnValueChanged += GamePaused_OnValue_changed;
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkSceneManager_OnLoadEventCompleted;
		}
	}

	private void NetworkSceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
		{
			Transform playerTransform = Instantiate(playerPrefab);
			playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
		}
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
	{
		StartCoroutine(UpdatePauseStateOnDisconnect());
	}

	private IEnumerator UpdatePauseStateOnDisconnect()
	{
		yield return new WaitForEndOfFrame();

		UpdatePauseState();

		StopCoroutine(UpdatePauseStateOnDisconnect());
	}

	private void Start()
	{
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	private void Update()
	{
		if (!IsServer)
			return;

		switch (state.Value)
		{
			case State.CountdownToStart:
				countdownToStartTimer.Value -= Time.deltaTime;
				if (countdownToStartTimer.Value < 0f)
				{
					state.Value = State.GamePlaying;
					gamePlayingTimer.Value = gamePlayingTimerMax;
				}
				break;
			case State.GamePlaying:
				gamePlayingTimer.Value -= Time.deltaTime;
				if (gamePlayingTimer.Value < 0f)
				{
					state.Value = State.GameOver;
				}
				break;
			case State.GameOver:
				break;
		}
	}

	#endregion

	#region EventCallbacks

	private void GamePaused_OnValue_changed(bool previousValue, bool newValue)
	{
		if (newValue)
		{
			Time.timeScale = 0f;
			OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			Time.timeScale = 1f;
			OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
		}

	}

	private void State_OnValueChanged(State previousValue, State newValue)
	{
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void GameInput_OnInteractAction(object sender, EventArgs e)
	{
		if (state.Value == State.WaitingToStart)
		{
			isLocalPlayerReady = true;
			OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
			
			SendReadyServerRpc();
		}
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e)
	{
		if (isLocalGamePaused)
			ResumeGame();
		else
			PauseGame();
	}

	#endregion

	#region RPCs

	[ServerRpc(RequireOwnership = false)]
	private void PauseGameServerRpc(ServerRpcParams param = default)
	{
		pausedPlayersDictionary[param.Receive.SenderClientId] = true;

		UpdatePauseState();
	}

	[ServerRpc(RequireOwnership = false)]
	private void UnpauseGameServerRpc(ServerRpcParams param = default)
	{
		pausedPlayersDictionary[param.Receive.SenderClientId] = false;

		UpdatePauseState();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SendReadyServerRpc(ServerRpcParams param = default)
	{
		readyPlayersDictionary[param.Receive.SenderClientId] = true;

		foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (!readyPlayersDictionary.ContainsKey(player) || !readyPlayersDictionary[player])
			{
				return;
			}
		}

		state.Value = State.CountdownToStart;
	}

	#endregion

	#region Functions

	public bool IsLocalPlayerReady()
	{
		return isLocalPlayerReady;
	}

	public bool IsGamePlaying()
	{
		return state.Value == State.GamePlaying;
	}

	public bool IsCountdownToStartActive()
	{
		return state.Value == State.CountdownToStart;
	}

	public float GetCountDownToStartTimer()
	{
		return countdownToStartTimer.Value;
	}

	public bool IsGameOver()
	{
		return state.Value == State.GameOver;
	}

	public bool IsWaitingToStart()
	{
		return state.Value == State.WaitingToStart;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - gamePlayingTimer.Value / gamePlayingTimerMax;
	}

	private void PauseGame()
	{
		isLocalGamePaused = true;
		OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
		PauseGameServerRpc();
	}
	public void ResumeGame()
	{
		isLocalGamePaused = false;
		OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
		UnpauseGameServerRpc();
	}

	private void UpdatePauseState()
	{
		foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (pausedPlayersDictionary.ContainsKey(clientID) && pausedPlayersDictionary[clientID])
			{
				isGamePaused.Value = true;
				//Player is paused
				return;
			}
		}
		//Unpaused
		isGamePaused.Value = false;
	}

	#endregion
}
