using UnityEngine;
using System.Collections;

public class ActiveButton : MonoBehaviour
{
    public ActiveSkillsEnum Active;

    private Color[] _colors = new Color[]
    {
        new Color(1f, 1f, 1f, 0f),
        new Color(1f, 0f, 0f, 40f/255f),
        new Color(0f, 1f, 0f, 40f/255f),
        new Color(0f, 1f, 1f, 40f/255f),
        new Color(1f, 1f, 0f, 40f/255f),
    };

    public void OnClick()
    {
		ActiveController.Instance.ActivateSkill(Active);
        //switch (Active)
        //{
        //    case ActiveSkillsEnum.Flare:
        //        if (!ActiveController.Instance.FlareAvailable) return;
        //        break;
        //    case ActiveSkillsEnum.Buoy:
        //        if (!ActiveController.Instance.BuoyAvailable) return;
        //        break;
        //    case ActiveSkillsEnum.Freeze:
        //        if (!ActiveController.Instance.FreezeAvailable) return;
        //        break;
        //    case ActiveSkillsEnum.SecondLight:
        //        if (!ActiveController.Instance.SecondAvailable) return;
        //        break;
        //}
        //ActiveController.Instance.CurrentActive = Active; 
        //GUIController.Instance.CurrentActive.color = _colors[(int) Active];
        //GUIController.Instance.OnActiveChosen();
    }
}
