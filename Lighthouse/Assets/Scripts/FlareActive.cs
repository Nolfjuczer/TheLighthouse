using UnityEngine;
using System.Collections;

public class FlareActive : MonoBehaviour {

	// Use this for initialization
    void OnEnable()
    {
        StartCoroutine(BurnFlare());
    }

    protected IEnumerator BurnFlare()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
