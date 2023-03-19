using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
	public static Player Instance { get; private set; }

	public event EventHandler OnPickedSomething;

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public class OnSelectedCounterChangedEventArgs : EventArgs
	{
		public BaseCounter selectedCounter;
	}

	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private float rotationSpeed = 10f;
	[SerializeField] private float playerHeight = 2.0f;
	[SerializeField] private float playerRadius = 1.0f;
	[SerializeField] private float interactionDistance = 2.0f;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private Transform snapPoint;

	private BaseCounter selectedCounter;

	private KitchenObject kitchenObject;

	private bool isWalking;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Несколько экземпляров Player");
		}
		Instance = this;
	}

	private void Start()
	{
		gameInput.OnInteractAction += GameInput_OnInteractAction;
		gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
	}

	private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
	{
		if(!GameInstance.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
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
		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleInteractions()
	{
		Vector2 movementInput = gameInput.GetMovementVectorNormalized();
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
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0.0f, inputVector.y);

		float moveDistance = moveSpeed * Time.deltaTime;

		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

		transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
		if (!canMove)
		{
			Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
			canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

			if (canMove)
				moveDir = moveDirX;
			else
			{
				Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
				canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

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

	#endregion
}
