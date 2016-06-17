using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;

public enum ActiveSkillsEnum
{
    None,
    Flare,
    Buoy,
    Freeze,
    SecondLight
}

public class ActiveController : Singleton<ActiveController>
{
    public ActiveSkillsEnum CurrentActive = ActiveSkillsEnum.None;

    public Action OnFreeze;
    public Action OnSecondLight;
    public Action OnSapper;

    private float _activeTimer = 0f;
    private bool _unable;

	private Vector3 _selectedPosition = Vector3.zero;
	private float _activationLength = 0.0f;

    public bool FlareAvailable;
    public bool BuoyAvailable;
    public bool FreezeAvailable;
    public bool SecondAvailable;

    private float _flareCooldown;
    private float _buoyCooldown;
    private float _freezeCooldown;
    private float _secondCooldown;

	private const float activeCooldownLength = 4.0f;

    [System.Serializable]
	public struct ActiveInfo
	{
		public ActiveInfo(ActiveSkillsEnum activeType, Image activeProgresImage,Vector3 position)
		{
			this.activeType = activeType;
			this.activeProgresImage = activeProgresImage;
			this.position = position;
			this.progres = 0.0f;
		}
		public ActiveSkillsEnum activeType;
		public Image activeProgresImage;
		public Vector3 position;
		public float progres;
	}

	private List<ActiveInfo> _activeInfos = new List<ActiveInfo>();

    public void OnEnable()
    {
        //_circleImage = GameController.Instance.GetProgressCricle(transform.position);
        //_circleImage.enabled = false;
        //_circleImage.color = Color.green;

        FlareAvailable = true;
        BuoyAvailable = true;
        FreezeAvailable = true;
        SecondAvailable = true;
    } 

	// Update is called once per frame
	void Update ()
	{
	    ProcessInput();
	}

    public void ProcessInput()
    {
		if (_unable && InputManager.Instance.PreviousFrameTouch)
		{
			return;
		}
        else
        {
			_unable = false;
		}

        if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME && InputManager.Instance.ThisFrameTouch && CurrentActive != ActiveSkillsEnum.None && !GameController.Instance.Light.Targeted)
		{
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

			if (_selectedPosition == Vector3.zero)
			{
				_selectedPosition = ray.origin;
			}

			GameObject circleGO = InstanceLord.Instance.GetInstance(InstanceLord.InstanceType.IT_CIRCLE);

			//Image circleImage = GameController.Instance.GetProgressCricle(transform.position);
			circleGO.SetActive(true);
			Image circleImage = circleGO.GetComponent<Image>();
			_activeInfos.Add(new ActiveInfo(CurrentActive, circleImage, _selectedPosition));
			CurrentActive = ActiveSkillsEnum.None;

		} else {
			//Debug.LogFormat("Declick");
			_selectedPosition = Vector3.zero;
            _activeTimer = 0f;
			if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME)
			{
				GameController.Instance.Light.ActiveOn = false;
			}
			if(InputManager.Instance.PreviousFrameTouch)
			{
				CurrentActive = ActiveSkillsEnum.None;
			}
		}

		float deltaTime = Time.deltaTime;
		for(int i = 0;i < _activeInfos.Count;++i)
		{
			ActiveInfo info = _activeInfos[i];

			info.progres += deltaTime;
			info.activeProgresImage.fillAmount = Mathf.Clamp01(info.progres / _activationLength);
			
			if(info.progres > _activationLength)
			{
				UseActive(info.position, info.activeType);
				info.activeProgresImage.gameObject.SetActive(false);
				_activeInfos.RemoveAt(i);
				--i;
			} else {
				_activeInfos[i] = info;
				info.activeProgresImage.color = new Color(0.0f,Mathf.Clamp01(info.progres / _activationLength),0.0f);
				GameController.Instance.SetCirclePosition(info.activeProgresImage,info.position);
				info.activeProgresImage.enabled = true;
				info.activeProgresImage.gameObject.SetActive(true);
            }
		}
    }



    private void UseActive(Vector3 position, ActiveSkillsEnum activeType)
    {
        _unable = true;
        _activeTimer = 0f;
        GUIController.Instance.CurrentActive.gameObject.SetActive(false);
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
            GUIController.Instance.ActiveIcons[0].fillAmount = _flareCooldown/4f;
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
            GUIController.Instance.ActiveIcons[1].fillAmount = _buoyCooldown / 4f;
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
            GUIController.Instance.ActiveIcons[2].fillAmount = _freezeCooldown / 4f;
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
            GUIController.Instance.ActiveIcons[3].fillAmount = _secondCooldown / 4f;
            yield return null;
        }
        SecondAvailable = true;
    }
}
