using System.Collections.Generic;
using UnityEngine;
using AStar;
using UnityEngine.EventSystems;

public class WaterGridElement : GridElement
{
    public Sprite MySprite;
    protected SpriteRenderer _myRenderer;

    protected List<Ship> _shipsUsing = new List<Ship>();

    public void UsePoint(bool flag, Ship ship)
    {
        if (flag)
        {
            if (_shipsUsing.Contains(ship)) return;
            _shipsUsing.Add(ship);
            _myRenderer.enabled = true;

        }
        else
        {
            _shipsUsing.Remove(ship);
            if (_shipsUsing.Count <= 0) _myRenderer.enabled = false;
        }
    }

    public void OnEnable()
    {
        _myRenderer= GetComponent<SpriteRenderer>();
        if (_myRenderer == null)
        {
            _myRenderer = gameObject.AddComponent<SpriteRenderer>();
            _myRenderer.sortingLayerID = LayerMask.NameToLayer("Ship");
            _myRenderer.sprite = MySprite;
        }
        _myRenderer.enabled = false;
    }
}
