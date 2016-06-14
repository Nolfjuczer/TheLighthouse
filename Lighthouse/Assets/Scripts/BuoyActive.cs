using UnityEngine;
using System.Collections;

public class BuoyActive : Active
{
    protected override IEnumerator Burn()
    {
        yield return base.Burn();
        GameController.Instance.ReturnBuoy(this);
    }
}
