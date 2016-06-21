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

 //   [System.Serializable]
	//public struct ActiveInfo
	//{
	//	public ActiveInfo(ActiveSkillsEnum activeType, Vector3 position)
	//	{
	//		this.activeType = activeType;
	//		this.position = position;
	//	}
	//	public ActiveSkillsEnum activeType;
	//	public Vector3 position;
	//}

	//private List<ActiveInfo> _activeInfos = new List<ActiveInfo>();

    public void OnEnable()
    {
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
        if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME && InputManager.Instance.ThisFrameTouch && CurrentActive != ActiveSkillsEnum.None && !GameController.Instance.Light.Targeted)
		{
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
			UseActive(ray.origin, CurrentActive);
            CurrentActive = ActiveSkillsEnum.None;
		}
        else
        {
			if (GameLord.Instance.CurrentGameState == GameLord.GameState.GS_GAME)
			{
				GameController.Instance.Light.ActiveOn = false;
			}
			if(InputManager.Instance.PreviousFrameTouch)
			{
				CurrentActive = ActiveSkillsEnum.None;
			}
		}
    }



    private void UseActive(Vector3 position, ActiveSkillsEnum activeType)
    {
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
