using UnityEngine;
using System.Collections;

public class Port : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            Ship ship = col2D.GetComponent<Ship>();
            if (ship.Captured) 
            {
                ship.GetToPort();
            }
            else
            {
                ship.ObstacleAvoid(gameObject.GetComponent<Collider2D>());
            }
        }
    }

    public void OnTriggerExit2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            Ship ship = col2D.GetComponent<Ship>();
            ship.ObstacleAvoided();
        }
    }
}
