using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using Random = UnityEngine.Random;

public class ShipSpawner : MonoBehaviour
{
    public enum  SpawnerType
    {
        ST_Vertical,
        ST_Horizontal
    }

	#region Variables

    public SpawnerType MyType;

	public Transform RightBotRestriction;
    public bool Delayed;

	public int MaxCycleSpawn = 3;
	public Vector2 SpawnIntervalMaxMin = new Vector2(5.0f,3.0f);
    private float _currentSpawnInterval;
    public float SpawnIntervalDecreaserPerCycle = 0.5f;
	public Vector2 CycleIntervalMaxMin = new Vector2(15.0f,10f);
    private float _currentCycleInterval;
    public float CycleIntervalDecreaserPerCycle = 0.5f;

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
	    _currentCycleInterval = CycleIntervalMaxMin.x;
	    _currentSpawnInterval = SpawnIntervalMaxMin.x;
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

	private void ProcessSpawning()
	{
		float deltaTime = Time.deltaTime;
		_timer -= deltaTime;
		if(_timer <  0.0f)
		{
			SpawnShip();
			if(_spawnedThisCycle < MaxCycleSpawn)
			{
				_timer += _currentSpawnInterval;
			}
            else
            {
				_timer += _currentCycleInterval;
				_spawnedThisCycle = 0;
			    _currentCycleInterval = Mathf.Clamp(_currentCycleInterval - CycleIntervalDecreaserPerCycle, CycleIntervalMaxMin.y,
			        CycleIntervalMaxMin.x);

                _currentSpawnInterval = Mathf.Clamp(_currentSpawnInterval - SpawnIntervalDecreaserPerCycle, SpawnIntervalMaxMin.y,
                    SpawnIntervalMaxMin.x);
            }
		}
	}

	private void SpawnShip()
	{
		GameObject ship = GameController.Instance.GetShip();

	    Vector3 spawnPosition = Vector3.zero;
	    switch (MyType)
	    {
            case SpawnerType.ST_Horizontal:
                spawnPosition = new Vector3(spawnPosition.x = Random.Range(Mathf.CeilToInt(transform.position.x), Mathf.FloorToInt(RightBotRestriction.position.x)), transform.position.y, 0f);
                break;
            case SpawnerType.ST_Vertical:
                spawnPosition = new Vector3(transform.position.x, spawnPosition.y = Random.Range(Mathf.FloorToInt(RightBotRestriction.position.y) ,Mathf.CeilToInt(transform.position.y)), 0f);
	            break;
	    }
        
		Quaternion spawnQuaternion = Quaternion.LookRotation(Vector3.forward, GameController.Instance.IslandTransfrom.position - spawnPosition);

		ship.transform.position = spawnPosition;
		ship.transform.rotation = spawnQuaternion;
		ship.gameObject.SetActive(true);

		++_spawnedThisCycle;
	}

	#endregion Methods
}
