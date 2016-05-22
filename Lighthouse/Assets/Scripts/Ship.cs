using System;
using UnityEngine;
using System.Collections;
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
    private Image _image;
    private float _speedMultiplier = 0.2f;
    private float _captureTimer;
    private bool _captured;
    private Vector2 _wanderDestination;
    private TrailRenderer _trailRenderer;

    public override void Awake()
    {
        base.Awake();
        _trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
        _renderer = GetComponent<SpriteRenderer>();

        _trailRenderer.sortingLayerID = _renderer.sortingLayerID;
        _trailRenderer.sortingOrder = _renderer.sortingOrder - 1;
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
	
	void Update ()
	{
	    ShipMovement();
	}

    public void GetToPort()
    {
        StartCoroutine(ArriveToLand());
    }

    public void DestoryOnIsland()
    {
        if(_image != null) GameController.Instance.ReturnProgressCircle(_image);
        GameController.Instance.ReturnShip(this);
    }

    private IEnumerator ArriveToLand()
    {
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
        if(_image != null) GameController.Instance.ReturnProgressCircle(_image);
        //TODO give points
        GameController.Instance.ReturnShip(this);
        gameObject.SetActive(false);
    }

    private void ShipMovement()
    {
        if (_captured || _captureTimer > 0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(Vector3.forward, GameController.Instance.IslandTransfrom.position - gameObject.transform.position), 
                Time.deltaTime * 0.15f * ShipManoeuvrability);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _wanderDestination), Time.deltaTime * 0.15f);
        }
        transform.position += transform.up.normalized * ShipSpeed * Time.deltaTime * _speedMultiplier;            
    }

    private IEnumerator CaptureByLightHouse()
    {
        _image = GameController.Instance.GetProgressCricle(gameObject.transform.position);
        while (_captureTimer < CaptureTime)
        {
            _captureTimer += Time.deltaTime;
            _image.fillAmount = _captureTimer/CaptureTime;
            GameController.Instance.SetCirclePosition(_image, gameObject.transform.position);
            yield return null;
        }
        _renderer.color = Color.green;
        _captured = true;
        GameController.Instance.ReturnProgressCircle(_image);
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
            GameController.Instance.ReturnProgressCircle(_image);
        }
    }
}
