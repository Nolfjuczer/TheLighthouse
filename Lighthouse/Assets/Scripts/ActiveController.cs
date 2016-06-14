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
    private Image _circleImage;
    private bool _unable;

	private Vector3 _selectedPosition = Vector3.zero;
	private float _activationLength = 0.0f;

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
        _circleImage = GameController.Instance.GetProgressCricle(transform.position);
        _circleImage.enabled = false;
        _circleImage.color = Color.green;
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

        if (InputManager.Instance.ThisFrameTouch && !GameController.Instance.Light.Targeted && CurrentActive != ActiveSkillsEnum.None)
        {
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

			if (_selectedPosition == Vector3.zero)
			{
				_selectedPosition = ray.origin;
			}

			Image circleImage = GameController.Instance.GetProgressCricle(transform.position);
			_activeInfos.Add(new ActiveInfo(CurrentActive, circleImage, _selectedPosition));
			CurrentActive = ActiveSkillsEnum.None;

		} else {
			_selectedPosition = Vector3.zero;
            _activeTimer = 0f;
            _circleImage.enabled = false;
            GameController.Instance.Light.ActiveOn = false;
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
        _circleImage.enabled = false;
        GameController.Instance.Light.ActiveOn = false;
        switch (activeType)
        {
            case ActiveSkillsEnum.Buoy:
                GameController.Instance.GetBuoy(position);
                break;
            case ActiveSkillsEnum.Flare:
                GameController.Instance.GetFlare(position);
                break;
            case ActiveSkillsEnum.Freeze:
                OnFreeze();
                break;
            case ActiveSkillsEnum.SecondLight:
                OnSecondLight();
                break;
        }
		
    }
}
