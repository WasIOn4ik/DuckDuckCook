using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class GameLobby : MonoBehaviour
{
	private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
	public static GameLobby Instance { get; private set; }

	private Lobby joinedLobby;
	private float heartbeatTimerMax = 15f;
	private float currentHeartbeatTImer;

	private float listLobbiesTimerMax = 5f;
	private float listLobbiesTimer;

	public event EventHandler OnCreateLobbyStarted;
	public event EventHandler OnCreateLobbyFailed;

	public event EventHandler OnJoinStarted;
	public event EventHandler OnJoinFailed;

	public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
	public class OnLobbyListChangedEventArgs : EventArgs
	{
		public List<Lobby> lobbyList;
	}

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);

		InitializeUnityAuthentifcationAsync();
		currentHeartbeatTImer = heartbeatTimerMax;
		listLobbiesTimer = listLobbiesTimerMax;
	}

	private void Update()
	{
		HandleHeartbeat();
		HandlePeriodicListLobbies();
	}

	private void HandlePeriodicListLobbies()
	{
		if (joinedLobby != null || AuthenticationService.Instance.IsSignedIn == false ||
			SceneManager.GetActiveScene().name != Loader.SceneName.LobbyScene.ToString())
			return;

		listLobbiesTimer -= Time.deltaTime;

		if (listLobbiesTimer <= 0)
		{
			listLobbiesTimer = listLobbiesTimerMax - listLobbiesTimer;

			ListLobbiesAsync();
		}
	}

	private void HandleHeartbeat()
	{
		if (IsLobbyHost())
		{
			currentHeartbeatTImer -= Time.deltaTime;
			if (currentHeartbeatTImer <= 0f)
			{
				currentHeartbeatTImer = heartbeatTimerMax - currentHeartbeatTImer;

				LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
			}
		}
	}

	private bool IsLobbyHost()
	{
		return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
	}

	private async void InitializeUnityAuthentifcationAsync()
	{
		if (UnityServices.State != ServicesInitializationState.Initialized)
		{
			InitializationOptions options = new InitializationOptions();
			options.SetProfile(UnityEngine.Random.Range(0, 100000).ToString());
			await UnityServices.InitializeAsync(options);

			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}

	private async Task<Allocation> AllocateRelayAsync()
	{
		try
		{
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameInstanceMultiplayer.MAX_PLAYERS - 1);

			return allocation;
		}
		catch (RelayServiceException exception)
		{
			Debug.Log(exception);

			return default;
		}
	}

	private async Task<string> GetRelayJoinCodeAsync(Allocation allocation)
	{
		try
		{
			string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			return relayJoinCode;
		}
		catch (RelayServiceException exception)
		{
			Debug.Log(exception);

			return "";
		}
	}

	private async Task<JoinAllocation> JoinRelay(string joinCode)
	{
		try
		{
			JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

			return joinAlloc;
		}
		catch (RelayServiceException exception)
		{
			Debug.Log(exception);

			return default;
		}
	}
	public async void CreateLobbyAsync(string lobbyName, bool isPrivate)
	{
		OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
		try
		{
			joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameInstanceMultiplayer.MAX_PLAYERS, new CreateLobbyOptions() { IsPrivate = isPrivate });

			Allocation allocation = await AllocateRelayAsync();

			string joinCode = await GetRelayJoinCodeAsync(allocation);

			await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions()
			{
				Data = new Dictionary<string, DataObject>
				{
					{KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
				}
			});

			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
			GameInstanceMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.SceneName.CharacterSelectScene);
		}
		catch (LobbyServiceException exception)
		{
			Debug.Log(exception);
			OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	public async void QuickJoinAsync()
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try
		{
			joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

			GameInstanceMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException exception)
		{
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
			Debug.Log(exception);
		}
	}

	public Lobby GetLobby()
	{
		return joinedLobby;
	}

	public async void JoinLobbyByCodeAsync(string code)
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try
		{
			joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

			GameInstanceMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException exception)
		{
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
			Debug.Log(exception);
		}
	}
	public async void JoinLobbyByIDAsync(string id)
	{
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try
		{
			joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);

			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

			GameInstanceMultiplayer.Instance.StartClient();
		}
		catch (LobbyServiceException exception)
		{
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
			Debug.Log(exception);
		}
	}

	public async void DeleteLobbyAsync()
	{
		try
		{
			if (joinedLobby != null)
			{
				await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

				joinedLobby = null;
			}
		}
		catch (LobbyServiceException exception)
		{
			Debug.Log(exception);
		}
	}

	public async void LeaveLobbyAsync()
	{
		if (joinedLobby != null)
		{
			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
				joinedLobby = null;
			}
			catch (LobbyServiceException exception)
			{
				Debug.Log(exception);
			}
		}
	}

	public async void KickPlayerAsync(string playerId)
	{
		if (IsLobbyHost())
		{
			try
			{
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
			}
			catch (LobbyServiceException exception)
			{
				Debug.Log(exception);
			}
		}
	}

	public async void ListLobbiesAsync()
	{
		try
		{
			QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
			{
				Filters = new List<QueryFilter>()
			{
				new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
			}
			};
			QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

			OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = response.Results });
		}
		catch (LobbyServiceException exception)
		{
			Debug.Log(exception);
		}
	}
}
