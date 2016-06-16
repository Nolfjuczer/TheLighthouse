using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum PowerUpType : int
{
    PLightEnlarger = 0,
    PCaptureBooster = 1,
    NDirectionSwapper = 2,
    NCaptureSlower = 3,

	COUNT
}

public class PowerUpController : Singleton<PowerUpController>
{
	#region Variables

    public Action LightEnlargerBegin;
    public Action LightEnlargerEnd;
    public Action CaptureBoosterBegin;
    public Action CaptureBoosterEnd;
    public Action DirectionSwapperBegin;
    public Action DirectionSwapperEnd;
    public Action CaptureSlowerBegin;
    public Action CaptureSlowerEnd;

    private float _lightEnlargerTimer;
    private float _captureBoosterTimer;
    private float _directionSwapperTimer;
    private float _captureSlowerTimer;

	[SerializeField]
	private Transform _transform = null;

	[System.Serializable]
	public struct PowerUpInfo
	{
		public PowerUpType type;
		public GameObject prefab;
		//[HideInInspector]
		public Utility.MemberObjectPool pool;
	}

	[SerializeField]
	private int _powerUpInfoCount = 0;

	[SerializeField]
	private PowerUpInfo[] _powerUpInfos = null;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidatePowerUpController();
    }

	protected override void Awake()
	{
		base.Awake();
		InitPowerUpController();
    }

	#endregion Monobehaviour Methods

	#region Methods

	private void ValidatePowerUpController()
	{
		_transform = this.GetComponent<Transform>();

		_powerUpInfoCount = (int)PowerUpType.COUNT;

		PowerUpInfo[] oldPowerupInfos = _powerUpInfos;
		int oldPowerUpInfoCount = oldPowerupInfos != null ? oldPowerupInfos.Length : 0;

		if(oldPowerUpInfoCount != _powerUpInfoCount)
		{
			_powerUpInfos = new PowerUpInfo[_powerUpInfoCount];
		}
		for(int i = 0;i < _powerUpInfoCount;++i)
		{
			if(i < oldPowerUpInfoCount)
			{
				_powerUpInfos[i] = oldPowerupInfos[i];
			}
			_powerUpInfos[i].type = (PowerUpType)i;
		}
    }

	private void InitPowerUpController()
	{
		for(int i = 0;i < _powerUpInfoCount;++i)
		{
			if(_powerUpInfos[i].prefab != null)
			{
				_powerUpInfos[i].pool = new Utility.MemberObjectPool(_powerUpInfos[i].prefab, _transform, 1);
			}
		}
	}

	/** Input type -1 for random */
	public GameObject SpawnPowerUp(Vector3 position)
	{
		GameObject result = null;

		int type = UnityEngine.Random.Range(0, _powerUpInfoCount);

		result = _powerUpInfos[type].pool.GetPooledObject();

		result.transform.position = position;
		result.SetActive(true);

		return result;
	}

    public void ApplyPowerUp(PowerUpType type)
    {
        switch(type)
        {
            case PowerUpType.NCaptureSlower:
                _captureSlowerTimer = 0f;
                StartCoroutine(CaptureSlowerEnumerator());
                break;
            case PowerUpType.NDirectionSwapper:
                _directionSwapperTimer = 0f;
                StartCoroutine(DirectionSwapperEnumerator());
                break;
            case PowerUpType.PCaptureBooster:
                _captureBoosterTimer = 0f;
                StartCoroutine(CaptureBoosterEnumerator());
                break;
            case PowerUpType.PLightEnlarger:
                _lightEnlargerTimer = 0f;
                StartCoroutine(LightEnlargerEnumerator());
                break;
        }
    }

    protected IEnumerator CaptureSlowerEnumerator()
    {
        CaptureSlowerBegin();
        GUIController.Instance.PowerUps[1].fillAmount = 1;
        GUIController.Instance.PowerUps[1].gameObject.SetActive(true);
        while (_captureSlowerTimer < 5f)
        {
            _captureSlowerTimer += Time.deltaTime;
            GUIController.Instance.PowerUps[1].fillAmount = (5f - _captureSlowerTimer) / 5f;
            yield return null;
        }
        CaptureSlowerEnd();
        GUIController.Instance.PowerUps[1].gameObject.SetActive(false);
    }

    protected IEnumerator CaptureBoosterEnumerator()
    {
        CaptureBoosterBegin();
        GUIController.Instance.PowerUps[2].fillAmount = 1;
        GUIController.Instance.PowerUps[2].gameObject.SetActive(true);
        while (_captureBoosterTimer < 5f)
        {
            _captureBoosterTimer += Time.deltaTime;
            GUIController.Instance.PowerUps[2].fillAmount = (5f - _captureBoosterTimer) / 5f;
            yield return null;
        }
        CaptureBoosterEnd();
        GUIController.Instance.PowerUps[2].gameObject.SetActive(false);
    }

    protected IEnumerator DirectionSwapperEnumerator()
    {
        DirectionSwapperBegin();
        GUIController.Instance.PowerUps[3].fillAmount = 1;
        GUIController.Instance.PowerUps[3].gameObject.SetActive(true);
        while (_directionSwapperTimer < 5f)
        {
            _directionSwapperTimer += Time.deltaTime;
            GUIController.Instance.PowerUps[3].fillAmount = (5f - _directionSwapperTimer) / 5f;
            yield return null;
        }
        DirectionSwapperEnd();
        GUIController.Instance.PowerUps[3].gameObject.SetActive(false);
    }

    protected IEnumerator LightEnlargerEnumerator()
    {
        LightEnlargerBegin();
        GUIController.Instance.PowerUps[0].fillAmount = 1;
        GUIController.Instance.PowerUps[0].gameObject.SetActive(true);
        while (_lightEnlargerTimer < 5f)
        {
            _lightEnlargerTimer += Time.deltaTime;
            GUIController.Instance.PowerUps[0].fillAmount = (5f - _lightEnlargerTimer) / 5f;
            yield return null;
        }
        LightEnlargerEnd();
        GUIController.Instance.PowerUps[0].gameObject.SetActive(false);
    }

	#endregion Methods
}
