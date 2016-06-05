using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Text;
using AStar;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Ship : WandererBehavior
{
    [Range(1,4)]
    public int ShipManoeuvrability = 1;
    [Range(1, 4)]
    public int ShipSpeed = 1;

    public Sprite[] Sprites;

    public float CaptureTime = 2f;
    private float _captureTime;

    public bool Captured
    {
        get { return _captured;}
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

    private int _speed;
    private Vector3 _baseScale;

    private bool _died;

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
        _myAgent.MyGrid = GameController.Instance.MainGrid;
        _myAgent.TargetObject = GameController.Instance.IslandTransfrom;

        _baseScale = gameObject.transform.localScale;
        _speed = ShipManoeuvrability;
        _captureTime = CaptureTime;
    }

    public override void OnEnable ()
    {
        base.OnEnable();
                
        gameObject.transform.localScale = _baseScale;
        _speed = ShipSpeed;

        WanderDistance /= ShipManoeuvrability;
        WanderRadius *= ShipManoeuvrability;

        _captureTimer = 0f;
        _captured = false;

        _trailRenderer.enabled = true;

        _circleImage = GameController.Instance.GetProgressCricle(transform.position);
        _circleImage.enabled = false;

        _isSuper = Random.Range(0f, 1f) > 0.7f;
        _renderer.sprite = _isSuper ? Sprites[1] : Sprites[0];

        _renderer.color = Color.white;
        _renderer.enabled = true;

        _died = false;

        _boxCollider.enabled = true;

        PowerUpController.Instance.CaptureBoosterBegin += CaptureBoostBegin;
        PowerUpController.Instance.CaptureBoosterEnd += CapturePowerUpEnd;
        PowerUpController.Instance.CaptureSlowerBegin += CaptureSlowerBegin;
        PowerUpController.Instance.CaptureSlowerEnd += CapturePowerUpEnd;

        StartCoroutine(WandererCoroutine());
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
            yield return new WaitForSeconds(1f);
        }
    }
	
	public void Update ()
	{
	    ShipMovement();
	}

    public void GetToPort()
    {
        StartCoroutine(ArriveToLand());
    }

    public void DestoryOnIsland()
    {
        if(_captureTimer > 0f) StopCoroutine("CaptureByLightHouse");
        _trailRenderer.enabled = false;
        _boxCollider.enabled = false;

        _particleSystem.Play();
        StartCoroutine(WaitForExplosion());
        _circleImage.enabled = false;
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
            landTimer -= Time.deltaTime;
            landTimer = Mathf.Clamp(landTimer, 0f, float.MaxValue);
            yield return null;
        }
        _particleSystem.Stop();
        CleanShip();
    }

    private IEnumerator ArriveToLand()
    {
        _circleImage.enabled = false;
        foreach (GridElement element in _myAgent.Path)
        {
            WaterGridElement waterElement = element as WaterGridElement;
            if (waterElement != null) waterElement.MyRenderer.enabled = false;
        }
        _trailRenderer.enabled = false;
        float landTimer = 2f;
        while (landTimer > 0f)
        {
            landTimer -= Time.deltaTime;
            landTimer = Mathf.Clamp(landTimer,0f,3f);
            gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 0.05f, _baseScale, landTimer / 2f);
            yield return null;
        }
        //TODO give points
        CleanShip();
    }

    public void CleanShip()
    {
        transform.localScale = Vector3.one;
        if(_circleImage != null) GameController.Instance.ReturnProgressCircle(_circleImage);
        GameController.Instance.ReturnShip(this);
        gameObject.SetActive(false);

        PowerUpController.Instance.CaptureBoosterBegin -= CaptureBoostBegin;
        PowerUpController.Instance.CaptureBoosterEnd -= CapturePowerUpEnd;
        PowerUpController.Instance.CaptureSlowerBegin -= CaptureSlowerBegin;
        PowerUpController.Instance.CaptureSlowerEnd -= CapturePowerUpEnd;
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
        if (_died) return;
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, transform.up.normalized * 1.75f, Color.red);
#endif
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up.normalized, 1.75f);
        if (_captured)
        {
            GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position); //new idea
            //transform.rotation = Quaternion.Lerp(transform.rotation, 
            //    Quaternion.LookRotation(Vector3.forward, GameController.Instance.IslandTransfrom.position - gameObject.transform.position), 
            //    Time.deltaTime * 0.15f * ShipManoeuvrability);
            if (_currentElement != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(Vector3.forward, _currentElement.transform.position - gameObject.transform.position),
                    Time.deltaTime * /*0.15f **/ ShipManoeuvrability);
                if( Vector3.Distance(transform.position, _currentElement.transform.position) < 0.2f) NextGridElement();                         
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _wanderDestination), Time.deltaTime * 0.15f);
        }

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
                    }
                    else if(!Captured && _speed > ship.ShipSpeed)
                    {
                        _wanderDestination = Wander();
                    }
                }
            }
        }

        transform.position += transform.up.normalized * _speed * Time.deltaTime * _speedMultiplier;   
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
        foreach (GridElement element in _myAgent.Path)
        {
            WaterGridElement waterElement = element as WaterGridElement;
            if(waterElement != null) waterElement.MyRenderer.enabled = true;
        }
        NextGridElement();

        if (_isSuper) PowerUpController.Instance.GetPowerUp(transform.position);
        _captured = true;
    }

    protected void NextGridElement()
    {
        if (_myAgent.Path.Count == 0) return;
        if(_currentElement != null) _currentElement.MyRenderer.enabled = false;
        _currentElement = _myAgent.Path[0] as WaterGridElement;
        _myAgent.Path.RemoveAt(0);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (!_renderer.isVisible) return;
        if (!_captured)
        {
            if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
            {
                StartCoroutine("CaptureByLightHouse");
                return;
            }

            if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
            {
                DestoryOnIsland();
                return;
            }
            if(col2D.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                DestoryOnIsland();
                return;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D col2D)
    {
        if (!_renderer.isVisible) return;
        if (_captured) return;
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            StopCoroutine("CaptureByLightHouse");
            _circleImage.enabled = false;
            _captureTimer = 0f;
        }
    }
}
