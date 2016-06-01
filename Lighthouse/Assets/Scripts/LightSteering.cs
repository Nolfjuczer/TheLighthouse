using System;
using UnityEngine;
using System.Collections;

public class LightSteering : MonoBehaviour
{
    private bool _targeted;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    InputGetter();
	}

    private void InputGetter()
    {
        if (InputManager.Instance.ThisFrameTouch)
        {
            if (_targeted && InputManager.Instance.PreviousFrameTouch)
            {
                Vector2 prev = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.PreviousTouchPosition);
                Vector2 cur = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.TouchPosition);

                float angle = Vector2.Angle(prev, cur);

                Vector3 cross = Vector3.Cross(prev,cur);
                angle = cross.z > 0 ? angle : 360f - angle ;

                gameObject.transform.Rotate(Vector3.forward, angle);
            }
            else
            {
                Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Light"))
                    {
                        _targeted = true;
                    }
                }
                else
                {
                    _targeted = false;
                }
            }
        }
    }
}
