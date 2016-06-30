using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
	#region Variables

	[System.Serializable]
	public struct ShipTypeGUIInfo
	{
		public ShipTypeEnum shipType;
		public RectTransform rectTransform;
		public Text scoreText;

		public void UpdateScore(WinController.WinCondition winCondition, int current, int target)
		{
			switch (winCondition)
			{
				case WinController.WinCondition.WC_SHIPS_RESCUED:
					scoreText.text = string.Format("{0} / {1}", current, target);
					break;
				case WinController.WinCondition.WC_TIME_ELAPSED:
					scoreText.text = string.Format("{0}", current);
					break;
			}
			
		}
	}

	[SerializeField]
	private Text _totalScore;

	[SerializeField]
	private ShipTypeGUIInfo[] _shipTypeGUIInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _shipTypeGUIInfoCount = 0;


	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ShipTypeGUIInfo[] oldShipTypeGUIInfos = _shipTypeGUIInfos;
		int oldShipTypeGUICount = oldShipTypeGUIInfos != null ? oldShipTypeGUIInfos.Length : 0;
		_shipTypeGUIInfoCount = (int)ShipTypeEnum.COUNT;
		if(oldShipTypeGUICount != _shipTypeGUIInfoCount)
		{
			_shipTypeGUIInfos = new ShipTypeGUIInfo[_shipTypeGUIInfoCount];
		}
		for(int i = 0;i < _shipTypeGUIInfoCount;++i)
		{
			if(i < oldShipTypeGUICount)
			{
				_shipTypeGUIInfos[i] = oldShipTypeGUIInfos[i];
			}
			_shipTypeGUIInfos[i].shipType = (ShipTypeEnum)i;
		}
	}

	#endregion Monobehaviour Methods

	#region Methods

	public void UpdateScore(int cash ,WinController shipCounter)
	{
		Debug.LogFormat("Update score stats");

		_totalScore.text = string.Format("Cash ${0}", cash.ToString());
		//FerryScore.text = string.Format("{0} / {1}", currentScore.FerryCount, targetScore.FerryCount);
		//FreighterScore.text = string.Format("{0} / {1}", currentScore.FreighterCount, targetScore.FreighterCount);
		//KeelboatScore.text = string.Format("{0} / {1}", currentScore.KeelboatCount, targetScore.KeelboatCount);
		//MotorboatScore.text = string.Format("{0} / {1}", currentScore.MotorboatCount, targetScore.MotorboatCount);

		float currentHeight = 0.0f;

		for(int i = 0;i < shipCounter.ShipCounterCount;++i)
		{
			if(shipCounter.ShipTypeCounters[i].goal > 0)
			{
				_shipTypeGUIInfos[i].rectTransform.gameObject.SetActive(true);
				_shipTypeGUIInfos[i].rectTransform.anchoredPosition = new Vector2(0.0f,currentHeight);
				currentHeight -= _shipTypeGUIInfos[i].rectTransform.sizeDelta.y;

				_shipTypeGUIInfos[i].UpdateScore(shipCounter.CurrentWinCondition, shipCounter.ShipTypeCounters[i].counter, shipCounter.ShipTypeCounters[i].goal);
            } else {
				_shipTypeGUIInfos[i].rectTransform.gameObject.SetActive(false);
			}
		}
	}

	public void Show(bool visible)
	{
		this.gameObject.SetActive(visible);
	}

	#endregion Methods
}
