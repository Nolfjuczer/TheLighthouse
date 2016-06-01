using UnityEngine;
using System.Collections;

public class InputManager : Singleton<InputManager>
{
    public bool ThisFrameTouch, PreviousFrameTouch;
    public Vector2 TouchPosition, PreviousTouchPosition;
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	    PreviousFrameTouch = ThisFrameTouch;
	    PreviousTouchPosition = TouchPosition;
	    if (Input.touchCount > 0)
	    {
	        TouchPosition = Input.touches[0].position;
	        ThisFrameTouch = true;
	    }
	    else
	    {
	        ThisFrameTouch = false;
	    }
#else
        PreviousFrameTouch = ThisFrameTouch;
        PreviousTouchPosition = TouchPosition;
	    if (Input.GetMouseButton(0))
	    {
            TouchPosition = Input.mousePosition;
            ThisFrameTouch = true;
        }
        else
        {
            ThisFrameTouch = false;
        }
#endif
    }

    public bool ReturnButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
