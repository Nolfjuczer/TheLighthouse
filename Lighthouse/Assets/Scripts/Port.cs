using UnityEngine;
using System.Collections;

public class Port : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        Debug.Log("port?");
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            Ship ship = col2D.GetComponent<Ship>();
            if (ship != null && ship.Captured) 
            {
                ship.GetToPort();
            }
            else
            {
                Debug.Log("ShipCrushed!");
                Destroy(col2D.gameObject);
            }
        }
    }
}
