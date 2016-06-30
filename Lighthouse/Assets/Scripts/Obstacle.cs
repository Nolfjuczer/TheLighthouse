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
    public Sprite[] IslandSprites;

    public void OnEnable()
    {
        if (ObstacleType == ObstacleTypeEnum.Whirlpool) StartCoroutine(WhirpoolBehavior());
        else
        {
            SpriteRenderer render = null;
            render = GetComponent<SpriteRenderer>();
            if (render != null) render.sprite = IslandSprites[Random.Range(0, 3)];
        }
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
