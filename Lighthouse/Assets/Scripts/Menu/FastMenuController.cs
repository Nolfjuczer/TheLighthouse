using UnityEngine;
using System.Collections;

public class FastMenuController : MonoBehaviour
{
	#region Variables

	public enum FastMenuState : int
	{
		FMS_MAIN = 0,
		FMS_LEVEL_SELECT = 1,
		FMS_CREDITS = 2,

		FMS_COUNT
	}

	[System.Serializable]
	public struct FastMenuStateInfo
	{
		public FastMenuState state;
		public GameObject panel;
	}

	[SerializeField]
	private FastMenuStateInfo[] _fastMenuStateInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _fastMenuStateInfoCount = 0;

	private FastMenuState _currentFastMenuState = FastMenuState.FMS_MAIN;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		FastMenuStateInfo[] oldFastMenuStateInfos = _fastMenuStateInfos;
		int oldfastMenuStateInfoCount = oldFastMenuStateInfos != null ? oldFastMenuStateInfos.Length : 0;
		_fastMenuStateInfoCount = (int)FastMenuState.FMS_COUNT;
		if(oldfastMenuStateInfoCount != _fastMenuStateInfoCount)
		{
			_fastMenuStateInfos = new FastMenuStateInfo[_fastMenuStateInfoCount];
		}
		for(int i = 0;i < _fastMenuStateInfoCount;++i)
		{
			if(i < oldfastMenuStateInfoCount)
			{
				_fastMenuStateInfos[i] = oldFastMenuStateInfos[i];
			}
			_fastMenuStateInfos[i].state = (FastMenuState)i;
		}
	}

	void OnEnable()
	{
		for(int i = 0;i < _fastMenuStateInfoCount;++i)
		{
			FastMenuState tmpState = (FastMenuState)i;
			SetPanelActive(tmpState, tmpState == FastMenuState.FMS_MAIN);
		}
		_currentFastMenuState = FastMenuState.FMS_MAIN;
	}

	#endregion Monobehaviour Methods

	#region Methods

	public void ChangeMenuState(FastMenuState newState)
	{
		if (newState != _currentFastMenuState)
		{
			SetPanelActive(_currentFastMenuState, false);
			SetPanelActive(newState, true);
			_currentFastMenuState = newState;
		}
	}

	private void SetPanelActive(FastMenuState state, bool active)
	{
		int index = (int)state;
		if(index >= 0 && index < _fastMenuStateInfoCount)
		{
			_fastMenuStateInfos[index].panel.SetActive(active);
		}
	}

	public void ClickLevelSelect()
	{
		ChangeMenuState(FastMenuState.FMS_LEVEL_SELECT);
	}

	public void ClickMenuMain()
	{
		ChangeMenuState(FastMenuState.FMS_MAIN);
	}
	public void ClickMenuCredits()
	{
		ChangeMenuState(FastMenuState.FMS_CREDITS);
	}
	public void ClickQuitApp()
	{
		Application.Quit();
	}

	public void ClickPlay(int levelIndex)
	{
		string[] levels = GameLord.Instance.Levels;
		int levelCount = levels.Length;
		if (levelIndex >= 0 && levelIndex < levelCount)
		{
			GameLord.Instance.LoadScene(levels[levelIndex]);
		}
	}


	#endregion Methods

}
