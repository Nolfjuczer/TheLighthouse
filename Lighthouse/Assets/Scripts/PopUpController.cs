using UnityEngine;
using System.Collections;

public class PopUpController : Singleton<PopUpController>
{
    public int[] PopUpsToShow;
    private int _counter;
    private int _currentIndex;

    private Coroutine _delayCoroutine;

    public void Start()
    {
        if (PopUpsToShow.Length > 0)
        {
            _delayCoroutine = StartCoroutine(DelayCoroutine());
        }
        else
        {
            GameController.Instance.GameState = EGameState.InGame;
            enabled = false;
        }
    }

    public IEnumerator DelayCoroutine()
    {
        while (GUIController.Instance == null) yield return null;
        _counter = -1;
        NextPopUp();
        while (GameController.Instance.GameState != EGameState.InGame) yield return null;
        GameController.Instance.GameState = EGameState.Paused;
    }

    public void NextPopUp()
    {
        if (PopUpsToShow.Length > _counter + 1)
        {
            if (_counter > -1)
            {
                GUIController.Instance.TutorialPopUps[_currentIndex].SetActive(false);                      
            }
            ++_counter;
            _currentIndex = PopUpsToShow[_counter];
            GUIController.Instance.TutorialPopUps[_currentIndex].SetActive(true);
        }
        else
        {
            StopCoroutine(_delayCoroutine);
            GameController.Instance.GameState = EGameState.InGame;
            GUIController.Instance.TutorialPopUps[_currentIndex].SetActive(false);
            enabled = false;
        }
    }
}
