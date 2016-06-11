using UnityEngine;
using System.Collections;

public class WandererBehavior : MonoBehaviour
{
    public float WanderRadius;
    public float WanderDistance;
    public float WanderJitter;

    private float _wanderRadius;
    private float _wanderDistance;
    private float _wanderJitter;

    private Vector2 _wanderTarget = Vector2.zero;

    public virtual void Awake()
    {
        _wanderRadius = WanderRadius;
        _wanderDistance = WanderDistance;
        _wanderJitter = WanderJitter;
    }

    public virtual void OnEnable()
    {
        //_wanderTarget = gameObject.transform.position;
        WanderRadius = _wanderRadius;
        WanderDistance = _wanderDistance;
        WanderJitter = _wanderJitter;
    }

    public Vector2 Wander()
    {
        _wanderTarget = new Vector2(Random.Range(-1f,1f) * WanderJitter,Random.Range(-1f, 1f) * WanderJitter);

        _wanderTarget.Normalize();

        _wanderTarget *= WanderRadius;

        Vector2 destination = _wanderTarget + (Vector2) transform.up * WanderDistance;

        return destination;
    }
}
