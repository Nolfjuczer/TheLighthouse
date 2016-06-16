using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Sapper : MonoBehaviour {

    private Vector3 _selectedPosition;
    private float _disarmTimer;
    private Image _circleImage;

    private Mine _currentMine;

    public void OnEnable()
    {
        //_circleImage = GameController.Instance.GetProgressCricle(transform.position);
        //_circleImage.enabled = false;
        //_circleImage.color = Color.white;
    }

    public void Update()
    {
        MineInput();
    }


    public void MineInput()
    {
        if (InputManager.Instance.ThisFrameTouch && !GameController.Instance.Light.Targeted)
        {
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            Mine mine = null;
            if (hit.collider != null && /*_currentMine == null &&*/ hit.collider.gameObject.layer == LayerMask.NameToLayer("Mine") )
            {
                mine = hit.collider.GetComponent<Mine>();
                if (mine != null)
                {
                    if (_selectedPosition == Vector3.zero)
                    {
                        _currentMine = mine;
                        _selectedPosition = ray.origin;
						_circleImage = GameController.Instance.GetProgressCricle(mine.transform.position);
                        _circleImage.enabled = true;
                    }                    
                }
            }

            if (_currentMine != null)
            {
                _disarmTimer += Time.deltaTime;
                _circleImage.fillAmount = _disarmTimer;
                if (_disarmTimer >= 1f)
                {
                    _currentMine.DisarmMine();
                    _currentMine = null;
                    _selectedPosition = Vector3.zero;
                    _circleImage.enabled = false;
					_circleImage = null;
                }                
            }


        }
        else
        {
            _selectedPosition = Vector3.zero;
            _disarmTimer = 0f;
			if(_currentMine != null)
			{
				_circleImage.enabled = false;
				_circleImage = null;
			}
            _currentMine = null;
        }
    }
}
