using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : Singleton<GUIController>
{
	#region Variables

	public Text Score;
    public GameObject Pause;
    public Image CurrentActive;
    public GameObject ActiveButtons;
    public Image[] ActiveIcons;
    public Image[] PowerUps;
    private bool _activeExpanded;

	#endregion Variables

	#region Monobehaviour Methods

	void OnEnable()
	{
		if(Pause.activeSelf)
		{
			Pause.SetActive(false);
		}
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
        Pause.SetActive(true);
    }

    public void OnResumeClick()
    {
        GameController.Instance.GameState = EGameState.InGame;
        Pause.SetActive(false);
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

	#endregion Methods
}
