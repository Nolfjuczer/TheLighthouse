using UnityEngine;
using System.Collections;

public enum ObstacleTypeEnum
{
    Whirlpool,
    Island
}

public class Obstacle : MonoBehaviour
{

    public ObstacleTypeEnum ObstacleType;

    public void OnEnable()
    {
        if (ObstacleType == ObstacleTypeEnum.Whirlpool) StartCoroutine(WhirpoolBehavior());
    }

    private IEnumerator WhirpoolBehavior()
    {
        while (true)
        {
            gameObject.transform.Rotate(Vector3.forward, 150f * Time.deltaTime);
            yield return null;
        }
    }
}
