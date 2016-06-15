using UnityEngine;
using System.Collections;
using System.Linq;

namespace Multiscene
{
	public class GameLord : MonoBehaviour
	{
		#region Variables

		private static GameLord _instance = null;
		public static GameLord Instance
		{
			get
			{
				if (_instance == null)
				{

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

		[SerializeField]
		private string[] _levels = null;
		[SerializeField]
		[HideInInspector]
		private int _levelCount = 0;

		#endregion Variables

		#region Monobehaviour Methods

		void OnValidate()
		{

		}

		void Awake()
		{
			InitializeGameLord();
		}

		void Start()
		{

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

		private void LoadScene(string sceneName)
		{
			if (_levels.Contains(sceneName))
			{
				StartCoroutine(LoadSceneCoroutine(sceneName));
			}
		}

		private IEnumerator LoadSceneCoroutine(string sceneName)
		{
			GUILord instance = GUILord.Instance;
			instance.ChangeGUIState(GUILord.GUIState.GUIS_LOADING);
			while(instance.IsTransition)
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
			instance.ChangeGUIState(GUILord.GUIState.GUIS_GAME);
			yield return null;
		} 
		
		#endregion Methods
	}
}