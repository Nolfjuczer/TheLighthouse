using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager>
{
    public bool ThisFrameTouch, PreviousFrameTouch;
    public Vector2 TouchPosition, PreviousTouchPosition;

	private bool _inputUILock = false;

	// Update is called once per frame
	void Update ()
    {
		UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;

#if UNITY_ANDROID && !UNITY_EDITOR
	    PreviousFrameTouch = ThisFrameTouch;
	    PreviousTouchPosition = TouchPosition;
	    if (Input.touchCount > 0)
	    {
			if( !eventSystem.IsPointerOverGameObject())
			{
				if(!_inputUILock)
				{
					TouchPosition = Input.touches[0].position;
					ThisFrameTouch = true;
				}
			}else{
				_inputUILock = true;
			}
	    }
	    else
	    {
	        ThisFrameTouch = false;
			_inputUILock = false;
	    }
#else
		PreviousFrameTouch = ThisFrameTouch;
        PreviousTouchPosition = TouchPosition;
	    if (Input.GetMouseButton(0))
	    {
			if (!eventSystem.IsPointerOverGameObject())
			{
				if (!_inputUILock)
				{
					TouchPosition = Input.mousePosition;
					ThisFrameTouch = true;
				}
			} else {
				_inputUILock = true;
			}
        }
        else
        {
			_inputUILock = false;
			ThisFrameTouch = false;
        }
#endif
    }

    public bool ReturnButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
