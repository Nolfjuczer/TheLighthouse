using UnityEngine;
using System.Collections;

public class FlareActive : Active
{
    protected override IEnumerator Burn()
    {
        yield return base.Burn();
        gameObject.SetActive(false);
    }
}
