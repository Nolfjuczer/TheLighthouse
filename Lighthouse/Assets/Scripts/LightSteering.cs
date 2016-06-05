using System;
using UnityEngine;
using System.Collections;

public class LightSteering : MonoBehaviour
{
    private bool _targeted;
    private Vector3 _normalScale = new Vector3(0.4f,0.58f,1f);
    private Vector3 _largeScale = new Vector3(0.7f,0.58f,1f);
    private Vector3 _smallScale = new Vector3(0.1f, 0.58f, 1f);

    private bool _invertSteering;
    
	// Use this for initialization
	void OnEnable ()
	{
	    PowerUpController.Instance.DirectionSwapperBegin += InvertSteeringBegin;
        PowerUpController.Instance.DirectionSwapperEnd += InvertSteeringEnd;
        PowerUpController.Instance.LightEnlargerBegin += LightEnlargerBegin;
        PowerUpController.Instance.LightEnlargerEnd += LightEnlargerEnd;
    }

    public void LightEnlargerBegin()
    {
        transform.localScale = _largeScale;
    }

    public void LightEnlargerEnd()
    {
        transform.localScale = _normalScale;
    }

    public void InvertSteeringBegin()
    {
        _invertSteering = true;
    }

    public void InvertSteeringEnd()
    {
        _invertSteering = false;
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

                gameObject.transform.Rotate(Vector3.forward, _invertSteering ? -angle : angle);
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
