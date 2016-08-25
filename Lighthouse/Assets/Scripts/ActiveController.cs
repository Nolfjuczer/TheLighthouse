using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;

public enum ActiveSkillsEnum
{
    Flare = 0,
    Buoy = 1,
    Freeze = 2,
    SecondLight = 3,

	COUNT,
    NONE,
}

public class ActiveController : Singleton<ActiveController>
{
	#region Variables
	private ActiveSkillsEnum CurrentActive = ActiveSkillsEnum.NONE;

    public Action OnFreeze;
    public Action OnSecondLight;
    public Action OnSapper;

    [HideInInspector]
    public bool FlareAvailable;
    [HideInInspector]
    public bool BuoyAvailable;
    [HideInInspector]
    public bool FreezeAvailable;
    [HideInInspector]
    public bool SecondAvailable;

    private float _flareCooldown;
    private float _buoyCooldown;
    private float _freezeCooldown;
    private float _secondCooldown;

	private const float activeCooldownLength = 4.0f;

	public bool AnySkillActive
	{
		get
		{
			return CurrentActive != ActiveSkillsEnum.NONE;
		}
	}

	[System.Serializable]
	public struct ActiveInfo
	{
		public ActiveSkillsEnum activeType;
		public bool unlocked; //this is needed for unlocking abilities in subsequent levels
		[HideInInspector]
		public bool showSkillScreen;
		[System.NonSerialized]
		public float timer;
		public float cooldown;
		public const float initialCooldownLength = 4.0f;
		public System.Action OnActiveSkillUsed;

		public bool Availible
		{
			get
			{
				return timer >= cooldown;
			}
		}

		public void Reset()
		{
			if (unlocked)
			{
				timer = cooldown;
				GUIController.Instance.UpdateActiveGUI(activeType, 1.0f);
			} else {
				timer = 0.0f;
				GUIController.Instance.UpdateActiveGUI(activeType, 0.0f);
			}
		}

		public void UpdateActive(float deltaTime)
		{
			timer += deltaTime;
			float progres = Mathf.Clamp01(timer / cooldown);
			if( GUIController.Instance != null) GUIController.Instance.UpdateActiveGUI(activeType, progres);
            else
                Debug.Log("null :D");
		}

		public void Activate()
		{
			if(unlocked)
			{
				timer = 0.0f;
				if(OnActiveSkillUsed != null)
				{
					OnActiveSkillUsed();
				}
			}
		}
	}

	[SerializeField]
	private ActiveInfo[] _activeInfos = null;
	public ActiveInfo[] ActiveInfos { get { return _activeInfos; } }
	[HideInInspector]
	[SerializeField]
	private int _activeInfoCount = 0;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateActiveController();
	}
	protected override void Awake()
	{
		base.Awake();
	}

	void OnEnable()
    {
		//FlareAvailable = true;
		//BuoyAvailable = true;
		//FreezeAvailable = true;
		//SecondAvailable = true;
		//ResetActiveController();
    } 


	// Update is called once per frame
	void Update ()
	{
	    //ProcessInput();
		ProcessActives();
    }

	#endregion Monobehaviour Methods

	#region Methods

	private void ValidateActiveController()
	{
		ActiveInfo[] oldActiveInfos = _activeInfos;
		int oldActiveInfoCount = oldActiveInfos != null ? oldActiveInfos.Length : 0;
		_activeInfoCount = (int)ActiveSkillsEnum.COUNT;
		if(oldActiveInfoCount != _activeInfoCount)
		{
			_activeInfos = new ActiveInfo[_activeInfoCount];
		}
		for(int i = 0;i < _activeInfoCount;++i)
		{
			if(i < oldActiveInfoCount)
			{
				_activeInfos[i] = oldActiveInfos[i];
			} else {
				_activeInfos[i].unlocked = true;
				_activeInfos[i].cooldown = ActiveInfo.initialCooldownLength;
            }
			_activeInfos[i].activeType = (ActiveSkillsEnum)i;
			_activeInfos[i].showSkillScreen = _activeInfos[i].activeType == ActiveSkillsEnum.Buoy || _activeInfos[i].activeType == ActiveSkillsEnum.Flare;
        }
	}
	private void InitActiveController()
	{

	}
	private void DeInittActiveController()
	{

	}

	private void ResetActiveInfos()
	{
		for (int i = 0; i < _activeInfoCount; ++i)
		{
			_activeInfos[i].Reset();
		}
	}

	public void ResetActiveController()
	{
		ResetActiveInfos();
	}

	private void UpdateActives()
	{
		float deltaTime = Time.deltaTime;
		for(int i = 0;i < _activeInfoCount;++i)
		{
			if(_activeInfos[i].unlocked)
			{
				_activeInfos[i].UpdateActive(deltaTime);
			}
		}
	}

	public void ProcessInput()
    {
        if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME && InputManager.Instance.ThisFrameTouch && CurrentActive != ActiveSkillsEnum.NONE && !GameController.Instance.Light.Targeted)
		{
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
			UseActive(ray.origin, CurrentActive);
            CurrentActive = ActiveSkillsEnum.NONE;
		}
        else
        {
			if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME)
			{
				GameController.Instance.Light.ActiveOn = false;
			}
			if(InputManager.Instance.PreviousFrameTouch)
			{
				CurrentActive = ActiveSkillsEnum.NONE;
			}
		}
    }

	public void ProcessActives()
	{
		if(GameController.Instance.GameState == EGameState.InGame)
		{
			UpdateActives();
            if (CurrentActive != ActiveSkillsEnum.NONE && !InputManager.Instance.PreviousFrameTouch && InputManager.Instance.ThisFrameTouch)
			{
				Vector3 worldTouchPoint = GameController.Instance.MainCamera.ScreenToWorldPoint(InputManager.Instance.TouchPosition);
				PlaceActive(CurrentActive, worldTouchPoint);
			}
		}
	}

	public void ActivateSkill(ActiveSkillsEnum type)
	{
		int index = (int)type;
		if(index >= 0 && index < _activeInfoCount)
		{
			if(_activeInfos[index].unlocked && _activeInfos[index].Availible)
			{
				_activeInfos[index].Activate();
				if(_activeInfos[index].showSkillScreen)
				{
					GUIController.Instance.ShowSkillScreen(_activeInfos[index].activeType);
					CurrentActive = _activeInfos[index].activeType;
				} else {
					GUIController.Instance.HideActiveSkills();
				}
			}
		}
	}
	private void PlaceActive(ActiveSkillsEnum type, Vector3 position)
	{
		position.z = 0.0f;
		switch(type)
		{
			case ActiveSkillsEnum.Buoy:
				GameController.Instance.GetBuoy(position);
				break;
			case ActiveSkillsEnum.Flare:
				GameController.Instance.GetFlare(position);
				break;
		}
		_activeInfos[(int)CurrentActive].timer = 0.0f;
        GUIController.Instance.HideActiveSkills();
		CurrentActive = ActiveSkillsEnum.NONE;
	}
	public void CancelActive()
	{
		if(CurrentActive != ActiveSkillsEnum.NONE)
		{
			_activeInfos[(int)CurrentActive].Reset();
		}
		GUIController.Instance.HideActiveSkills();
	}

    private void UseActive(Vector3 position, ActiveSkillsEnum activeType)
    {
        //GUIController.Instance.CurrentActive.gameObject.SetActive(false);
        GameController.Instance.Light.ActiveOn = false;
        switch (activeType)
        {
            case ActiveSkillsEnum.Buoy:
                GameController.Instance.GetBuoy(position);
                StartCoroutine(BuoyCD());
                break;
            case ActiveSkillsEnum.Flare:
                GameController.Instance.GetFlare(position);
                StartCoroutine(FlareCD());
                break;
            case ActiveSkillsEnum.Freeze:
                OnFreeze();
                StartCoroutine(FreezeCD());
                break;
            case ActiveSkillsEnum.SecondLight:
                OnSecondLight();
                StartCoroutine(SecondCD());
                break;
        }
    }

    protected IEnumerator FlareCD()
    {
        FlareAvailable = false;
        _flareCooldown = 0f;
        while (_flareCooldown < activeCooldownLength)
        {
            _flareCooldown += Time.deltaTime;
            //GUIController.Instance.ActiveIcons[0].fillAmount = _flareCooldown/4f;
            yield return null;
        }
        FlareAvailable = true;
    }

    protected IEnumerator BuoyCD()
    {
        BuoyAvailable = false;
        _buoyCooldown = 0f;
        while (_buoyCooldown < activeCooldownLength)
        {
            _buoyCooldown += Time.deltaTime;
            //GUIController.Instance.ActiveIcons[1].fillAmount = _buoyCooldown / 4f;
            yield return null;
        }
        BuoyAvailable = true;
    }

    protected IEnumerator FreezeCD()
    {
        FreezeAvailable = false;
        _freezeCooldown = 0f;
        while (_freezeCooldown < activeCooldownLength)
        {
            _freezeCooldown += Time.deltaTime;
            //GUIController.Instance.ActiveIcons[2].fillAmount = _freezeCooldown / 4f;
            yield return null;
        }
        FreezeAvailable = true;
    }

    protected IEnumerator SecondCD()
    {
        SecondAvailable = false;
        _secondCooldown = 0f;
        while (_secondCooldown < activeCooldownLength)
        {
            _secondCooldown += Time.deltaTime;
            //GUIController.Instance.ActiveIcons[3].fillAmount = _secondCooldown / 4f;
            yield return null;
        }
        SecondAvailable = true;
    }

	#endregion Methods
}
