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
	}

	#endregion Methods
}
