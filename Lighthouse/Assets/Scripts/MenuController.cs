using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject GameOver;
    public Text Score;
    public Text BestScore;
    public GameObject Credits;

    public void OnEnable()
    {
        //TODO Impelemt getting score from prefs
    }

    public void Update()
    {
        if (InputManager.Instance.ReturnButton())
        {
            Application.Quit();
        }
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCreditsClick()
    {
        MainMenu.SetActive(false);
        GameOver.SetActive(false);
        Credits.SetActive(true);
    }
    

    public void OnBackClick()
    {
        MainMenu.SetActive(true);
        GameOver.SetActive(false);
        Credits.SetActive(false);
    }
}
