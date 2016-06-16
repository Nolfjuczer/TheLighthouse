using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EGameState
{
    InGame,
    Paused
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

    public int Money;

    #region Capture

    public RectTransform MainCanvasRectTransform;
    public GameObject ProgressCircleTemplate;
    public List<Image> PossibleCircles;

    public Image GetProgressCricle(Vector3 shipPosition)
    {
        Image img = null;
        if (PossibleCircles.Count > 0)
        {
            img = PossibleCircles[0];
            PossibleCircles.Remove(img);
        }
        else
        {
            GameObject tmpObject = Instantiate(ProgressCircleTemplate, ProgressCircleTemplate.transform.position, Quaternion.identity) as GameObject;
            img = tmpObject.GetComponent<Image>();
            img.rectTransform.SetParent(MainCanvasRectTransform, false);
        }
        SetCirclePosition(img, shipPosition);
        img.gameObject.SetActive(true);
        return img;
    }

    public void ReturnProgressCircle(Image img)
    {
        PossibleCircles.Add(img);
        img.gameObject.SetActive(false);
    }

    public void SetCirclePosition(Image img, Vector3 shipPosition)
    {
        Vector2 ViewportPosition = MainCamera.WorldToViewportPoint(shipPosition);
        //Vector2 WorldObject_ScreenPosition = new Vector2(
        //    ((ViewportPosition.x*MainCanvasRectTransform.sizeDelta.x) - (MainCanvasRectTransform.sizeDelta.x*0.5f)),
        //    ((ViewportPosition.y*MainCanvasRectTransform.sizeDelta.y) - (MainCanvasRectTransform.sizeDelta.y*0.5f)));

		Vector2 WorldObject_ScreenPosition = new Vector2(
			((ViewportPosition.x * _referenceResolution.x) - (_referenceResolution.x * 0.5f)),
			((ViewportPosition.y * _referenceResolution.y) - (_referenceResolution.y * 0.5f)));

		img.rectTransform.anchoredPosition = WorldObject_ScreenPosition;
    }
    #endregion

    #region Flares
    public List<FlareActive> PossibleFlares = new List<FlareActive>();
    public GameObject FlareTemplate;
    public FlareActive GetFlare(Vector3 spawnPosition)
    {
        FlareActive flare = null;
        if (PossibleFlares.Count > 0)
        {
            flare = PossibleFlares[0];
            PossibleFlares.Remove(flare);
        }
        else
        {
            GameObject tmpObject = Instantiate(FlareTemplate, spawnPosition, Quaternion.identity) as GameObject;
            flare = tmpObject.GetComponent<FlareActive>();
        }
        flare.transform.position = spawnPosition;
        flare.gameObject.SetActive(true);
        return flare;
    }

    public void ReturnFlare(FlareActive flare)
    {
        PossibleFlares.Add(flare);
        flare.gameObject.SetActive(false);
    }
    #endregion

    #region Mines
    public List<Mine> PossibleMines = new List<Mine>();
    public GameObject MineTemplate;
    public Mine GetMine(Vector3 spawnPosition)
    {
        Mine mine= null;
        if (PossibleMines.Count > 0)
        {
            mine = PossibleMines[0];
            PossibleMines.Remove(mine);
        }
        else
        {
            GameObject tmpObject = Instantiate(MineTemplate, spawnPosition, Quaternion.identity) as GameObject;
            mine = tmpObject.GetComponent<Mine>();
        }
        mine.transform.position = spawnPosition;
        mine.gameObject.SetActive(true);
        return mine;
    }

    public void ReturnMine(Mine mine)
    {
        PossibleMines.Add(mine);
        mine.gameObject.SetActive(false);
    }
    #endregion

    #region Buoys
    public List<BuoyActive> PossibleBuoys = new List<BuoyActive>();
    public GameObject BuoyTemplate;
    public BuoyActive GetBuoy(Vector3 spawnPosition)
    {
        BuoyActive buoy = null;
        if (PossibleBuoys.Count > 0)
        {
            buoy = PossibleBuoys[0];
            PossibleBuoys.Remove(buoy);
        }
        else
        {
            GameObject tmpObject = Instantiate(BuoyTemplate, spawnPosition, Quaternion.identity) as GameObject;
            buoy = tmpObject.GetComponent<BuoyActive>();
        }
        buoy.transform.position = spawnPosition;
        buoy.gameObject.SetActive(true);
        return buoy;
    }

    public void ReturnBuoy(BuoyActive buoy)
    {
        PossibleBuoys.Add(buoy);
        buoy.gameObject.SetActive(false);
    }
    #endregion

    #region Ships
    public Transform ShipParentTransform;
    public GameObject Keelboat;
    public GameObject MotorshipTempalte;
    public GameObject FerryTemplate;
    public GameObject FreighterTemplate;
    public List<Ship> PossibleShips;

    public Ship GetShip()
    {
        Ship ship;
        if (PossibleShips.Count > 0)
        {
            ship = PossibleShips[Random.Range(0,PossibleShips.Count - 1)];
            PossibleShips.Remove(ship);
        }
        else
        {
            GameObject tmpObject = null;
            int type = Random.Range(0, 4);
            switch (type)
            {
                case 0:
                    tmpObject = Instantiate(Keelboat, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case 1:
                    tmpObject = Instantiate(MotorshipTempalte, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case 2:
                    tmpObject = Instantiate(FerryTemplate, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
                case 3:
                    tmpObject = Instantiate(FreighterTemplate, Vector3.zero, Quaternion.identity) as GameObject;
                    break;
            }
            tmpObject.transform.parent = ShipParentTransform;
            ship = tmpObject.GetComponent<Ship>();
        }
        return ship;
    }

    public void ReturnShip(Ship ship)
    {
        PossibleShips.Add(ship);
        ship.gameObject.SetActive(false);
    }
    #endregion
#endregion
}

