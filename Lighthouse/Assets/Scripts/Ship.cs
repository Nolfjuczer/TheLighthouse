using System;
using UnityEngine;
using System.Collections;
using AStar;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;

public enum ShipTypeEnum : int
{
    Ferry = 0,
    Freighter,
    Keelboat,
    Motorboat,

	COUNT,
	NONE
}

public class Ship : WandererBehavior
{
	#region Variables

	public enum ShipState
	{
		SS_WANDER = 0,
		SS_CAPTURED = 1,
		SS_ARRIVED = 2,
		SS_DEAD = 3,

		SS_COUNT,
		SS_NONE
	}

	private ShipState _lastShipState = ShipState.SS_NONE;
	private ShipState _currentShipState = ShipState.SS_NONE;
	public ShipState CurrentShipState {  get { return _currentShipState; } }

	private float _captureCoolingTimer = 0.0f;
	private float _captureCoolingLength = 0.25f;
	private float _captureProgres = 0.0f;
	private float _captureToCoolTimeRatio = 0.5f;

	public ShipTypeEnum ShipType;
    [Range(1,4)]
    public int ShipManoeuvrability = 1;
    [Range(1, 4)]
    public int ShipSpeed = 1;

    public SpriteRenderer Outline;
    public Sprite[] Sprites;

    public float CaptureTime = 2f;
    private float _captureTime;

	private int _layer_obstacle = 0;
	private int _layer_ship = 0;
	private int _layer_mine = 0;
	private int _layerMask = 0;

	private Alert _alert = null;

    public bool Captured
    {
        get{ return _currentShipState == ShipState.SS_CAPTURED;}
		private set
		{
			_captured = value;
			if (Outline != null)
			{
				Outline.enabled = _captured;
			}
		}
    }

    private SpriteRenderer _renderer;
    private Image _circleImage;
    private float _speedMultiplier = 0.2f;
    private float _captureTimer;
    private bool _captured;
    private Vector2 _wanderDestination;
    private TrailRenderer _trailRenderer;
    private ParticleSystem _particleSystem;
    private AStarAgent _myAgent;
    private WaterGridElement _currentElement;
    private BoxCollider2D _boxCollider;
    private bool _isSuper;
    private bool _frozen;

    private int _speed;
    private Vector3 _baseScale;

    private bool _died;
    private bool _gotToPort;

    private bool _fastAvoidance;
    private bool _enlighted;

	private bool _eventsSet = false;

	private float _wanderTimer = 0.0f;
	private float _wanderChangeInterval = 3.0f;

	[SerializeField]
	private BoxCollider2D _collider;
	[SerializeField]
	private Transform _transform = null;

	//private float _checkDistance = 1.5f;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		_transform = this.GetComponent<Transform>();
		_collider = this.GetComponent<BoxCollider2D>();
	}

	public override void Awake()
    {
        base.Awake();
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogWarning("There's no renderer attached");
            gameObject.SetActive(false);
            return;
        }

        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider == null)
        {
            Debug.LogWarning("There's no collider attached");
            gameObject.SetActive(false);
            return;
        }

        _trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
        if (_trailRenderer == null)
        {
            Debug.LogWarning("There's no trail renderer attached");
            gameObject.SetActive(false);
            return;
        }

        _trailRenderer.sortingLayerID = _renderer.sortingLayerID;
        _trailRenderer.sortingOrder = _renderer.sortingOrder - 1;

        _particleSystem = transform.GetComponentInChildren<ParticleSystem>();
        if (_particleSystem == null)
        {
            Debug.LogWarning("There's no particle system attached");
            gameObject.SetActive(false);
            return;
        }

        _particleSystem.GetComponent<Renderer>().sortingLayerID = _renderer.sortingLayerID;
        _particleSystem.GetComponent<Renderer>().sortingOrder = _renderer.sortingOrder + 1;

        _myAgent = GetComponent<AStarAgent>();
        if (_myAgent == null)
        {
            Debug.LogWarning("There's no agent attached");
            gameObject.SetActive(false);
            return;
        }

        _baseScale = gameObject.transform.localScale;
        _speed = ShipManoeuvrability;
        _captureTime = CaptureTime;

        GameLord.Instance.OnReloadStage += OnReloadStage;

		InitializeShip();
    }

    private void OnDestroy()
    {
        GameLord.Instance.OnReloadStage -= OnReloadStage;
    }

	public override void OnEnable()
	{
		base.OnEnable();

        _myAgent.MyGrid = GameController.Instance.MainGrid;
        _myAgent.TargetObject = GameController.Instance.IslandTransfrom;

        gameObject.transform.localScale = _baseScale;
		_speed = ShipSpeed;

		WanderDistance /= ShipManoeuvrability;
		WanderRadius *= ShipManoeuvrability;

		_captureTimer = 0f;
		Captured = false;

		_trailRenderer.Clear();
		_trailRenderer.enabled = true;

		_captureProgres = 0.0f;

		GameObject circleGO = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_CIRCLE);
		if (circleGO == null)
		{
			Debug.LogFormat("Here {0}", this.gameObject.name);
		}
		circleGO.SetActive(true);
		_circleImage = circleGO.GetComponent<Image>();
		//_circleImage = GameController.Instance.GetProgressCricle(transform.position);
		_circleImage.enabled = false;
		_circleImage.color = Color.white;

		_isSuper = Random.Range(0f, 1f) > 0.7f;
		_renderer.sprite = _isSuper ? Sprites[1] : Sprites[0];

		_renderer.enabled = true;

		_died = false;
		_gotToPort = false;
		_fastAvoidance = false;
		_enlighted = false;

		_boxCollider.enabled = true;

		SetEvents(true);

		_wanderDestination = GameController.Instance.IslandTransfrom.position - gameObject.transform.position;
		_wanderTimer = 0.0f;

		ChangeShipState(ShipState.SS_WANDER);
		//StartCoroutine(WandererCoroutine());
	}

	void OnDisable()
	{
		if(_circleImage != null)
		{
			_circleImage.gameObject.SetActive(false);
			_circleImage = null;
		}
		SetEvents(false);
	}

	public void OnTriggerEnter2D(Collider2D col2D)
	{
		if (!_renderer.isVisible) return;
		if (GameController.Instance.GameState != EGameState.InGame) return;

		//if ((col2D.gameObject.layer == LayerMask.NameToLayer("Light") || col2D.gameObject.layer == LayerMask.NameToLayer("Flare")) && _captureTimer < CaptureTime && !_gotToPort)
		//{
		//	_enlighted = true;
		//	StopCoroutine("UncaptureByLightHouse");
		//	StartCoroutine("CaptureByLightHouse");
		//	return;
		//}

		if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship") && !Captured)
		{
			DestoryOnIsland();
			return;
		}

		if (col2D.gameObject.layer == LayerMask.NameToLayer("Obstacle") && !Captured)
		{
			if (col2D.gameObject.GetComponent<Obstacle>().ObstacleType == ObstacleTypeEnum.Island)
			{
				DestoryOnIsland();
			}
			else
			{
				DestoryOnWhirlpool(col2D.gameObject.transform.position);
			}
			return;
		}

		if (col2D.gameObject.layer == LayerMask.NameToLayer("Mine"))
		{
			DestoryOnIsland();
			return;
		}
	}

	public void OnTriggerExit2D(Collider2D col2D)
	{
		if (!_renderer.isVisible) return;
		if (GameController.Instance.GameState != EGameState.InGame) return;

		//if ((col2D.gameObject.layer == LayerMask.NameToLayer("Light") || col2D.gameObject.layer == LayerMask.NameToLayer("Flare")) && !_gotToPort)
		//{
		//	_enlighted = false;
		//	if (Captured) return;
		//	StopCoroutine("CaptureByLightHouse");
		//	StartCoroutine("UncaptureByLightHouse");
		//}
	}

	public void Update()
	{
		//ShipMovement();
		ProcesShipState();
	}

	#endregion Monobehaviour Methods

	#region Old Methods

	private void OnReloadStage()
	{
		gameObject.SetActive(false);
	}


	public IEnumerator WandererCoroutine()
    {
        //targetIsland
        _wanderDestination = GameController.Instance.IslandTransfrom.position - gameObject.transform.position;
        yield return new WaitForSeconds(1f * ShipSpeed);

        //Do what youre meant to xD
        while (true)
        {
            _wanderDestination = Wander();
            yield return new WaitForSeconds(3f);
        }
    }

    protected void FreezeShip()
    {
        _frozen = true;
        StartCoroutine(FreezeCoroutine());
    }

    protected IEnumerator FreezeCoroutine()
    {
        yield return new WaitForSeconds(4f);
        _frozen = false;
    }


    public void GetToPort()
    {
        _gotToPort = true;
		ChangeShipState(ShipState.SS_ARRIVED);
        StartCoroutine(ArriveToLand());
    }

    public void DestoryOnIsland()
    {
		ChangeShipState(ShipState.SS_DEAD);
		//if(_captureTimer > 0f) StopCoroutine("CaptureByLightHouse");
		_trailRenderer.enabled = false;
        _boxCollider.enabled = false;

        _particleSystem.Play();
        StartCoroutine(WaitForExplosion());
        _circleImage.enabled = false;

		CleanShipEffects();

		GameController.Instance.ShipDestroyed(true,_transform.position);
    }

    public void DestoryOnWhirlpool(Vector3 whirlpoolPosition)
    {
		ChangeShipState(ShipState.SS_DEAD);
		//if (_captureTimer > 0f) StopCoroutine("CaptureByLightHouse");
        _trailRenderer.enabled = false;
        _boxCollider.enabled = false;

        StartCoroutine(WaitForSpin(whirlpoolPosition));
        _circleImage.enabled = false;

		CleanShipEffects();

		GameController.Instance.ShipDestroyed(true, _transform.position);
    }

    private IEnumerator WaitForSpin(Vector3 whirlpoolPosition)
    {
        Vector3 startPos = transform.position;
        bool expolsion = true;
        float landTimer = 3f;
        while (landTimer > 0f)
        {
            if (landTimer < 1.5f)
            {
                if (expolsion)
                {
                    expolsion = false;
                    _died = true;
                    _particleSystem.Play();
                }
                if (landTimer < 0.5f)
                {
                    _renderer.enabled = false;
                    transform.localScale = _baseScale;
                }
                StartCoroutine(WaitForExplosion());
            }
			float deltaTime = 0.0f;
			if (GameController.Instance.GameState == EGameState.PostGame)
			{
				deltaTime = Time.unscaledDeltaTime;
			} else {
				deltaTime = Time.deltaTime;
			}

            transform.position = Vector3.Lerp(whirlpoolPosition, startPos, Mathf.Clamp01(landTimer / 2f - 1f));
            transform.localScale = Vector3.Lerp(Vector3.zero, _baseScale, landTimer / 3f);
            transform.Rotate(Vector3.forward, 250f * deltaTime);
			landTimer -= deltaTime;			
			landTimer = Mathf.Clamp(landTimer, 0f, float.MaxValue);
            yield return null;
        }

    }

    private IEnumerator WaitForExplosion()
    {
        float landTimer = _particleSystem.startLifetime;
        while (landTimer > 0f)
        {
            if (landTimer < 2.7f)
            {
                _renderer.enabled = false;
                _died = true;
            }
			float deltaTime = 0.0f;
			if (GameController.Instance.GameState == EGameState.PostGame)
			{
				deltaTime = Time.unscaledDeltaTime;
				_particleSystem.Simulate(deltaTime, true, false);
			} else {
				deltaTime = Time.deltaTime;
			}
			landTimer -= deltaTime;

			landTimer = Mathf.Clamp(landTimer, 0f, float.MaxValue);
            yield return null;
        }
        _particleSystem.Stop();

		CleanShipEffects();

		CleanShip();
    }

    private IEnumerator ArriveToLand()
    {
        _circleImage.enabled = false;
        PathVisibility();
        _trailRenderer.enabled = false;
        float landTimer = 2f;
        while (landTimer > 0f)
        {
            landTimer -= Time.deltaTime;
            landTimer = Mathf.Clamp(landTimer,0f,3f);
            gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 0.05f, _baseScale, landTimer / 2f);
            yield return null;
        }

		if (GameController.Instance.GameState == EGameState.InGame)
		{
			GameController.Instance.ShipCollected(ShipType);
		}

        CleanShip();
    }

    private void PathVisibility(bool flag = false)
    {
        if (_currentElement != null) _currentElement.UsePoint(flag, this);
        foreach (GridElement element in _myAgent.Path)
        {
            WaterGridElement waterElement = element as WaterGridElement;
            if (waterElement != null) waterElement.UsePoint(flag, this);
        }
    }

    public void CleanShip()
    {
        PathVisibility();

        transform.localScale = Vector3.one;
        _circleImage.gameObject.SetActive(false);
        _circleImage = null;
        gameObject.SetActive(false);

		CleanShipEffects();

		SetEvents(false);
	}

    protected void CaptureBoostBegin()
    {
        _captureTime = CaptureTime - 1f;
    }

    protected void CaptureSlowerBegin()
    {
        _captureTime = CaptureTime + 1f;
    }

    protected void CapturePowerUpEnd()
    {
        _captureTime = CaptureTime;
    }

    private void ShipMovement()
    {
        if (_died || _frozen) return;
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.up.normalized * 2f, Color.red);
#endif
        bool breaking = ShipBreak();
        //if(!_fastAvoidance)
        //    ObstacleAvoidance();

        if (Captured)
        {
            GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position);
            if (_currentElement != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(Vector3.forward, _currentElement.transform.position - gameObject.transform.position),
                    Time.deltaTime * ShipManoeuvrability);
				if (Vector3.Distance(transform.position, _currentElement.transform.position) < 0.2f)
				{
					NextGridElement();                         
				}
            }

			if (!breaking)
			{
				transform.position += transform.up.normalized * _speed * Time.deltaTime * _speedMultiplier;
			}
        }
        else
        {
			_wanderTimer += Time.deltaTime;
			if(_wanderTimer > _wanderChangeInterval)
			{
				_wanderTimer = 0.0f;
				_wanderDestination = Wander();
			}

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _wanderDestination), Time.deltaTime * (_fastAvoidance ? 3f : 0.15f));
            transform.position += transform.up.normalized * _speed * Time.deltaTime * _speedMultiplier;
        }
  
    }

    public bool ShipBreak()
    {
        bool breaking = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up.normalized, 1.1f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ship"))
            {
                Ship ship = hit.collider.gameObject.GetComponent<Ship>();
                if (ship != null)
                {
                    if (Captured && _speed > ship.ShipSpeed)
                    {
                        _speed = ship.ShipSpeed;
                        continue;
                    }
                }
                breaking = true;
            }
        }
        return breaking;
    }

    public void ObstacleAvoided()
    {
        _fastAvoidance = false;
        _wanderDestination = Wander();
    }

    public void ObstacleAvoid(Collider2D collider)
    {
        _fastAvoidance = true;
        if (!Captured)
        {
            float shift = 0f;
            if (collider.gameObject.layer == LayerMask.NameToLayer("Island") || collider.gameObject.layer == LayerMask.NameToLayer("Bouy"))
            {
                CircleCollider2D col = collider as CircleCollider2D;
                shift = col.radius;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Submarine"))
            {
                BoxCollider2D col = collider as BoxCollider2D;
                shift = col.size.x > col.size.y ? col.size.x : col.size.y;
            }
            else
            {
                return;
            }

            Vector3 hitPosition = collider.transform.position;
            float avoidX = collider.transform.position.x - transform.position.x;
            float avoidY = collider.transform.position.y - transform.position.y;
            if (avoidX > 0)
            {
                _wanderDestination = new Vector3(hitPosition.x - (shift), hitPosition.y, hitPosition.z);
            }
            else
            {
                _wanderDestination = new Vector3(hitPosition.x + (shift), hitPosition.y, hitPosition.z);
            }
            if (avoidY > 0)
            {
                _wanderDestination = new Vector3(_wanderDestination.x, hitPosition.y - (shift), hitPosition.z);
            }
            else
            {
                _wanderDestination = new Vector3(_wanderDestination.x, hitPosition.y + (shift), hitPosition.z);
            }
            return;
        }
    }

    public void ObstacleAvoidance()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up.normalized, 2.5f);
        foreach (RaycastHit2D hit in hits)
        {
            if (!Captured)
            {
                float shift = 0f;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Island") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Bouy"))
                {
                    CircleCollider2D col = hit.collider as CircleCollider2D;
                    shift = col.radius;
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Submarine"))
                {
                    BoxCollider2D col = hit.collider as BoxCollider2D;
                    shift = col.size.x > col.size.y ? col.size.x : col.size.y;
                }
                else
                {
                    continue;
                }
                
                Vector3 hitPosition = hit.transform.position;
                float avoidX = hit.transform.position.x - transform.position.x;
                float avoidY = hit.transform.position.y - transform.position.y;
                if (avoidX > 0)
                {
                    _wanderDestination = new Vector3(hitPosition.x - (shift + 1.5f), hitPosition.y, hitPosition.z);
                }
                else
                {
                    _wanderDestination = new Vector3(hitPosition.x + (shift + 1.5f), hitPosition.y, hitPosition.z);
                }
                if (avoidY > 0)
                {
                    _wanderDestination = new Vector3(_wanderDestination.x, hitPosition.y - (shift + 1.5f), hitPosition.z);
                }
                else
                {
                    _wanderDestination = new Vector3(_wanderDestination.x, hitPosition.y + (shift + 1.5f), hitPosition.z);
                }
                return;
            }
        }
    }

    private IEnumerator CaptureByLightHouse()
    {
        _circleImage.enabled = true;
        while (_captureTimer < _captureTime)
        {
            _captureTimer += Time.deltaTime;
            _circleImage.fillAmount = _captureTimer/ _captureTime;
            GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position); //new idea
            yield return null;
        }

        _myAgent.CalculatePath();
        _myAgent.Path.RemoveAt(0);
        PathVisibility(true);
        NextGridElement();

        if (_isSuper)
        {
            //PowerUpController.Instance.GetPowerUp(transform.position);
			PowerUpController.Instance.SpawnPowerUp(transform.position);
			_isSuper = false;
            _renderer.sprite = Sprites[0];
        }
		Captured = true;

        yield return new WaitForSeconds(1f);

        StartCoroutine("UncaptureByLightHouse");

    }

    private IEnumerator UncaptureByLightHouse()
    {
        while (_captureTimer > 0f)
        {
            if (!_enlighted)
            {
                _captureTimer -= Time.deltaTime / 3f;
                _circleImage.fillAmount = _captureTimer / _captureTime;
                GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position);                
            }
            yield return null;
        }
        PathVisibility();
        _captureTimer = 0f;
		Captured = false;
        _circleImage.enabled = false;
    }

    protected void NextGridElement()
    {
        if (_myAgent.Path.Count == 0)
        {
            _currentElement = null;
            return;
        }
        if (_currentElement != null) _currentElement.UsePoint(false, this);
        _currentElement = _myAgent.Path[0] as WaterGridElement;
        _myAgent.Path.RemoveAt(0);
    }

	private void SetEvents(bool state)
	{
		if (!state && _eventsSet)
		{
			//PowerUpController.Instance.CaptureBoosterBegin -= CaptureBoostBegin;
			//PowerUpController.Instance.CaptureBoosterEnd -= CapturePowerUpEnd;
			//PowerUpController.Instance.CaptureSlowerBegin -= CaptureSlowerBegin;
			//PowerUpController.Instance.CaptureSlowerEnd -= CapturePowerUpEnd;

			//ActiveController.Instance.OnFreeze -= FreezeShip;
			ActiveController.Instance.ActiveInfos[(int)ActiveSkillsEnum.Freeze].OnActiveSkillUsed -= FreezeShip;
            _eventsSet = false;
		}
		if (state && !_eventsSet)
		{
			//PowerUpController.Instance.CaptureBoosterBegin += CaptureBoostBegin;
			//PowerUpController.Instance.CaptureBoosterEnd += CapturePowerUpEnd;
			//PowerUpController.Instance.CaptureSlowerBegin += CaptureSlowerBegin;
			//PowerUpController.Instance.CaptureSlowerEnd += CapturePowerUpEnd;

			//ActiveController.Instance.OnFreeze += FreezeShip;
			ActiveController.Instance.ActiveInfos[(int)ActiveSkillsEnum.Freeze].OnActiveSkillUsed += FreezeShip;
			_eventsSet = true;
		}
	}

	#endregion Old Methods

	#region Methods

	private void InitializeShip()
	{
		_layer_obstacle = LayerMask.NameToLayer("Obstacle");
		_layer_ship = LayerMask.NameToLayer("Ship");
		_layer_mine = LayerMask.NameToLayer("Mine");
		_layerMask = 0;
		_layerMask |= (1 << _layer_obstacle);
		_layerMask |= (1 << _layer_ship);
		_layerMask |= (1 << _layerMask);
	}

	private void ChangeShipState(ShipState newShipState)
	{
		_lastShipState = _currentShipState;
		OnStateExit();
		_currentShipState = newShipState;
		OnStateEnter();
	}

	private void OnStateExit()
	{
		switch (_lastShipState)
		{
			case ShipState.SS_WANDER:
				break;
			case ShipState.SS_CAPTURED:
				PathVisibility();
				break;
			case ShipState.SS_ARRIVED:
				break;
			case ShipState.SS_DEAD:
				break;
		}
	}
	private void OnStateEnter()
	{
		switch (_currentShipState)
		{
			case ShipState.SS_WANDER:
				Outline.enabled = false;
				break;
			case ShipState.SS_CAPTURED:
				Outline.enabled = true;
				if(_lastShipState == ShipState.SS_WANDER)
				{
					_myAgent.CalculatePath();
					_myAgent.Path.RemoveAt(0);
					PathVisibility(true);
					NextGridElement();

					if (_isSuper)
					{
						//PowerUpController.Instance.GetPowerUp(transform.position);
						PowerUpController.Instance.SpawnPowerUp(transform.position);
						_isSuper = false;
						_renderer.sprite = Sprites[0];
					}
				}
				break;
			case ShipState.SS_ARRIVED:
				Outline.enabled = false;
				break;
			case ShipState.SS_DEAD:
				Outline.enabled = false;
				break;
		}
	}

	private void ProcesShipState()
	{
		switch (_currentShipState)
		{
			case ShipState.SS_WANDER:
				ProcesShipCapture();
                ShipMovement();
				ScanForCollision();
                break;
			case ShipState.SS_CAPTURED:
				ProcesShipCapture();
                ShipMovement();
				break;
			case ShipState.SS_ARRIVED:
				ShipMovement();
				break;
			case ShipState.SS_DEAD:
				break;
		}
	}

	private void ProcesShipCapture()
	{
		float deltaTime = Time.deltaTime;
		if (!_frozen)
		{
			_captureCoolingTimer += deltaTime;
			if (_captureCoolingTimer > _captureCoolingLength)
			{
				_captureProgres -= deltaTime * _captureToCoolTimeRatio;
			}
		}

		_captureProgres = Mathf.Clamp(_captureProgres, 0.0f, CaptureTime);

		if (_currentShipState == ShipState.SS_WANDER && _captureProgres >= CaptureTime)
		{
			ChangeShipState(ShipState.SS_CAPTURED);
		}
		if(_currentShipState == ShipState.SS_CAPTURED && _captureProgres <= 0.0f)
		{
			ChangeShipState(ShipState.SS_WANDER);
		}

		if(_circleImage != null)
		{
			if (_captureProgres > 0.0f)
			{
				GameController.Instance.SetCirclePosition(_circleImage, transform.position);
				_circleImage.enabled = true;
				_circleImage.fillAmount = Mathf.Clamp01(_captureProgres / CaptureTime);
			} else {
				_circleImage.enabled = false;
			}
		}
	}

	public void NotifyCapture(float deltaCapture)
	{
		//Debug.Log("Notify capture");
		_captureCoolingTimer = 0.0f;
		_captureProgres += deltaCapture;
		if (_circleImage == null)
		{
			GameObject circleGO = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_CIRCLE);
			circleGO.SetActive(true);
			_circleImage = circleGO.GetComponent<Image>();
			_circleImage.enabled = true;
		}
	}

	private void ScanForCollision()
	{
		Vector2 boxSize = _boxCollider.size;
		boxSize.x *= _transform.localScale.x;
		boxSize.y *= _transform.localScale.y;

		boxSize.y *= 1.0f;
		boxSize.x *= 2.5f;

		float angle = this.transform.eulerAngles.z;

		//Gizmos.DrawWireCube(_transform.position, boxSize);
		float checkDistance = boxSize.y;

		//DebugTools.DrawDebugBox(_transform.position, angle, boxSize);
		DebugTools.DrawDebugBox(_transform.position + _transform.up * checkDistance, angle, boxSize);
		DebugTools.DrawDebugBox(_transform.position + _transform.up * (checkDistance + _speed * _speedMultiplier), angle, boxSize);
		RaycastHit2D[] hits = Physics2D.BoxCastAll(_transform.position + _transform.up * checkDistance, boxSize, angle, _transform.up, _speed * _speedMultiplier,_layerMask);

		if (hits != null && hits.Length > 1)
		{
			RaycastHit2D[] newHits = hits.Where<RaycastHit2D>(x => x.collider.gameObject != this.gameObject) .ToArray<RaycastHit2D>();
			//bool collidingCourse = false;


			int hitCount = newHits.Length;

			//for(int i = 0;i < hitCount;++i)
			//{
			//	if(hits[i].collider != null && hits[i].collider.gameObject.layer == _layer_obstacle)
			//	{
			//		collidingCourse = true;
			//		break;
			//	}
			//}

			if (hitCount > 0)
			{
				Vector2 obstaclePosition = newHits[0].collider.transform.position;
				Vector2 targetPosition = Vector2.Lerp(_transform.position, obstaclePosition, 0.5f);
				if (_alert == null)
				{
					GameObject alertGO = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_ALERT);
					if(alertGO != null)
					{
						alertGO.SetActive(true);
						_alert = alertGO.GetComponent<Alert>();
					}
				}
				if (_alert != null)
				{
					_alert.UpdateAlert(targetPosition);
				}
			}

		} else {
			if (_alert != null)
			{
				_alert = null;
			}
		}
	}

	private void CleanShipEffects()
	{
		if(_alert != null)
		{
			_alert = null;
		}
	}

	#endregion Methods
}
