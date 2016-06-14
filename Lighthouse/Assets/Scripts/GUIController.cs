using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : Singleton<GUIController>
{
    public Text Score;
    public GameObject Pause;
    public Image CurrentActive;
    public GameObject ActiveButtons;
    public Image[] ActiveIcons;
    public Image[] PowerUps;
    private bool _activeExpanded;

    public void Update()
    {
        Score.text = "$ " + GameController.Instance.Money;
    }

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
        SceneManager.LoadScene(1);
    }

    public void OnMenuClick()
    {
        GameController.Instance.GameState = EGameState.InGame;
        SceneManager.LoadScene(0);
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
}
