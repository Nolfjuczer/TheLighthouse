using UnityEngine;
using System.Collections;

public class LoadingLogo : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private Transform _transform = null;

	[SerializeField]
	private GameObject[] _leftShips = null;
	[SerializeField]
	private GameObject[] _rightShips = null;

	private const float _speed = 90.0f;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		_transform = this.GetComponent<Transform>();
	}

	void OnEnable()
	{
		if(_leftShips != null)
		{
			RandomizeShips(_leftShips);
		}
		if(_rightShips != null)
		{
			RandomizeShips(_rightShips);
		}
	}

	void Update()
	{
		Quaternion localRotation = _transform.localRotation;
		localRotation *= Quaternion.Euler(new Vector3(0.0f, 0.0f, _speed * Time.unscaledDeltaTime));
		_transform.localRotation = localRotation;
	}

	#endregion Monobehaviour Methods

	#region Methods

	private void RandomizeShips(GameObject[] ships)
	{
		int shipCount = ships.Length;
		int randomToShow = UnityEngine.Random.Range(0, shipCount);
		for(int i = 0;i < shipCount;++i)
		{
			if(ships[i] != null)
			{
				ships[i].SetActive(i == randomToShow);
			}
		}
	}

	#endregion Methods
}
