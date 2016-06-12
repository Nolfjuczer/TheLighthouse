using UnityEngine;
using System.Collections;

public class ActiveButton : MonoBehaviour
{
    public ActiveSkillsEnum Active;

    public void OnClick()
    {
        ActiveController.Instance.CurrentActive = Active;
        GUIController.Instance.OnActiveChosen();
    }
}
