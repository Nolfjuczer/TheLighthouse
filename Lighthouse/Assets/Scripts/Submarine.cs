using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AStar;
using UnityEngine.UI;

public class Submarine : MonoBehaviour
{
    [Range(1, 4)]
    public int ShipManoeuvrability = 1;
    [Range(1, 4)]
    public int ShipSpeed = 1;

    public GameObject BotPart;

    private float _fearTimer = 0f;
    private float _mineTimer = 0f;
    private float _mineNeedTime = 2f;
    private float _speedMultiplier = 0.2f;
    private float _emergeNeedTime = 2f;
    private float _emergeTimer;
    private bool _emerged;
    private bool _onPosition;
    private bool _lighted;

    private SpriteRenderer _botRenderer;
    private AStarAgent _myAgent;
    private WaterGridElement _currentElement;
    private Image _circleImage;
    private TrailRenderer _trailRenderer;

    private Color _whiteNoAlpha = new Color(1f, 1f, 1f, 0f);

    protected void Awake()
    {
        _botRenderer = BotPart.GetComponent<SpriteRenderer>();
        if (_botRenderer == null)
        {
            Debug.LogWarning("There's no renderer attached on bot part");
            gameObject.SetActive(false);
            return;
        }

        _myAgent = GetComponent<AStarAgent>();
        if (_myAgent == null)
        {
            Debug.LogWarning("There's no agent attached");
            gameObject.SetActive(false);
            return;
        }
        _myAgent.MyGrid = GameController.Instance.MainGrid;
        _myAgent.TargetObject = GameController.Instance.IslandTransfrom;

        _trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
        if (_trailRenderer == null)
        {
            Debug.LogWarning("There's no trail renderer attached");
            gameObject.SetActive(false);
            return;
        }

        _trailRenderer.sortingLayerID = _botRenderer.sortingLayerID;
        _trailRenderer.sortingOrder = _botRenderer.sortingOrder - 1;
    }

    public void OnEnable()
    {
		GameObject circleGO = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_CIRCLE);
		//_circleImage = GameController.Instance.GetProgressCricle(transform.position);
		_circleImage = circleGO.GetComponent<Image>();
		_circleImage.gameObject.SetActive(true);
		_circleImage.enabled = false;
        _circleImage.color = Color.red;
        SetCourseOnTarget();
    }

	void OnDisable()
	{
		if(_circleImage != null)
		{
			_circleImage.gameObject.SetActive(false);
			_circleImage = null;
        }
	}

    protected void SetCourseOnTarget()
    {
        RandomTargetElement();
        _myAgent.CalculatePath();
        NextGridElement();
    }

    protected void RandomTargetElement()
    {
        int x, y;
        GridElement possibruElemento = null;
        while (possibruElemento == null || !possibruElemento.Walkable)
        {
            x = Random.Range(8, 22);
            y = Random.Range(0, 2);
            y = y == 0 ? Random.Range(4, 11) : Random.Range(19, 26);
            possibruElemento = _myAgent.MyGrid.Elements[x, y, 0];
        }
        _myAgent.TargetObject = possibruElemento.transform;
    }

    public void Update()
    {
        ShipMovement();
    }

    private void ShipMovement()
    {
        if (_currentElement != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(Vector3.forward, _currentElement.transform.position - gameObject.transform.position),
                Time.deltaTime * ShipManoeuvrability);
            if (Vector3.Distance(transform.position, _currentElement.transform.position) < 0.2f)
            {
                NextGridElement();
                if(_myAgent.Path.Count == 1)
                    StartCoroutine(EmergeCoroutine(true));
                if (_currentElement == null)
                    StartCoroutine("MineTime");
            }

            transform.position += transform.up.normalized * ShipSpeed * Time.deltaTime * _speedMultiplier;
        }
    }

    protected IEnumerator EmergeCoroutine(bool flag)
    {
        _emergeTimer = flag ? 0f : _emergeNeedTime;
        while (true)
        {
            if (flag)
            {
                _emergeTimer += Time.deltaTime;
                _botRenderer.color = Color.Lerp(_whiteNoAlpha, Color.white, _emergeTimer/_emergeNeedTime);
                if (_emergeTimer >= _emergeNeedTime)
                {
                    _trailRenderer.transform.localPosition = Vector3.up * -3.5f;
                    break;
                }
            }
            else
            {
                _emergeTimer -= Time.deltaTime;
                _botRenderer.color = Color.Lerp(_whiteNoAlpha, Color.white, _emergeTimer / _emergeNeedTime);
                if (_emergeTimer <= 0f)
                {
                    _trailRenderer.transform.localPosition = Vector3.up;
                    SetCourseOnTarget();
                    break;
                }
            }
            yield return null;
        }
        _emerged = flag;
    }

    protected IEnumerator MineTime()
    {
        while (!_emerged)
        {
            yield return null;
        }
        while (_mineTimer < _mineNeedTime)
        {
            if (!_lighted) _mineTimer += Time.deltaTime;
            yield return null;
        }
        SpawnMine();
        StartCoroutine(EmergeCoroutine(false));
    }

    protected void SpawnMine()
    {
        GameController.Instance.GetMine(transform.position);
    }

    protected void NextGridElement()
    {
        if (_myAgent.Path.Count == 0)
        {
            _currentElement = null;
            return;
        }
        _currentElement = _myAgent.Path[0] as WaterGridElement;
        _myAgent.Path.RemoveAt(0);

		//SpawnMine();
    }

    protected IEnumerator FearOfTheLight()
    {
        GameController.Instance.SetCirclePosition(_circleImage, gameObject.transform.position);
        _circleImage.enabled = true;
        while (_fearTimer < 1f)
        {
            _fearTimer += Time.deltaTime;
            _circleImage.fillAmount = _fearTimer;
            yield return null;
        }
        _fearTimer = 0f;
        _circleImage.enabled = false;
        _mineTimer = 0f;
        StartCoroutine(EmergeCoroutine(false));
        StopCoroutine("MineTime");
    }

    private IEnumerator UnfearOfTheLight()
    {
        while (_fearTimer > 0f)
        {
            _fearTimer -= Time.deltaTime;
            _circleImage.fillAmount = _fearTimer;
            yield return null;
        }
        _fearTimer = 0f;
        _circleImage.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (!_emerged) return;

        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            _lighted = true;
            StopCoroutine("UnfearOfTheLight");
            StartCoroutine("FearOfTheLight");
            return;
        }
    }

    public void OnTriggerExit2D(Collider2D col2D)
    {
        if (!_emerged || _emergeTimer > 0f) return;

        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            _lighted = false;
            StopCoroutine("FearOfTheLight");
            StartCoroutine("UnfearOfTheLight");
            return;
        }
    }

}
