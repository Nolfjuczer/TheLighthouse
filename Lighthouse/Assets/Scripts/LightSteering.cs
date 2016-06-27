using System;
using UnityEngine;
using System.Collections;

public class LightSteering : MonoBehaviour
{
	#region Variables

	private Transform _transform = null;

	public GameObject SecondLight;
    private bool _targeted;
    public bool Targeted
    {
        get { return _targeted; }
    }
    private bool _activeOn;
    public bool ActiveOn
    {
        get { return _activeOn; }
        set { _activeOn = value; }
    }

    private Vector3 _largeScale = new Vector3(0.7f,0.58f,1f);
    private Vector3 _normalScale = new Vector3(0.4f,0.58f,1f);
    private Vector3 _smallScale = new Vector3(0.1f, 0.58f, 1f);

	private bool _lightEnlarge = false;
	private bool _lightShrink = false;

    private bool _invertSteering;

	private int _shipLayer = 0;

	private float _maxRotateSpeed = 540.0f;

	[SerializeField]
	private Vector2 _debugPosition = Vector2.zero;
	[SerializeField]
	private float _debugAngle = 0.0f;
	[SerializeField]
	private Vector2 _debugSize = Vector2.one;

	#endregion Variables

	#region Monobehaviour Methods

	void Awake()
	{
		InitializeLightSteering();
	}
	
	void OnDestroy()
	{
		DeInitializeLightSteering();
	}

	void OnEnable ()
	{
		PowerUpController.Instance.PowerUpInfos[(int)PowerUpType.PLightEnlarger].OnPowerUpStateChange += OnLightEnlargeStateChanged;
		PowerUpController.Instance.PowerUpInfos[(int)PowerUpType.NDirectionSwapper].OnPowerUpStateChange += OnDirectionSwap;

		ActiveController.Instance.ActiveInfos[(int)ActiveSkillsEnum.SecondLight].OnActiveSkillUsed += OnSecondLight;

		//PowerUpController.Instance.DirectionSwapperBegin += InvertSteeringBegin;
        //PowerUpController.Instance.DirectionSwapperEnd += InvertSteeringEnd;
        //PowerUpController.Instance.LightEnlargerBegin += LightEnlargerBegin;
        //PowerUpController.Instance.LightEnlargerEnd += LightEnlargerEnd;
	    //ActiveController.Instance.OnSecondLight += OnSecondLight;
	}
	void OnDisable()
	{
		//PowerUpController.Instance.PowerUpInfos[(int)PowerUpType.PLightEnlarger].OnPowerUpStateChange -= OnLightEnlargeStateChanged;
		//PowerUpController.Instance.PowerUpInfos[(int)PowerUpType.NDirectionSwapper].OnPowerUpStateChange -= OnDirectionSwap;
		//
		//ActiveController.Instance.ActiveInfos[(int)ActiveSkillsEnum.SecondLight].OnActiveSkillUsed -= OnSecondLight;

		//PowerUpController.Instance.DirectionSwapperBegin -= InvertSteeringBegin;
		//PowerUpController.Instance.DirectionSwapperEnd -= InvertSteeringEnd;
		//PowerUpController.Instance.LightEnlargerBegin -= LightEnlargerBegin;
		//PowerUpController.Instance.LightEnlargerEnd -= LightEnlargerEnd;
		//ActiveController.Instance.OnSecondLight -= OnSecondLight;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.layer == _shipLayer)
		{
			Ship tmpShip = other.gameObject.GetComponent<Ship>();
			if(tmpShip != null)
			{
				float deltaCapture = PowerUpController.Instance.CaptureTimeScale * Time.deltaTime;
				tmpShip.NotifyCapture(deltaCapture);
			}
		}
	}

	void Update()
	{
		//InputGetter();
		LightControll();
		TestDraw();
    }

	#endregion Monobehaviour Methods

	#region Methods

	private void InitializeLightSteering()
	{
		_shipLayer = LayerMask.NameToLayer("Ship");
		_transform = this.GetComponent<Transform>();
	}

	private void DeInitializeLightSteering()
	{
		
	}

	public void OnSecondLight()
    {
        SecondLight.SetActive(true);
        StartCoroutine(SecondLightCoroutine());
    }

    protected IEnumerator SecondLightCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SecondLight.SetActive(false);
    }

    //public void LightEnlargerBegin()
    //{
    //    transform.localScale = _largeScale;
    //}
	//
    //public void LightEnlargerEnd()
    //{
    //    transform.localScale = _normalScale;
    //}
	//
    //public void InvertSteeringBegin()
    //{
    //    _invertSteering = true;
    //}
	//
    //public void InvertSteeringEnd()
    //{
    //    _invertSteering = false;
    //}

    //private void InputGetter()
    //{
    //    if(_activeOn) return;
    //    if (InputManager.Instance.ThisFrameTouch)
    //    {
    //        if (_targeted && InputManager.Instance.PreviousFrameTouch)
    //        {
    //            Vector2 prev = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.PreviousTouchPosition);
    //            Vector2 cur = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.TouchPosition);
	//
    //            float angle = Vector2.Angle(prev, cur);
	//
    //            Vector3 cross = Vector3.Cross(prev,cur);
    //            angle = cross.z > 0 ? angle : 360f - angle ;
	//
    //            gameObject.transform.Rotate(Vector3.forward, _invertSteering ? -angle : angle);
    //        }
    //        else
    //        {
    //            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
    //            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
    //            if (hit.collider != null)
    //            {
    //                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Light"))
    //                {
    //                    _targeted = true;
    //                }
    //            }
    //            else
    //            {
    //                _targeted = false;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        _targeted = false;
    //    }
    //}

	private void LightControll()
	{
		if(InputManager.Instance.ThisFrameTouch && !ActiveController.Instance.AnySkillActive)
		{
			Vector3 worldTouchPoint = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.TouchPosition);
			Vector3 lighthousePosition = _transform.position;
			Vector3 boom = worldTouchPoint - lighthousePosition;
			Vector3 direction = boom.normalized;
			if(_invertSteering)
			{
				direction = -direction;
			}
			Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

			Quaternion lightRotation = _transform.localRotation;
			lightRotation = Quaternion.RotateTowards(lightRotation, targetRotation, _maxRotateSpeed * Time.deltaTime);
			_transform.localRotation = lightRotation;
		}
	}

	private void UpdateLightSize()
	{
		if( (!_lightShrink && !_lightEnlarge) || (_lightShrink && _lightEnlarge) )
		{
			transform.localScale = _normalScale;
		} else {
			if(_lightEnlarge)
			{
				transform.localScale = _largeScale;
			}
			if(_lightShrink)
			{
				transform.localScale = _smallScale;
			}
		}
	}
	
	private void OnLightEnlargeStateChanged(bool active)
	{
		_lightEnlarge = active;
		UpdateLightSize();
    }
	private void OnDirectionSwap(bool active)
	{
		_invertSteering = active;
	}

	public void TestDraw()
	{
		DebugTools.DrawDebugBox(_debugPosition, _debugAngle, _debugSize);
	}

	#endregion Methods
}
