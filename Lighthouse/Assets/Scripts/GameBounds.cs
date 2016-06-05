using UnityEngine;
using System.Collections;

public class GameBounds : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            col.gameObject.GetComponent<Ship>().CleanShip();
        }
    }
}
