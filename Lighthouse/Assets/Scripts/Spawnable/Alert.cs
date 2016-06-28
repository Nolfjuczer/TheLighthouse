using UnityEngine;
using System.Collections;

public class Alert : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private Transform _transform = null;
	[SerializeField]
	private Transform _spriteTransform = null;

	[SerializeField]
	private AudioClip[] _alertSounds = null;

	private float _deltaScale = 0.15f;

	private float _spriteTimer = 0.0f;
	private float _spriteTimeMultiplier = 6.0f;

	private float _alertTimer = 0.0f;
	private float _alertInterval = 1.0f;

	private float _upTimeTimer = 0.0f;
	private float _timeToHideExclamation = 0.1f;
	private float _timeToDisable = 1.0f;

	#endregion Variables

	#region Monobehaviour Methods

	void OnEnable()
	{
		ResetAlert();
	}
	void Update()
	{
		ProcessAlert();
	}

	#endregion Monobehaviour Methods

	#region Methods

	private void ResetAlert()
	{
		_spriteTransform.gameObject.SetActive(true);
		_spriteTimer = 0.0f;
		_alertTimer = 0.0f;
	}

	private void ProcessAlert()
	{
		float deltaTime = Time.deltaTime;
		_spriteTimer += deltaTime * _spriteTimeMultiplier;
		_alertTimer += deltaTime;

		_spriteTransform.localScale = Vector3.one * (1.0f + Mathf.Sin(_spriteTimer) * _deltaScale);

		if (_alertTimer > _alertInterval)
		{
			_alertTimer = 0.0f;
			//sound alert
		}

		_upTimeTimer += deltaTime;
		if(_upTimeTimer > _timeToHideExclamation)
		{
			_spriteTransform.gameObject.SetActive(false);
        }
		if(_upTimeTimer > _timeToDisable)
		{
			this.gameObject.SetActive(false);
		}
	}

	public void UpdateAlert(Vector3 newPosition)
	{
		_upTimeTimer = 0.0f;
		_transform.position = newPosition;
    }

	#endregion Methods
}
