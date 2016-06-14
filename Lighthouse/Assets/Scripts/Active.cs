using UnityEngine;
using System.Collections;

public abstract class Active : MonoBehaviour
{
    public float Time = 4f;

    // Use this for initialization
    public virtual void OnEnable()
    {
        StartCoroutine(Burn());
    }

    protected virtual IEnumerator Burn()
    {
        yield return new WaitForSeconds(Time);
    }
}
