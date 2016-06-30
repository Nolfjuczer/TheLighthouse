using UnityEngine;
using System.Collections;

public class GUILord : Singleton<GUILord>
{
	#region Variables

	public enum GUIState
	{
		GUIS_MENU = 0,
		GUIS_LOADING = 1,
		GUIS_GAME = 2,

		GUIS_COUNT,
		GUIS_NONE
	}

	public enum TransitionState
	{
		TS_NONE = 0,
		TS_FADE_OUT = 1,
		TS_FADE_IN = 2
	}

	[System.Serializable]
	public struct GUIStateInfo
	{
		public GUIState state;
		public GameObject panel;
	}

	[SerializeField]
	private GUIStateInfo[] _guiStateInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _guiStateInfoCount = 0;

	[SerializeField]
	private UnityEngine.UI.CanvasScaler _canvasScaler = null;

	[SerializeField]
	private Camera _mainUICamera = null;

	public Vector2 ReferenceResolution
	{
		get
		{
			return _canvasScaler.referenceResolution;
		}
	}

	[SerializeField]
	private UnityEngine.UI.Image _fader = null;

	private GUIState _currentGUIState = GUIState.GUIS_MENU;
	private GUIState _targetGUIState = GUIState.GUIS_NONE;
	private GUIState _lastGUIState = GUIState.GUIS_NONE;

	private TransitionState _currentTransitionState = TransitionState.TS_NONE;
	private float _transitionTimer = 0.0f;
	private float _transitionLength = 0.5f;

	public bool IsTransition { get { return _currentTransitionState != TransitionState.TS_NONE; } }

	public TransitionState CurrentTransitionState { get { return _currentTransitionState; } }

	public System.Action<GUIState> OnPostGUIStateChanged;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateGUILord();
    }

	protected override void Awake()
	{
		base.Awake();
		InitializeGUILord();
    }

	void Start()
	{
		StartGUILord();
    }

	void Update()
	{
		ProcesTransition();
	}

	#endregion Monobehaviour Methods

	#region Methods

	private void ValidateGUILord()
	{
		_guiStateInfoCount = (int)GUIState.GUIS_COUNT;
		GUIStateInfo[] oldInfos = _guiStateInfos;
		int oldInfoCount = oldInfos != null ? oldInfos.Length : 0;
		if (oldInfoCount != _guiStateInfoCount)
		{
			_guiStateInfos = new GUIStateInfo[_guiStateInfoCount];
		}
		for (int i = 0; i < _guiStateInfoCount; ++i)
		{
			if (i < oldInfoCount)
			{
				_guiStateInfos[i] = oldInfos[i];
			}
			_guiStateInfos[i].state = (GUIState)i;
		}
	}

	private void InitializeGUILord()
	{
		for(int i = 0;i < _guiStateInfoCount;++i)
		{
			_guiStateInfos[i].panel.SetActive(_guiStateInfos[i].state == GUIState.GUIS_MENU);
		}
	}

	private void StartGUILord()
	{
		switch (_currentGUIState)
		{
			case GUIState.GUIS_MENU:
				_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
				break;
			case GUIState.GUIS_LOADING:
				_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
				break;
			case GUIState.GUIS_GAME:
				_mainUICamera.clearFlags = CameraClearFlags.Nothing;
				break;
		}
	}

	public void ChangeGUIState(GUIState newGUIState, bool instant = false)
	{
		if (instant)
		{
			SetPanelActive(newGUIState, true);
			SetPanelActive(_currentGUIState, false);
			_lastGUIState = _currentGUIState;
			_currentGUIState = newGUIState;
			_targetGUIState = GUIState.GUIS_NONE;
			switch(_currentGUIState)
			{
				case GUIState.GUIS_MENU:
					_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
					break;
				case GUIState.GUIS_LOADING:
					_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
					break;
				case GUIState.GUIS_GAME:
					_mainUICamera.clearFlags = CameraClearFlags.Nothing;
					break;
			}
		} else {
			switch (_currentTransitionState)
			{
				case TransitionState.TS_NONE:
					{
						_targetGUIState = newGUIState;
						_currentTransitionState = TransitionState.TS_FADE_OUT;
					}
					break;
				case TransitionState.TS_FADE_OUT:
					{
						_targetGUIState = newGUIState;
					}
					break;
				case TransitionState.TS_FADE_IN:
					{
						_targetGUIState = newGUIState;
						_currentTransitionState = TransitionState.TS_FADE_OUT;
					}
					break;
			}
		}
	}

	private void ProcesTransition()
	{
		switch(_currentTransitionState)
		{
			case TransitionState.TS_NONE:
				break;
			case TransitionState.TS_FADE_OUT:
				{
					if(!_fader.gameObject.activeSelf)
					{
						_fader.gameObject.SetActive(true);
					}
					
					_transitionTimer += Time.unscaledDeltaTime;
					Color faderColor = _fader.color;
					faderColor.a = Mathf.Clamp01(_transitionTimer / _transitionLength);
					_fader.color = faderColor;
					if(_transitionTimer > _transitionLength)
					{
						_lastGUIState = _currentGUIState;
						_currentGUIState = _targetGUIState;
						_targetGUIState = GUIState.GUIS_NONE;

						SetPanelActive(_currentGUIState, true);
						SetPanelActive(_lastGUIState, false);

						switch (_currentGUIState)
						{
							case GUIState.GUIS_MENU:
								_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
								break;
							case GUIState.GUIS_LOADING:
								_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
								break;
							case GUIState.GUIS_GAME:
								_mainUICamera.clearFlags = CameraClearFlags.Nothing;
								break;
						}

						_currentTransitionState = TransitionState.TS_FADE_IN;
					}
				}
				break;
			case TransitionState.TS_FADE_IN:
				{
					_transitionTimer -= Time.unscaledDeltaTime;
					Color faderColor = _fader.color;
					faderColor.a = Mathf.Clamp01(_transitionTimer / _transitionLength);
					_fader.color = faderColor;
					if (_transitionTimer < 0.0f)
					{
						_transitionTimer = 0.0f;

						_fader.gameObject.SetActive(false);
						_currentTransitionState = TransitionState.TS_NONE;
						PostGUIStateChange(_currentGUIState);
                    }
				}
				break;
		}
	}

	private void SetPanelActive(GUIState state, bool active = true)
	{
		int index = (int)state;
		if(index >= 0 && index < _guiStateInfoCount)
		{
			if(_guiStateInfos[index].panel != null)
			{
				_guiStateInfos[index].panel.SetActive(active);
            }
		}
	}

	private void PostGUIStateChange(GUIState newGUIState)
	{
		//Debug.LogFormat("Post gui: {0}", newGUIState.ToString());
		switch(newGUIState)
		{
			case GUIState.GUIS_MENU:
				break;
			case GUIState.GUIS_GAME:
				break;
		}
		if(OnPostGUIStateChanged != null)
		{
			OnPostGUIStateChanged(newGUIState);
		}
	}

	#endregion Methods
}
