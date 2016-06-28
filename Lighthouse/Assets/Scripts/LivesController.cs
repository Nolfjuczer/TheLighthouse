using UnityEngine;
using System.Collections;

public class LivesController : MonoBehaviour
{
	#region Variables

	[System.Serializable]
	public struct LifeInfo
	{
		public enum LifeState
		{
			LS_ON = 0,
			LS_FLIGHT = 1,
			LS_OFF
		}
		public GameObject lifeGO;
		[HideInInspector]
		public RectTransform transform;

		public LifeState state;

		private float _timer;
		private const float flightLength = 1.5f;

		private const float regularSize = 1.0f;
		private const float flightSize = 2.0f;
		private const float landSize = 0.5f;

		private static AnimationCurve flightCurve = AnimationCurve.EaseInOut(0.0f,0.0f,1.0f,1.0f);

		[HideInInspector]
		public Vector2 startPosition;
		[System.NonSerialized]
		[HideInInspector]
		public Vector2 endPosition;

		public void ChangeLifeState(LifeState newState)
		{
			state = newState;
			switch(state)
			{
				case LifeState.LS_ON:
					lifeGO.SetActive(true);
					transform.anchoredPosition = startPosition;
					transform.localScale = Vector3.one * regularSize;
					break;
				case LifeState.LS_FLIGHT:
					_timer = 0.0f;
					break;
				case LifeState.LS_OFF:
					lifeGO.SetActive(false);
					break;
			}
		}
		public void UpdateLife(float deltaTime)
		{
			if(state == LifeState.LS_FLIGHT)
			{
				_timer += deltaTime;
				if(_timer > flightLength)
				{
					ChangeLifeState(LifeState.LS_OFF);
				} else {
					float progres = flightCurve.Evaluate(_timer / flightLength);
					transform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progres);
					float halfLength = flightLength * 0.5f;
					if(_timer < halfLength)
					{
						float halfProgres = flightCurve.Evaluate( _timer / (halfLength) );
						transform.localScale = Vector3.one * Mathf.Lerp(regularSize, flightSize, halfProgres);
					} else {
                        float halfProgres = flightCurve.Evaluate( (_timer - halfLength) / (halfLength) );
						transform.localScale = Vector3.one * Mathf.Lerp(flightSize, landSize, halfProgres);
					}
				}
			}
		}

		public void InitLife()
		{
			startPosition = transform.anchoredPosition;
		}
		public void ResetLife()
		{
			ChangeLifeState(LifeState.LS_ON);
		}
		public void SendLife(Vector2 destination)
		{
			endPosition = destination;
			ChangeLifeState(LifeState.LS_FLIGHT);
		}
	}

	public const int maxLifeCount = 3;

	[SerializeField]
	private LifeInfo[] _lifeInfos = null;

	private bool _wasControllerInited = false;

	private int _currentLiveStatus = 0;

	#endregion Variables

	#region Monobehaviour Methods
	
	void OnValidate()
	{
		LifeInfo[] oldLifeInfos = _lifeInfos;
		int oldLifeInfoCount = oldLifeInfos != null ? oldLifeInfos.Length : 0;
		if(oldLifeInfoCount != maxLifeCount)
		{
			_lifeInfos = new LifeInfo[maxLifeCount];
		}
		for(int i = 0;i < maxLifeCount;++i)
		{
			if(i < oldLifeInfoCount)
			{
				_lifeInfos[i] = oldLifeInfos[i];
			}
			if (_lifeInfos[i].lifeGO != null)
			{
				_lifeInfos[i].transform = _lifeInfos[i].lifeGO.GetComponent<RectTransform>();
			}
		}
	}

	void Awake()
	{
		InitController();
    }

	void Update()
	{
		UpdateLifes();
    }

	#endregion Monobehaviour Methods

	#region Methods

	public void InitController()
	{
		_wasControllerInited = true;
		for (int i = 0; i < maxLifeCount; ++i)
		{
			_lifeInfos[i].InitLife();
		}
	}

	public void ResetLifes()
	{
		if(!_wasControllerInited)
		{
			InitController();
		}
		_currentLiveStatus = maxLifeCount;
		for(int i = 0;i < maxLifeCount;++i)
		{
			_lifeInfos[i].ResetLife();
		}
	}

	public void Damage(bool sendHelp,Vector3 worldPosition = new Vector3())
	{
		if (!_wasControllerInited)
		{
			InitController();
		}
		if (_currentLiveStatus > 0)
		{
			--_currentLiveStatus;
			if (sendHelp)
			{
				Vector2 screenPosition = GameController.Instance.WorldToScreenPosition(worldPosition);
				_lifeInfos[_currentLiveStatus].SendLife(screenPosition);
			} else {
				_lifeInfos[_currentLiveStatus].ChangeLifeState(LifeInfo.LifeState.LS_OFF);
            }
		}
	}

	public void UpdateLifes()
	{
		float deltaTime = 0.0f;
		GameController instance = GameController.Instance;
        if (instance != null && instance.GameState == EGameState.PostGame)
		{
			deltaTime = Time.unscaledDeltaTime;
		} else {
			deltaTime = Time.deltaTime;
		}
		for(int i = 0;i < maxLifeCount;++i)
		{
			_lifeInfos[i].UpdateLife(deltaTime);
		}
	}

	#endregion Methods
}
