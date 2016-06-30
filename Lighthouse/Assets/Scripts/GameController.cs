using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EGameState : int
{
	PreGame = 0,
    InGame = 1,
	PostGame = 2,
    Paused = 3,
    Win = 4,
	Lost = 5,

	None
}

[System.Serializable]
public class WinController
{
	public enum WinCondition : int
	{
		WC_SHIPS_RESCUED = 0,
		WC_TIME_ELAPSED = 1
	}

	[SerializeField]
	private WinCondition _currentWinCondition = WinCondition.WC_SHIPS_RESCUED;
	public WinCondition CurrentWinCondition {  get { return _currentWinCondition; } }

	[System.Serializable]
	public struct SingleShipCounter
	{
		public ShipTypeEnum shipType;
		[System.NonSerialized]
		public int counter;
		public int goal;
		public SingleShipCounter(ShipTypeEnum shipType)
		{
			this.shipType = shipType;
			this.counter = 0;
			this.goal = 0;
		}
	}

	[SerializeField]
	private SingleShipCounter[] _shipCounters = null;
	public SingleShipCounter[] ShipTypeCounters { get { return _shipCounters; } }

	[HideInInspector]
	[SerializeField]
	private int _shipCounterCount = 0;
	public int ShipCounterCount {  get { return _shipCounterCount; } }

	[System.NonSerialized]
	private float _timer = 0.0f;
	[SerializeField]
	private float _length = 60.0f;

    public WinController()
    {
		_shipCounterCount = (int)ShipTypeEnum.COUNT;
		_shipCounters = new SingleShipCounter[_shipCounterCount];
    }

	public void Update(float deltaTime)
	{
		_timer += deltaTime;
	}

    public void IncrementCounter(ShipTypeEnum shipType)
    {
		int index = (int)shipType;
		if (index >= 0 && index < _shipCounterCount)
		{
			++_shipCounters[index].counter;
		}
    }

	public bool Completed()
	{
		bool result = true;

		switch(_currentWinCondition)
		{
			case WinCondition.WC_SHIPS_RESCUED:
				for(int i = 0;i < _shipCounterCount;++i)
				{
					if(_shipCounters[i].counter < _shipCounters[i].goal)
					{
						result = false;
						break;
					}
				}
				break;
			case WinCondition.WC_TIME_ELAPSED:
				if(_timer < _length)
				{
					result = false;
				}
				break;
		}

		return result;
	}
	public void Validate()
	{
		SingleShipCounter[] oldShipCounters = _shipCounters;
		int oldShipCounterCount = oldShipCounters != null ? oldShipCounters.Length : 0;
		_shipCounterCount = (int)ShipTypeEnum.COUNT;
		if(oldShipCounterCount != _shipCounterCount)
		{
			_shipCounters = new SingleShipCounter[_shipCounterCount];
		}
		for(int i = 0;i < _shipCounterCount;++i)
		{
			if(i < oldShipCounterCount)
			{
				_shipCounters[i] = oldShipCounters[i];
			}
			_shipCounters[i].shipType = (ShipTypeEnum)i;
		}
	}
}

public class GameController : Singleton<GameController>
{
	#region Variables
	public EGameState GameState
    {
        get { return _gameState; }
        set
        {
            _gameState = value;
			OnGameStateChanged();
        }
    }
    private EGameState _gameState = EGameState.None;
	private EGameState _pendingGameState = EGameState.None;

    public AStar.Grid MainGrid;
    public Transform IslandTransfrom;
    public LightSteering Light;
    public Camera MainCamera;

	private Vector2 _referenceResolution = Vector2.zero;
    public Vector2 RereferenceResolution
    {
        get { return _referenceResolution; }
    }

    private float _stateTimer = 0.0f;
	private const float stateLength_preGame = 1.5f;
	private const float stateLength_postGame = 3.0f;

	#endregion Variables

	#region Monobehaviour Methods

	protected override void Awake()
	{
		base.Awake();
		GameLord instance = GameLord.Instance;
		if(instance != null)
		{
			
        }
		GUILord guiLord = GUILord.Instance;
		if(guiLord != null)
		{
			_referenceResolution = guiLord.ReferenceResolution;
		}

	}

	void OnValidate()
	{
		if(_winController == null)
		{
			_winController = new WinController();
		}
		_winController.Validate();
	}

    void OnEnable()
    {
		ResetGameController();
    }

    public void Update()
    {
		ProcessGameState();
        OnBackButton();
    }

	#endregion Monobehaviour Methods

	#region Methods

	private void OnGameStateChanged()
	{
		_stateTimer = 0.0f;
		switch (_gameState)
		{
			case EGameState.PreGame:
				Time.timeScale = 0.0f;
				ActiveController.Instance.ResetActiveController();
				break;
			case EGameState.InGame:
				Time.timeScale = 1.0f;
				break;
			case EGameState.PostGame:
				Time.timeScale = 0.0f;
				break;
			case EGameState.Paused:
				//Time.timeScale = 0.000001f;
				Time.timeScale = 0.0f;
				break;
			case EGameState.Win:
				Time.timeScale = 0.0f;
				GUIController.Instance.ChangeHudState(GUIController.HUDState.HS_WIN);
				break;
			case EGameState.Lost:
				Time.timeScale = 0.0f;
				GUIController.Instance.ChangeHudState(GUIController.HUDState.HS_LOST);
				break;
		}
	}

	private void ProcessGameState()
	{
		_stateTimer += Time.unscaledDeltaTime;
		switch (GameState)
		{
			case EGameState.PreGame:
				if(_stateTimer > stateLength_preGame)
				{
					GameState = EGameState.InGame;
				}
				break;
			case EGameState.InGame:
				_winController.Update(Time.deltaTime);
				break;
			case EGameState.PostGame:
				if (_stateTimer > stateLength_postGame)
				{
					GameState = _pendingGameState;
				}
				break;
			case EGameState.Paused:
				break;
			case EGameState.Win:
				
				break;
			case EGameState.Lost:
				
				break;
		}
	}

	private void ResetGameController()
	{
		_gameState = EGameState.None;
		_hp = 3;
		_money = 0;
		MainGrid.RegenerateGrid();
	}

    #region TotalGameControl

    public void OnBackButton()
    {
        if (InputManager.Instance.ReturnButton())
        {
            if (GameState != EGameState.InGame)
            {
                Application.Quit();
            }
            else
            {
                GameState = EGameState.Paused;
                GUIController.Instance.OnPauseClick();
            }
        }
    }

	#endregion TotalGameControl

	#region InGame

	[SerializeField]
	private WinController _winController;
	public WinController WinController {  get { return _winController; } }
  
    private int _hp;

    private int _money;
    public int Money
    {
        get { return _money;}
    }

    public void ShipCollected(ShipTypeEnum shipType)
    {
		if (GameState == EGameState.InGame)
		{
			_money += 10;
			_winController.IncrementCounter(shipType);
			if (_winController.Completed())
			{
				GameState = EGameState.PostGame;
				_pendingGameState = EGameState.Win;
			}
		}
    }

    public void ShipDestroyed(bool sendHelp, Vector3 worldPosition = new Vector3())
    {
        //_money -= 10;
        DecreaseHp(sendHelp, worldPosition);
    }

    private void DecreaseHp(bool sendHelp, Vector3 worldPosition = new Vector3())
    {
		if (GameState == EGameState.InGame)
		{
			_hp -= 1;
			GUIController.Instance.LivesController.Damage(sendHelp, worldPosition);
			if (_hp == 0)
			{
				GameState = EGameState.PostGame;
				_pendingGameState = EGameState.Lost;
			}
		}
    }

    public void SetCirclePosition(Image img, Vector3 shipPosition)
    {
        Vector2 ViewportPosition = MainCamera.WorldToViewportPoint(shipPosition);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * _referenceResolution.x) - (_referenceResolution.x * 0.5f)),
            ((ViewportPosition.y * _referenceResolution.y) - (_referenceResolution.y * 0.5f)));
            img.rectTransform.anchoredPosition = WorldObject_ScreenPosition;
    }

	public Vector2 WorldToScreenPosition(Vector3 worldPosition)
	{
		Vector2 ViewportPosition = MainCamera.WorldToViewportPoint(worldPosition);
		Vector2 WorldObject_ScreenPosition = new Vector2(
			((ViewportPosition.x * _referenceResolution.x) - (_referenceResolution.x * 0.5f)),
			((ViewportPosition.y * _referenceResolution.y) - (_referenceResolution.y * 0.5f)));
		return WorldObject_ScreenPosition;
	}


public FlareActive GetFlare(Vector3 spawnPosition)
    {
        FlareActive flare =
            InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_FLARE).GetComponent<FlareActive>();
        flare.transform.position = spawnPosition;
        flare.gameObject.SetActive(true);
        return flare;
    }

    public Mine GetMine(Vector3 spawnPosition)
    {
        Mine mine = 
            InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_MINE).GetComponent<Mine>();
        mine.transform.position = spawnPosition;
        mine.gameObject.SetActive(true);
        return mine;
    }

    public BuoyActive GetBuoy(Vector3 spawnPosition)
    {
        BuoyActive buoy =
                        InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_BUOY).GetComponent<BuoyActive>();
        buoy.transform.position = spawnPosition;
        buoy.gameObject.SetActive(true);
        return buoy;
    }  

    public GameObject GetShip()
    {
        GameObject ship = null;
        int type = Random.Range(0, 4);
        switch (type)
        {
            case 0:
                ship = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_KEELBOAT);
                break;
            case 1:
                ship = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_MOTORBOAT);
                break;
            case 2:
                ship = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_FERRY);
                break;
            case 3:
                ship = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_FREIGHTER);
                break;
        }
        return ship;
    }
	#endregion InGame

	#endregion Methods
}

