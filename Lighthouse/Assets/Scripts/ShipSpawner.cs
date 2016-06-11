using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;

public class ShipSpawner : MonoBehaviour
{
    public Transform RightRestriction;
    public bool Delayed;
    public bool Top;

    private int _spawnedThisCycle = 0;

    public void OnEnable()
    {
        StartCoroutine(SpawnnCycle());
    }

    public IEnumerator SpawnnCycle()
    {
        Vector3 spawnPosition;
        Quaternion spawnQuaternion;
        Ship ship;
        while (true)
        {
            if (Delayed)
            {
                Delayed = false;
                yield return new WaitForSeconds(1.5f);
            }

            ship = GameController.Instance.GetShip();

            spawnPosition = new Vector3(spawnPosition.x = Random.Range(transform.position.x, RightRestriction.position.x), transform.position.y, 0f);
            spawnQuaternion = Quaternion.LookRotation(Vector3.forward, GameController.Instance.IslandTransfrom.position - spawnPosition);

            ship.transform.position = spawnPosition;
            ship.transform.rotation = spawnQuaternion;
            ship.gameObject.SetActive(true);
            ++_spawnedThisCycle;
            yield return new  WaitForSeconds(5f);

            if (_spawnedThisCycle >= 3)
            {
                yield return new WaitForSeconds(15f);
                _spawnedThisCycle = 0;
            }
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }
}
