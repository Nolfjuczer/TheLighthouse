using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameGUI : MonoBehaviour
{
    public Text TotalScore;

    public Text FerryScore;
    public Text FreighterScore;
    public Text KeelboatScore;
    public Text MotorboatScore;

	public void UpdateScore(int cash ,ShipCounterType currentScore, ShipCounterType targetScore)
	{
		TotalScore.text = string.Format("Cash {0}$", cash.ToString());
		FerryScore.text = string.Format("{0} / {1}", currentScore.FerryCount, targetScore.FerryCount);
		FreighterScore.text = string.Format("{0} / {1}", currentScore.FreighterCount, targetScore.FreighterCount);
		KeelboatScore.text = string.Format("{0} / {1}", currentScore.KeelboatCount, targetScore.KeelboatCount);
		MotorboatScore.text = string.Format("{0} / {1}", currentScore.MotorboatCount, targetScore.MotorboatCount);
	}
}
