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
    [SerializeField]
    private GameObject SkillButton;

    #endregion General GUI

    #region Score GUI

    [SerializeField]
	private Text Score;

	[SerializeField]
	private LivesController _livesController = null;
	public LivesController LivesController { get { return _livesController; } }

	[SerializeField]
	private StatsController _statsController = null;

	#endregion Score GUI

	#region Active Skills GUI

	[System.Serializable]
	public struct ActiveSkillGUIInfo
	{
		public static Color disabledColor = new Color(0.1f, 0.1f, 0.1f, 40.0f / 255.0f);
		public static Color[] defaultColors = new Color[]
		{
			new Color(1f, 0f, 0f, 60f/255f),
			new Color(0f, 1f, 0f, 60f/255f),
			new Color(0f, 1f, 1f, 60f/255f),
			new Color(1f, 1f, 0f, 60f/255f)
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
		GameLord.Instance.NextLevel();
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

    public void SetActivePositionToIsland()
    {
		//resetowanie elementów, które są zależne od instancji w grze nie może się dziać w onEnable
		GameController instance = GameController.Instance;
		if (instance != null)
		{
			Vector2 screenPos = GameController.Instance.WorldToScreenPosition(GameController.Instance.IslandTransfrom.position);
			ActiveButtons.GetComponent<RectTransform>().anchoredPosition = screenPos;
			SkillButton.GetComponent<Image>().rectTransform.anchoredPosition = screenPos;
		}
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
        SetActivePositionToIsland();
	    //UpdateActiveGUI(ActiveSkillsEnum.Buoy, 1f);
        //UpdateActiveGUI(ActiveSkillsEnum.SecondLight, 1f);
        //UpdateActiveGUI(ActiveSkillsEnum.Flare, 1f);
        //UpdateActiveGUI(ActiveSkillsEnum.Freeze, 1f);
        for (int i = 0;i < _hudStateInfoCount;++i)
		{
			HUDState tmpState = (HUDState)i;
			SetPanelActive(tmpState, tmpState == _currentHudState);
		}
		HideActiveSkills();
		_statsController.Show(false);
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
				_statsController.Show(false);
				break;
			case HUDState.HS_PAUSE:
				UpdateShipStatsGUI();
				_statsController.Show(true);
				break;
			case HUDState.HS_WIN:
				UpdateShipStatsGUI();
				_statsController.Show(true);
				break;
			case HUDState.HS_LOST:
				UpdateShipStatsGUI();
				_statsController.Show(true);
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
	public void UpdateShipStatsGUI()
	{
		GameController gameControllerInstace = GameController.Instance;
		if (gameControllerInstace != null)
		{
			_statsController.UpdateScore(gameControllerInstace.Money, gameControllerInstace.WinController);
		}
	}

	#endregion Methods
}
