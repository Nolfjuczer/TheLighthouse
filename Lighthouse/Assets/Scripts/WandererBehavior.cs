using UnityEngine;
using System.Collections;

public class WandererBehavior : MonoBehaviour
{
    public float WanderRadius;
    public float WanderDistance;
    public float WanderJitter;

    private Vector2 _wanderTarget = Vector2.zero;

    public virtual void OnEnable()
    {
        _wanderTarget = gameObject.transform.position;
    }

    public Vector2 Wander()
    {
        _wanderTarget += new Vector2(Random.Range(-1f,1f) * WanderJitter,Random.Range(-1f, 1f) * WanderJitter);

        _wanderTarget.Normalize();

        _wanderTarget *= WanderRadius;

        Vector2 destination = _wanderTarget + (Vector2) transform.up * WanderDistance;

        return destination;
    }
}
