using System;
using UnityEngine;
using System.Collections;
using AStar;
using UnityEngine.UI;

public class Ship : WandererBehavior
{
    [Range(1,4)]
    public int ShipManoeuvrability = 1;
    [Range(1, 4)]
    public int ShipSpeed = 1;

    public float CaptureTime = 2f;

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
    private AStarAgent _myAgent;
    private WaterGridElement _currentElement;

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
        _trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
        if (_trailRenderer == null)
        {
            Debug.LogWarning("There's no trail renderer attached");
            gameObject.SetActive(false);
            return;
        }

        _trailRenderer.sortingLayerID = _renderer.sortingLayerID;
        _trailRenderer.sortingOrder = _renderer.sortingOrder - 1;

        _myAgent = GetComponent<AStarAgent>();
        if (_myAgent == null)
        {
            Debug.LogWarning("There's no agent attached");
            gameObject.SetActive(false);
            return;
        }
        _myAgent.MyGrid = GameController.Instance.MainGrid;
        _myAgent.TargetObject = GameController.Instance.IslandTransfrom;
    }

    public override void OnEnable ()
    {
        base.OnEnable();        
        gameObject.transform.localScale = Vector3.one;

        WanderDistance /= ShipManoeuvrability;
        WanderRadius *= ShipManoeuvrability;

        _captureTimer = 0f;
        _captured = false;

        _renderer.color = Color.white;
        _trailRenderer.enabled = true;
        StartCoroutine(WandererCoroutine());
    }

    public IEnumerator WandererCoroutine()
    {
        _wanderDestination = GameController.Instance.IslandTransfrom.position - gameObject.transform.position;
        yield return new WaitForSeconds(5f);
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
        if(_circleImage != null) GameController.Instance.ReturnProgressCircle(_circleImage);
        GameController.Instance.ReturnShip(this);
    }

    private IEnumerator ArriveToLand()
    {
        foreach (GridElement element in _myAgent.Path)
        {
            WaterGridElement waterElement = element as WaterGridElement;
            if (waterElement != null) waterElement.MyRenderer.enabled = false;
        }
        _trailRenderer.enabled = false;
        float landTimer = 3f;
        while (landTimer > 0f)
        {
            landTimer -= Time.deltaTime;
            landTimer = Mathf.Clamp(landTimer,0f,3f);
            gameObject.transform.localScale = Vector3.one * (landTimer / 3f);
            yield return null;
        }
        CleanShip();
    }

    private void CleanShip()
    {
        transform.localScale = Vector3.one;
        if(_circleImage != null) GameController.Instance.ReturnProgressCircle(_circleImage);
        //TODO give points
        GameController.Instance.ReturnShip(this);
        gameObject.SetActive(false);
    }

    private void ShipMovement()
    {
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
        transform.position += transform.up.normalized * ShipSpeed * Time.deltaTime * _speedMultiplier;   
    }

    private IEnumerator CaptureByLightHouse()
    {
        _circleImage = GameController.Instance.GetProgressCricle(gameObject.transform.position);
        while (_captureTimer < CaptureTime)
        {
            _captureTimer += Time.deltaTime;
            _circleImage.fillAmount = _captureTimer/CaptureTime;
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
        _renderer.color = Color.green;
        _captured = true;
        //GameController.Instance.ReturnProgressCircle(_circleImage);
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
        if (_captured) return;
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            StartCoroutine("CaptureByLightHouse");
        }
    }

    public void OnTriggerExit2D(Collider2D col2D)
    {
        if (_captured) return;
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            StopCoroutine("CaptureByLightHouse");
            _captureTimer = 0f;
            GameController.Instance.ReturnProgressCircle(_circleImage);
        }
    }
}
