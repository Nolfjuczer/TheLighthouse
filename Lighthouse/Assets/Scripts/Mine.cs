using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    protected bool _setUp;
    protected Vector3 _baseLocalScale;

    public int Touches;

    public bool SetUp
    {
        get { return _setUp; }
        set { _setUp = value; }
    }

    void Awake()
    {
        _baseLocalScale = transform.localScale;        
    }

    void OnEnable()
    {
        gameObject.transform.localScale = Vector3.zero;
        StartCoroutine(SetUpMine());
        Touches = 0;
    }

    protected IEnumerator SetUpMine()
    {
        yield return new WaitForSeconds(1f);
        float setUpTimer = 0f;
        while (setUpTimer < 1f)
        {
            setUpTimer += Time.deltaTime;
            gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, _baseLocalScale, setUpTimer);
            yield return null;
        }
        _setUp = true;
    }

    public void DisarmMine()
    {
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            DisarmMine();
        }
    }
}
