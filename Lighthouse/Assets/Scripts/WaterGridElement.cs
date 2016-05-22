using UnityEngine;
using AStar;

public class WaterGridElement : GridElement
{
    public Sprite MySprite;
    protected SpriteRenderer _myRenderer;
    public SpriteRenderer MyRenderer
    {
        get { return _myRenderer; }
        set { _myRenderer = value; }
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
