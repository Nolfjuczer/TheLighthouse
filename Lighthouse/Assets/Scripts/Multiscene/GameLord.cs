﻿using System;
using UnityEngine;
using System.Collections;
using System.Linq;

/*
	#region Variables

	#endregion Variables

	#region Monobehaviour Methods

	#endregion Monobehaviour Methods

	#region Methods

	#endregion Methods
*/

public class GameLord : MonoBehaviour
{
	#region Variables

	private PlayerData _currentPlayerData = null;
	public PlayerData CurrentPlayerData { get { return _currentPlayerData; } }

	private static GameLord _instance = null;
	public static GameLord Instance
	{
		get
		{
			if (_instance == null)
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName_start, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			}
			return _instance;
		}
	}

	public enum GameState
	{
		GS_MENU = 0,
		GS_LOADING = 1,
		GS_GAME = 2
	}

	private GameState _currentGameState = GameState.GS_MENU;
	public GameState CurrentGameState { get { return _currentGameState; } }

	const string sceneName_start = "Start";

	private string _currentSceneName = "";
	public string CurrentSceneName {  get { return _currentSceneName; } }

	[SerializeField]
	private GUILord _guiLord = null;

	[SerializeField]
	private string[] _levels = null;
	public string[] Levels {  get { return _levels; } }
	[SerializeField]
	[HideInInspector]
	private int _levelCount = 0;

    public Action OnReloadStage;
	
	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateGameLord();
    }

	void OnApplicationPause(bool pauseStatus)
	{
		SavePlayerData();
    }
	void OnApplicationQuit()
	{
		SavePlayerData();
	}

	void Awake()
	{
		InitializeGameLord();
	}
	void Start()
	{
		if(_guiLord == null)
		{
			_guiLord = GUILord.Instance;
		}
		if (_guiLord != null)
		{
			_guiLord.OnPostGUIStateChanged += OnPostGUIStateChanged;
		}
	}

	void Update()
	{

	}
	#endregion Monobehaviour Methods

	#region Methods

	private void ValidateGameLord()
	{
		_levelCount = _levels != null ? _levels.Length : 0;
	}

	private void InitializeGameLord()
	{
		_currentPlayerData = PlayerData.Load();

		_instance = this;

		UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
           _currentSceneName = activeScene.name;
           if (_currentSceneName == sceneName_start)
		{
			_currentGameState = GameState.GS_MENU;
			_currentSceneName = "";
           } else {
			_currentGameState = GameState.GS_GAME;
		}
	}

	void SavePlayerData()
	{
		PlayerData.Save(_currentPlayerData);
	}

	public void ChangeGameState(GameState newGameState)
	{
		_currentGameState = newGameState;
		OnGameStateChanged(_currentGameState);
    }

	private void OnGameStateChanged(GameState newGameState)
	{
		switch (newGameState)
		{
			case GameState.GS_MENU:
				_guiLord.ChangeGUIState(GUILord.GUIState.GUIS_MENU);
				break;
			case GameState.GS_LOADING:
				_guiLord.ChangeGUIState(GUILord.GUIState.GUIS_LOADING);
				break;
			case GameState.GS_GAME:
				_guiLord.ChangeGUIState(GUILord.GUIState.GUIS_GAME);
				//ActiveController.Instance.ResetActiveController();

				break;
		}
	}

	private void OnPostGUIStateChanged(GUILord.GUIState newState)
	{
		switch(newState)
		{
			case GUILord.GUIState.GUIS_MENU:
				PlayerData.Save(_currentPlayerData);
				break;
			case GUILord.GUIState.GUIS_GAME:
				{
					//Debug.LogFormat("Set to pregame");
					GameController gameControllerInstance = GameController.Instance;
					if (gameControllerInstance != null)
					{
						gameControllerInstance.GameState = EGameState.PreGame;
					}
					GUIController.Instance.ChangeHudState(GUIController.HUDState.HS_GAME);
				}
				break;
			case GUILord.GUIState.GUIS_LOADING:
				break;
		}
	}

	public void LoadScene(string sceneName)
	{
        if (OnReloadStage != null) OnReloadStage();
        if (_levels.Contains(sceneName))
		{
			StartCoroutine(LoadSceneCoroutine(sceneName));
		}
	}

	private IEnumerator LoadSceneCoroutine(string sceneName)
	{
		ChangeGameState(GameState.GS_LOADING);
		
		while(_guiLord.IsTransition)
		{
			yield return null;
		}
		if(_currentSceneName != "")
		{
			UnityEngine.SceneManagement.SceneManager.UnloadScene(_currentSceneName);
			yield return null;
			Resources.UnloadUnusedAssets();
			yield return null;
           }

		AsyncOperation loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		while(!loadingOperation.isDone)
		{
			yield return null;
		}
		_currentSceneName = sceneName;
		ChangeGameState(GameState.GS_GAME);
		yield return null;
	} 
	
	public void SwitchToMenu()
    {
        if (OnReloadStage != null) OnReloadStage();
        StartCoroutine(SwitchToMenuCoroutine());
	}

	private IEnumerator SwitchToMenuCoroutine()
	{
		GUILord instance = GUILord.Instance;
		ChangeGameState(GameState.GS_LOADING);
		while(instance.IsTransition)
		{
			yield return null;
		}
		if(_currentSceneName  != "")
		{
			UnityEngine.SceneManagement.SceneManager.UnloadScene(_currentSceneName);
			yield return null;
			Resources.UnloadUnusedAssets();
			yield return null;
		}
		ChangeGameState(GameState.GS_MENU);
	}

	public void NextLevel()
	{
		if(CurrentGameState == GameState.GS_GAME)
		{
			int currentLevelIndex = -1;
			for(int i = 0;i < _levelCount;++i)
			{
				if(_levels[i] == _currentSceneName)
				{
					currentLevelIndex = i;
					break;
				}
			}
			if(currentLevelIndex != -1 && currentLevelIndex < _levelCount - 1)
			{
				LoadScene(_levels[currentLevelIndex + 1]);
			}
		}
	}

	#endregion Methods
	
}