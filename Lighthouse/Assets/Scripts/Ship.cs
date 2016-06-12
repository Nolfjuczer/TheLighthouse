using System;
using UnityEngine;
using System.Collections;
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
    private bool _frozen;

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

        _trailRenderer.Clear();
        _trailRenderer.enabled = true;

        _circleImage = GameController.Instance.GetProgressCricle(transform.position);
        _circleImage.enabled = false;
        _circleImage.color = Color.white;

        _isSuper = Random.Range(0f, 1f) > 0.7f;
        _renderer.sprite = _isSuper ? Sprites[1] : Sprites[0];

        _renderer.enabled = true;

        _died = false;

        _boxCollider.enabled = true;

        PowerUpController.Instance.CaptureBoosterBegin += CaptureBoostBegin;
        PowerUpController.Instance.CaptureBoosterEnd += CapturePowerUpEnd;
        PowerUpController.Instance.CaptureSlowerBegin += CaptureSlowerBegin;
        PowerUpController.Instance.CaptureSlowerEnd += CapturePowerUpEnd;
        ActiveController.Instance.OnFreeze += FreezeShip;

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
        PathVisibility(true);
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

    private void PathVisibility(bool flag = false)
    {
        if (_currentElement != null) _currentElement.MyRenderer.enabled = flag;
        foreach (GridElement element in _myAgent.Path)
        {
            WaterGridElement waterElement = element as WaterGridElement;
            if (waterElement != null) waterElement.MyRenderer.enabled = flag;
        }
    }

    public void CleanShip()
    {
        PathVisibility();

        transform.localScale = Vector3.one;
        if(_circleImage != null) GameController.Instance.ReturnProgressCircle(_circleImage);
        GameController.Instance.ReturnShip(this);
        gameObject.SetActive(false);

        PowerUpController.Instance.CaptureBoosterBegin -= CaptureBoostBegin;
        PowerUpController.Instance.CaptureBoosterEnd -= CapturePowerUpEnd;
        PowerUpController.Instance.CaptureSlowerBegin -= CaptureSlowerBegin;
        PowerUpController.Instance.CaptureSlowerEnd -= CapturePowerUpEnd;
        ActiveController.Instance.OnFreeze -= FreezeShip;
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
        ShipBreak();
        ObstacleAvoidance();

        if (_captured)
        {
            GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position);
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
        transform.position += transform.up.normalized * _speed * Time.deltaTime * _speedMultiplier;   
    }

    public void ShipBreak()
    {
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
            }
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
                float avoidX = hitPosition.x - transform.position.x;
                float avoidY = hitPosition.y - transform.position.y;
                if (avoidX > 0)
                {
                    _wanderDestination = new Vector3(hitPosition.x - (shift + 1f), hitPosition.y, hitPosition.z);
                }
                else
                {
                    _wanderDestination = new Vector3(hitPosition.x + (shift + 1f), hitPosition.y, hitPosition.z);
                }
                if (avoidY > 0)
                {
                    _wanderDestination = new Vector3(hitPosition.x, hitPosition.y - (shift + 1f), hitPosition.z);
                }
                else
                {
                    _wanderDestination = new Vector3(hitPosition.x, hitPosition.y + (shift + 1f), hitPosition.z);
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

        if (_isSuper) PowerUpController.Instance.GetPowerUp(transform.position);
        _captured = true;

        yield return new WaitForSeconds(8f);

        StartCoroutine("UncaptureByLightHouse");

    }

    private IEnumerator UncaptureByLightHouse()
    {
        while (_captureTimer > 0f)
        {
            _captureTimer -= Time.deltaTime;
            _circleImage.fillAmount = _captureTimer / _captureTime;
            GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position);
            yield return null;
        }
        PathVisibility();
        _captureTimer = 0f;
        _captured = false;
        _circleImage.enabled = false;
    }

    protected void NextGridElement()
    {
        if (_myAgent.Path.Count == 0)
        {
            _currentElement = null;
            return;
        }
        if (_currentElement != null) _currentElement.MyRenderer.enabled = false;
        _currentElement = _myAgent.Path[0] as WaterGridElement;
        _myAgent.Path.RemoveAt(0);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (!_renderer.isVisible) return;

        if ((col2D.gameObject.layer == LayerMask.NameToLayer("Light") || col2D.gameObject.layer == LayerMask.NameToLayer("Flare")) && _captureTimer < CaptureTime)
        {
            StopCoroutine("UncaptureByLightHouse");
            StartCoroutine("CaptureByLightHouse");
            return;
        }

        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship") && !_captured)
        {
            DestoryOnIsland();
            return;
        }

        if(col2D.gameObject.layer == LayerMask.NameToLayer("Obstacle") && !_captured)
        {
            DestoryOnIsland();
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
        if (_captured) return;
        if ((col2D.gameObject.layer == LayerMask.NameToLayer("Light") || col2D.gameObject.layer == LayerMask.NameToLayer("Flare")))
        {
            StopCoroutine("CaptureByLightHouse");
            StartCoroutine("UncaptureByLightHouse");
        }
    }
}
