using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : Singleton<GUIController>
{
	#region Variables

	[SerializeField]
	private Text Score;
	[SerializeField]
	private GameObject Pause;
	public Image CurrentActive;
	[SerializeField]
	private GameObject ActiveButtons;
	public Image[] ActiveIcons;
	public Image[] PowerUps;
    public Image[] Lives;
	[SerializeField]
	private bool _activeExpanded;

    public EndGameGUI WinStats;
    public EndGameGUI LoseStats;

	public enum HUDState : int
	{
		HS_GAME = 0,
		HS_PAUSE = 1,
		HS_WIN = 2,
		HS_LOST = 3,

		HS_COUNT
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

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateGUIController();
	}

	void OnEnable()
	{
		///*i*/f(Pause.activeSelf)
		////{
		//	//Pause.SetActive(false);
		////}
		ResetHudState();
	    for (int i = 0; i < Lives.Length; ++i) Lives[i].enabled = true;
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
        CurrentActive.gameObject.SetActive(true);
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
	}

	#endregion Methods
}
