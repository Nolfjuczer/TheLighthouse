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
public class ShipCounterType
{
    [SerializeField]
    private int _ferryCount;
    [SerializeField]
    private int _freighterCount;
    [SerializeField]
    private int _keelboatCount;
    [SerializeField]
    private int _motorboatCount;

    public int FerryCount
    {
        get { return _ferryCount; }
    }
    public int FreighterCount
    {
        get { return _freighterCount; }
    }
    public int KeelboatCount
    {
        get { return _keelboatCount; }
    }
    public int MotorboatCount
    {
        get { return _motorboatCount; }
    }

    public ShipCounterType()
    {
        _ferryCount = 0;
        _freighterCount = 0;
        _keelboatCount = 0;
        _motorboatCount = 0;
    }

    public ShipCounterType(int ferry, int freighter, int keelboat, int motorboat)
    {
        _ferryCount = ferry;
        _freighterCount = freighter;
        _keelboatCount = keelboat;
        _motorboatCount = motorboat;
    }

    public void IncrementCounter(ShipTypeEnum shipType)
    {
        switch (shipType)
        {
            case ShipTypeEnum.Ferry:
                _ferryCount += 1;
                break;
            case ShipTypeEnum.Freighter:
                _freighterCount += 1;
                break;
            case ShipTypeEnum.Keelboat:
                _keelboatCount += 1;
                break;
            case ShipTypeEnum.Motorboat:
                _motorboatCount += 1;
                break;
        }
    }

    public bool CompareCounter(ShipCounterType mission)
    {
        if (_ferryCount < mission._ferryCount)
            return false;
        if (_freighterCount < mission._freighterCount)
            return false;
        if (_keelboatCount < mission._keelboatCount)
            return false;
        if (_motorboatCount < mission._motorboatCount)
            return false;
        return true;
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
    private EGameState _gameState = EGameState.PreGame;
	private EGameState _pendingGameState = EGameState.None;

    public AStar.Grid MainGrid;
    public Transform IslandTransfrom;
    public LightSteering Light;
    public Camera MainCamera;

	private Vector2 _referenceResolution = Vector2.zero;

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
				break;
			case EGameState.Lost:
				Time.timeScale = 0.0f;
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
				GUIController.Instance.OnWinGame(_money, _currentShipCounterState, MissionShipCounterState);
				break;
			case EGameState.Lost:
				GUIController.Instance.OnLoseGame(_money, _currentShipCounterState, MissionShipCounterState);
				break;
		}
	}

	private void ResetGameController()
	{
		_gameState = EGameState.PreGame;
		_hp = 3;
		_money = 0;
		_currentShipCounterState = new ShipCounterType();
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

	private ShipCounterType _currentShipCounterState;
	public ShipCounterType CurrentShipCounterState {  get { return _currentShipCounterState; } }
    public ShipCounterType MissionShipCounterState = new ShipCounterType();
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
			_currentShipCounterState.IncrementCounter(shipType);
			if (_currentShipCounterState.CompareCounter(MissionShipCounterState))
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

