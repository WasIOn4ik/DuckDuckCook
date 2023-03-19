using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
	public event EventHandler OnStateChanged;
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameUnpaused;

	public static GameInstance Instance { get; private set; }

	private enum State
	{
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	private bool isGamePaused = false;

	private State state;
	[SerializeField] private float countdownToStartTimer = 3f;
	[SerializeField] private float gamePlayingTimerMax = 10f;
	[SerializeField] private float gamePlayingTimer;

	private void Awake()
	{
		Instance = this;
		state = State.WaitingToStart;
	}

	private void Start()
	{
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	private void GameInput_OnInteractAction(object sender, EventArgs e)
	{
		if (state == State.WaitingToStart)
		{
			state = State.CountdownToStart;
			OnStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e)
	{
		if (isGamePaused)
			ResumeGame();
		else
			PauseGame();
	}

	private void Update()
	{
		switch (state)
		{
			case State.CountdownToStart:
				countdownToStartTimer -= Time.deltaTime;
				if (countdownToStartTimer < 0f)
				{
					state = State.GamePlaying;
					gamePlayingTimer = gamePlayingTimerMax;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case State.GamePlaying:
				gamePlayingTimer -= Time.deltaTime;
				if (gamePlayingTimer < 0f)
				{
					state = State.GameOver;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case State.GameOver:
				break;
		}
	}

	public bool IsGamePlaying()
	{
		return state == State.GamePlaying;
	}

	public bool IsCountdownToStartActive()
	{
		return state == State.CountdownToStart;
	}

	public float GetCountDownToStartTimer()
	{
		return countdownToStartTimer;
	}

	public bool IsGameOver()
	{
		return state == State.GameOver;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - gamePlayingTimer / gamePlayingTimerMax;
	}

	private void PauseGame()
	{
		Time.timeScale = 0f;
		isGamePaused = true;
		OnGamePaused?.Invoke(this, EventArgs.Empty);
	}
	public void ResumeGame()
	{
		Time.timeScale = 1f;
		isGamePaused = false;
		OnGameUnpaused?.Invoke(this, EventArgs.Empty);
	}
}
