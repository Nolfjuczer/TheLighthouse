using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using Random = UnityEngine.Random;

public class ShipSpawner : MonoBehaviour
{
	#region Variables

	public Transform RightRestriction;
    public bool Delayed;
    public bool Top;

	private const int cycleSpawnCount = 3;
	private const float spawnInterval = 5.0f;
	private const float cycleInterval = 15.0f;

	private float _timer = 0.0f;
    private int _spawnedThisCycle = 0;

	#endregion Variables

	#region Monobehaviour Methods

	void OnEnable()
	{
		if(Delayed)
		{
			_timer = 1.5f;
		} else {
			_timer = 0.0f;
		}
	}

	void Update()
	{
		GameController instance = GameController.Instance;
		if(instance != null && instance.GameState == EGameState.InGame)
		{
			ProcessSpawning();
		}
	}

	#endregion Monobehaviour Methods

	#region Methods

	//public IEnumerator SpawnnCycle()
    //{
	//
    //    while (true)
    //    {
    //        if (Delayed)
    //        {
    //            Delayed = false;
    //            yield return new WaitForSeconds(1.5f);
    //        }
	//		if (GameController.Instance.GameState != EGameState.InGame)
	//		{
	//			yield break;
	//		}
	//		SpawnShip();
	//		 ++_spawnedThisCycle;
    //        yield return new  WaitForSeconds(5f);
	//
    //        if (_spawnedThisCycle >= 3)
    //        {
    //            yield return new WaitForSeconds(15f);
    //            _spawnedThisCycle = 0;
    //        }
    //    }
    //}

	private void ProcessSpawning()
	{
		float deltaTime = Time.deltaTime;
		_timer -= deltaTime;
		if(_timer <  0.0f)
		{
			SpawnShip();
			if(_spawnedThisCycle < cycleSpawnCount)
			{
				_timer += spawnInterval;
			} else {
				_timer += cycleInterval;
				_spawnedThisCycle = 0;
			}
		}
	}

	private void SpawnShip()
	{
		GameObject ship = GameController.Instance.GetShip();

		Vector3 spawnPosition = new Vector3(spawnPosition.x = Random.Range(Mathf.CeilToInt(transform.position.x), Mathf.FloorToInt(RightRestriction.position.x)), transform.position.y, 0f);
		Quaternion spawnQuaternion = Quaternion.LookRotation(Vector3.forward, GameController.Instance.IslandTransfrom.position - spawnPosition);

		ship.transform.position = spawnPosition;
		ship.transform.rotation = spawnQuaternion;
		ship.gameObject.SetActive(true);

		++_spawnedThisCycle;
	}

	#endregion Methods
}
