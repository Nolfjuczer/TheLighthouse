using UnityEngine;
using System.Collections;

namespace Multiscene
{
	public class GameLord : MonoBehaviour
	{
		#region Variables

		public enum GameState
		{
			GS_MENU = 0,
			GS_LOADING = 1,
			GS_GAME = 2
		}

		private GameState _currentGameState = GameState.GS_MENU;
		public GameState CurrentGameState { get { return _currentGameState; } }

		#endregion Variables

		#region Monobehaviour Methods
		void Start()
		{

		}

		void Update()
		{

		}
		#endregion Monobehaviour Methods

		#region Methods

		#endregion Methods
	}
}