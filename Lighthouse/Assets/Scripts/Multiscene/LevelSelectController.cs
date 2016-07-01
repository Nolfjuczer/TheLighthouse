using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectController : MonoBehaviour
{
	#region Variables

	public struct LevelGUIInfo
	{
		public Button button;
	}

	[SerializeField]
	private LevelGUIInfo[] _levelGUIInfos = null;

	[HideInInspector]
	[SerializeField]
	private int _levelGUIInfoCount = 0;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateLevelSelectController();
    }

	#endregion Monobehaviour Methods

	#region Methods

	void ValidateLevelSelectController()
	{
		_levelGUIInfoCount = _levelGUIInfos != null ? _levelGUIInfos.Length : 0;
	}

	#endregion Methods
}
