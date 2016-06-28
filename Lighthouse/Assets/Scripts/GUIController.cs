using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GUIController : Singleton<GUIController>
{
	#region Variables

	#region General GUI
	public enum HUDState : int
	{
		HS_GAME = 0,
		HS_PAUSE = 1,
		HS_WIN = 2,
		HS_LOST = 3,

		HS_COUNT,
		HS_NONE
	}

	[System.Serializable]
	public struct HUDStateInfo
	{
		public HUDState state;
		public GameObject panel;
	}

	[SerializeField]
	private HUDStateInfo[] _hudStateInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _hudStateInfoCount = 0;

	private HUDState _currentHudState = HUDState.HS_GAME;

	[SerializeField]
	private GameObject Pause;
	[SerializeField]
	private GameObject ActiveButtons;

	#endregion General GUI

	#region Score GUI

	[SerializeField]
	private Text Score;

	[SerializeField]
	private LivesController _livesController = null;
	public LivesController LivesController { get { return _livesController; } }

	[SerializeField]
	private EndGameGUI _pauseStats = null;
	[SerializeField]
	private EndGameGUI WinStats;
	[SerializeField]
	private EndGameGUI LoseStats;

	#endregion Score GUI

	#region Active Skills GUI

	[System.Serializable]
	public struct ActiveSkillGUIInfo
	{
		public static Color disabledColor = new Color(0.1f, 0.1f, 0.1f, 40.0f / 255.0f);
		public static Color[] defaultColors = new Color[]
		{
			new Color(1f, 0f, 0f, 40f/255f),
			new Color(0f, 1f, 0f, 40f/255f),
			new Color(0f, 1f, 1f, 40f/255f),
			new Color(1f, 1f, 0f, 40f/255f)
		};

		public ActiveSkillsEnum type;
		public GameObject panel;
		public Color defaultBackgroundColor;
		public Image background;
		public Button button;

		public void UpdateActive(float progres)
		{
			bool active = progres > 0.0f;
			if(panel.activeSelf != active)
			{
				panel.SetActive(active);
			}
			if (active)
			{
				background.fillAmount = progres;
				background.color = Color.Lerp(disabledColor, defaultBackgroundColor, progres);
				button.interactable = progres >= 1.0f;
			}
		}
	}

	[SerializeField]
	private ActiveSkillGUIInfo[] _activeSkillGUIInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _activeSkillsGUIInfoCount = 0;

	[SerializeField]
	private Image _activeSkillScreen;
	[SerializeField]
	private bool _activeExpanded;

	#endregion Active Skills GUI

	#region Power Ups GUI

	[System.Serializable]
	public struct PowerUpInfo
	{
		public PowerUpType type;
		public Image image;
	}

	[SerializeField]
	private PowerUpInfo[] _powerUpInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _powerUpInfoCount = 0;

	#endregion Power Ups GUI

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateGUIController();
	}

	void OnEnable()
	{
		ResetHudState();
		if(_livesController != null)
		{
			_livesController.ResetLifes();
		}
	    //for (int i = 0; i < Lives.Length; ++i) Lives[i].enabled = true;
	}

	public void Update()
    {
		if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME)
		{
			Score.text = string.Format("$ {0}",GameController.Instance.Money);
		}
    }

	#endregion Monobehaviour Methods

	#region Methods

    public void OnWinGame(int total, ShipCounterType current, ShipCounterType goal)
    {
        WinStats.TotalScore.text = string.Format("Cash {0}$", total.ToString());
        WinStats.FerryScore.text = string.Format("{0} / {1}", current.FerryCount, goal.FerryCount);
        WinStats.FreighterScore.text = string.Format("{0} / {1}", current.FreighterCount, goal.FreighterCount);
        WinStats.KeelboatScore.text = string.Format("{0} / {1}", current.KeelboatCount, goal.KeelboatCount);
        WinStats.MotorboatScore.text = string.Format("{0} / {1}", current.MotorboatCount, goal.MotorboatCount);
        ChangeHudState(HUDState.HS_WIN);
    }

    public void OnLoseGame(int total, ShipCounterType current, ShipCounterType goal)
    {
        LoseStats.TotalScore.text = string.Format("Cash {0}$", total.ToString());
        LoseStats.FerryScore.text = string.Format("{0} / {1}", current.FerryCount, goal.FerryCount);
        LoseStats.FreighterScore.text = string.Format("{0} / {1}", current.FreighterCount, goal.FreighterCount);
        LoseStats.KeelboatScore.text = string.Format("{0} / {1}", current.KeelboatCount, goal.KeelboatCount);
        LoseStats.MotorboatScore.text = string.Format("{0} / {1}", current.MotorboatCount, goal.MotorboatCount);
        ChangeHudState(HUDState.HS_LOST);
    }

    public void OnPauseClick()
    {
        GameController.Instance.GameState = EGameState.Paused;
		//Pause.SetActive(true);
		ChangeHudState(HUDState.HS_PAUSE);
    }

    public void OnResumeClick()
    {
        GameController.Instance.GameState = EGameState.InGame;
		//Pause.SetActive(false);
		ChangeHudState(HUDState.HS_GAME);
    }

    public void OnRestartClick()
    {
        GameController.Instance.GameState = EGameState.InGame;
		GameLord.Instance.LoadScene(GameLord.Instance.CurrentSceneName);
    }

	public void OnNextClick()
	{
		//TODO
	}

    public void OnMenuClick()
    {
        GameController.Instance.GameState = EGameState.InGame;
		//SceneManager.LoadScene(0);
		GameLord.Instance.SwitchToMenu();
    }

    public void OnExpandActive()
    {
        _activeExpanded = !_activeExpanded;
        ActiveButtons.SetActive(_activeExpanded);
    }

    public void OnActiveChosen()
    {
        //CurrentActive.gameObject.SetActive(true);
        _activeExpanded = false;
        ActiveButtons.SetActive(false);
    }

	private void ValidateGUIController()
	{
		_hudStateInfoCount = (int)HUDState.HS_COUNT;
		HUDStateInfo[] oldHudStateInfos = _hudStateInfos;
		int oldHudStateInfoCount = oldHudStateInfos != null ? oldHudStateInfos.Length : 0;
		if(oldHudStateInfoCount != _hudStateInfoCount)
		{
			_hudStateInfos = new HUDStateInfo[_hudStateInfoCount];
		}
		for(int i = 0;i < _hudStateInfoCount;++i)
		{
			if(i < oldHudStateInfoCount)
			{
				_hudStateInfos[i] = oldHudStateInfos[i];
			}
			_hudStateInfos[i].state = (HUDState)i;
		}

		ActiveSkillGUIInfo[] oldActiveSkillGUIInfos = _activeSkillGUIInfos;
		int oldActiveSkillGUIInfoCount = oldActiveSkillGUIInfos != null ? oldActiveSkillGUIInfos.Length : 0;
		_activeSkillsGUIInfoCount = (int)ActiveSkillsEnum.COUNT;
		if(oldActiveSkillGUIInfoCount != _activeSkillsGUIInfoCount)
		{
			_activeSkillGUIInfos = new ActiveSkillGUIInfo[_activeSkillsGUIInfoCount];
		}
		for(int i = 0;i < _activeSkillsGUIInfoCount;++i)
		{
			if(i < oldActiveSkillGUIInfoCount)
			{
				_activeSkillGUIInfos[i] = oldActiveSkillGUIInfos[i];
			}
			_activeSkillGUIInfos[i].type = (ActiveSkillsEnum)i;
			_activeSkillGUIInfos[i].defaultBackgroundColor = ActiveSkillGUIInfo.defaultColors[i];
		}

		PowerUpInfo[] oldPowerUpInfos = _powerUpInfos;
		int oldPowerUpInfoCount = oldPowerUpInfos != null ? oldPowerUpInfos.Length : 0;
		_powerUpInfoCount = (int)PowerUpType.COUNT;
		if(oldPowerUpInfoCount != _powerUpInfoCount)
		{
			_powerUpInfos = new PowerUpInfo[_powerUpInfoCount];
		}
		for(int i = 0;i < _powerUpInfoCount;++i)
		{
			if(i < oldPowerUpInfoCount)
			{
				_powerUpInfos[i] = oldPowerUpInfos[i];
			}
			_powerUpInfos[i].type = (PowerUpType)i;
		}
	}

	private void ResetHudState()
	{
		_currentHudState = HUDState.HS_GAME;
		for(int i = 0;i < _hudStateInfoCount;++i)
		{
			HUDState tmpState = (HUDState)i;
			SetPanelActive(tmpState, tmpState == _currentHudState);
		}
	}

	private void SetPanelActive(HUDState state, bool active)
	{
		int index = (int)state;
		if(index >= 0 && index < _hudStateInfoCount)
		{
			_hudStateInfos[index].panel.SetActive(active);
		}
	}

	public void ChangeHudState(HUDState newState)
	{
		SetPanelActive(_currentHudState,false);
		_currentHudState = newState;
		SetPanelActive(_currentHudState,true);
		OnHudStateChanged(newState);
    }

	private void OnHudStateChanged(HUDState newState)
	{
		switch (newState)
		{
			case HUDState.HS_GAME:
				ResetHudState();
				LivesController.ResetLifes();
				break;
			case HUDState.HS_PAUSE:
				{
					GameController gameControllerInstace = GameController.Instance;
					if(gameControllerInstace != null)
					{
						_pauseStats.UpdateScore(gameControllerInstace.Money,gameControllerInstace.CurrentShipCounterState,gameControllerInstace.MissionShipCounterState);
					}
				}
				break;
			case HUDState.HS_WIN:
				break;
			case HUDState.HS_LOST:
				break;
		}
	}

	public void UpdatePowerUpIcon(PowerUpType type, float progres)
	{
		int index = (int)type;
		if(index >= 0 && index  < _powerUpInfoCount)
		{
			bool active = progres > 0.0f;
            _powerUpInfos[index].image.gameObject.SetActive(active);
			if(active)
			{
				_powerUpInfos[index].image.fillAmount = progres;
			}
		}
	}
	public void UpdateActiveGUI(ActiveSkillsEnum type, float progres)
	{
		int index = (int)type;
		if(index >= 0 && index < _activeSkillsGUIInfoCount)
		{
			_activeSkillGUIInfos[index].UpdateActive(progres);
		}
	}

	public void ShowSkillScreen(ActiveSkillsEnum type)
	{
		ActiveButtons.SetActive(false);
		int index = (int)type;
		if(index  >= 0 && index < _activeSkillsGUIInfoCount)
		{
			_activeSkillScreen.gameObject.SetActive(true);
			_activeSkillScreen.color = _activeSkillGUIInfos[index].defaultBackgroundColor;
		}
	}
	public void HideActiveSkills()
	{
		ActiveButtons.SetActive(false);
		_activeSkillScreen.gameObject.SetActive(false);
	}

	#endregion Methods
}
