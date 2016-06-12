using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    protected bool _setUp;

    public bool SetUp
    {
        get { return _setUp; }
        set { _setUp = value; }
    }

    void OnEnable()
    {
        gameObject.transform.localScale = Vector3.zero;
        StartCoroutine(SetUpMine());
    }

    protected IEnumerator SetUpMine()
    {
        float setUpTimer = 0f;
        while (setUpTimer < 1f)
        {
            setUpTimer += Time.deltaTime;
            gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one*2f, setUpTimer);
            yield return null;
        }
        _setUp = true;
    }

    public void DisarmMine()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            DisarmMine();
        }
    }
}
