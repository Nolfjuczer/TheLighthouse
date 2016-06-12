using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUIController : Singleton<GUIController>
{
    public GameObject Pause;
    public GameObject ActiveButtons;
    private bool _activeExpanded;

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
        _activeExpanded = false;
        ActiveButtons.SetActive(false);
    }
}
