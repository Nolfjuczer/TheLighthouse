using UnityEngine;
using System.Collections;

public class BuoyActive : Active
{
    protected override IEnumerator Burn()
    {
        yield return base.Burn();
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            Ship ship = col2D.GetComponent<Ship>();
            if (!ship.Captured)
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
