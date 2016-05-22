using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    public AStar.Grid MainGrid;
    public Transform IslandTransfrom;
    public Camera MainCamera;

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
            img.rectTransform.parent = MainCanvasRectTransform;
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
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x*MainCanvasRectTransform.sizeDelta.x) - (MainCanvasRectTransform.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*MainCanvasRectTransform.sizeDelta.y) - (MainCanvasRectTransform.sizeDelta.y*0.5f)));

        img.rectTransform.anchoredPosition = WorldObject_ScreenPosition;
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
}

