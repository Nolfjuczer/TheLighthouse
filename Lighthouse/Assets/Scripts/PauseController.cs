using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject Pause;

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
}
