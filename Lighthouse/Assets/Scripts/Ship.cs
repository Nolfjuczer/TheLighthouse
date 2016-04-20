using System;
using UnityEngine;
using System.Collections;
using Debug = UnityEngine.Debug;

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

    private SpriteRenderer _spriteRenderer;
    private float _speedMultiplier = 0.3f;
    private float _captureTimer;
    private bool _captured;

    private Transform _lightHouseTransfrom;

    public void Awake()
    {
        _lightHouseTransfrom = FindObjectOfType<Port>().transform;
        if (_lightHouseTransfrom == null)
        {
            Debug.LogError("Coldn't find Lighthouse!");
            gameObject.SetActive(false);
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    public override void OnEnable ()
    {
        base.OnEnable();        
        WanderDistance /= ShipManoeuvrability;
        WanderRadius *= ShipManoeuvrability;
        //WanderJitter *= ShipManoeuvrability;

        _captureTimer = 0f;
        _captured = false;

        _spriteRenderer.color = Color.white;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    ShipMovement();
	}

    public void GetToPort()
    {
        Debug.Log("GotALandCapitan");
        StartCoroutine(ArriveToLand());
    }

    private IEnumerator ArriveToLand()
    {
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
        //TODO give points
        //TODO back to pool
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    private void ShipMovement()
    {
        if (_captured || _captureTimer > 0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _lightHouseTransfrom.position - gameObject.transform.position), Time.deltaTime * 0.2f * ShipManoeuvrability);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Wander()), Time.deltaTime * 0.5f);
        }
        transform.position += transform.up * ShipSpeed * Time.deltaTime * _speedMultiplier;            
    }

    private IEnumerator CaptureByLightHouse()
    {
        while (_captureTimer < CaptureTime)
        {
            _captureTimer += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(Color.white, Color.green, _captureTimer / CaptureTime);
            yield return null;
        }
        _captureTimer = CaptureTime;
        _captured = true;
        Debug.Log("Captured!");
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
            _spriteRenderer.color = Color.white;
        }
    }
}
