using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EGameState
{
    InGame,
    Paused,
    Lost
}

public class GameController : Singleton<GameController>
{
    public EGameState GameState
    {
        get { return _gameState; }
        set
        {
            switch (value)
            {
                case EGameState.InGame:
                    Time.timeScale = 1f;
                    break;
                case EGameState.Paused:
                    Time.timeScale = 0.000001f;
                    break;
            }
            _gameState = value;
        }
    }
    private EGameState _gameState = EGameState.InGame;

    public AStar.Grid MainGrid;
    public Transform IslandTransfrom;
    public LightSteering Light;
    public Camera MainCamera;

	private Vector2 _referenceResolution = Vector2.zero;

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

    protected void OnEnable()
    {
        _hp = 3;
    }

    public void Update()
    {
        OnBackButton();
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

    #endregion

    #region InGame

    private int _hp;
    public int Money;

    public void DecreaseHP()
    {
        _hp -= 1;
        for(int i = 0; i < 3; ++i)
        {
            GUIController.Instance.Lives[i].enabled = i < _hp;
        }
        if (_hp == 0)
        {
            Debug.LogWarning("Game Over");
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
#endregion
}

