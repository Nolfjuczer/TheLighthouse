using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Sapper : MonoBehaviour
{

    private Mine _currentMine;

    public void Update()
    {
        MineInput();
    }

    public void MineInput()
    {
        if (InputManager.Instance.ThisFrameTouch && !InputManager.Instance.PreviousFrameTouch && !GameController.Instance.Light.Targeted)
        {
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            _currentMine = null;
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Mine") )
            {
                _currentMine = hit.collider.GetComponent<Mine>();
                if (_currentMine != null)
                {
                    ++_currentMine.Touches;
                    if(_currentMine.Touches >= 3)
                        _currentMine.DisarmMine();
                }
            }
        }
        else
        {
            _currentMine = null;
        }
    }
}
