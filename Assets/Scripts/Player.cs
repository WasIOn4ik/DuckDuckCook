using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
	public static event EventHandler OnLocalPlayerSpawned;
	public static event EventHandler OnAnyPickedSomething;
	public static Player LocalInstance { get; private set; }

	public event EventHandler OnPickedSomething;

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public class OnSelectedCounterChangedEventArgs : EventArgs
	{
		public BaseCounter selectedCounter;
	}

	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private float rotationSpeed = 10f;
	[SerializeField] private float playerRadius = 1.0f;
	[SerializeField] private float interactionDistance = 2.0f;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private LayerMask collisionsLayerMask;
	[SerializeField] private Transform snapPoint;
	[SerializeField] private List<Vector3> spawnPositionList;
	[SerializeField] private PlayerVisual playerVisual;

	private BaseCounter selectedCounter;

	private KitchenObject kitchenObject;

	private bool isWalking;

	private void Start()
	{
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
		GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

		PlayerData playerData = GameInstanceMultiplayer.Instance.GetPlayerDataFromClientID(OwnerClientId);
		playerVisual.SetPlayerColor(GameInstanceMultiplayer.Instance.GetPlayerColor(playerData.colorId));
	}

	private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if (!GameInstance.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (IsOwner)
		{
			LocalInstance = this;
			OnLocalPlayerSpawned?.Invoke(this, EventArgs.Empty);
		}
		transform.position = spawnPositionList[GameInstanceMultiplayer.Instance.GetPlayerDataIndexFromClientID(OwnerClientId)];

		if (IsServer)
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
	{
		if (clientID == OwnerClientId && HasKitchenObject())
		{
			KitchenObject.DestroyKitchenObject(KitchenObject);
		}
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (!GameInstance.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.Interact(this);
		}
	}

	private void Update()
	{
		if (!IsOwner)
			return;

		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleInteractions()
	{
		Vector2 movementInput = GameInput.Instance.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(movementInput.x, 0.0f, movementInput.y);
		if (moveDir == Vector3.zero)
			return;

		BaseCounter old = selectedCounter;

		if (Physics.Raycast(transform.position, moveDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
		{
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
			{
				if (baseCounter != selectedCounter)
				{
					selectedCounter = baseCounter;
				}
			}
			else
			{
				selectedCounter = null;
			}
		}
		else
		{
			selectedCounter = null;
		}

		if (selectedCounter != old)
		{
			OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter });
		}
	}

	private void HandleMovement()
	{
		Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0.0f, inputVector.y);

		float moveDistance = moveSpeed * Time.deltaTime;

		bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

		transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
		if (!canMove)
		{
			Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
			canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

			if (canMove)
				moveDir = moveDirX;
			else
			{
				Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
				canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

				if (canMove)
					moveDir = moveDirZ;
			}
		}


		if (canMove)
		{
			transform.position += moveDir * Time.deltaTime * moveSpeed;
		}

		isWalking = moveDir != Vector3.zero;
	}

	private void SetSelectedCounter(BaseCounter selectedCounter)
	{
		this.selectedCounter = selectedCounter;
	}

	#region IKitchenObjectParent

	public KitchenObject KitchenObject
	{
		get
		{
			return kitchenObject;
		}
		set
		{
			kitchenObject = value;
			if (value != null)
			{
				OnPickedSomething?.Invoke(this, EventArgs.Empty);
				OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public Transform GetSnapPoint()
	{
		return snapPoint;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}

	public NetworkObject GetNetworkObject()
	{
		return NetworkObject;
	}

	#endregion

	public static void ResetStaticData()
	{
		OnLocalPlayerSpawned = null;
		OnAnyPickedSomething = null;
	}
}
