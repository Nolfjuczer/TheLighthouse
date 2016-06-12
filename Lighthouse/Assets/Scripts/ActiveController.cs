using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;

public enum ActiveSkillsEnum
{
    None,
    Flare,
    Buoy,
    Freeze,
    SecondLight,
    Sapper
}

public class ActiveController : Singleton<ActiveController>
{
    public ActiveSkillsEnum CurrentActive = ActiveSkillsEnum.None;
    public GameObject BuoyGameObject;
    public GameObject FlareGameObject;

    public Action OnFreeze;
    public Action OnSecondLight;
    public Action OnSapper;

    private float _activeTimer = 0f;
    private Image _circleImage;
    private Mine mine = null;
    private bool _unable;

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
        if (_unable && InputManager.Instance.PreviousFrameTouch) return;
        else _unable = false;
        if (InputManager.Instance.ThisFrameTouch && !GameController.Instance.Light.Targeted && CurrentActive != ActiveSkillsEnum.None)
        {
            Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(InputManager.Instance.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            GameController.Instance.SetCirclePosition(_circleImage, ray.origin);

            mine = null;
            if (hit.collider != null)
                mine = hit.collider.gameObject.layer == LayerMask.NameToLayer("Mine") ? hit.collider.gameObject.GetComponent<Mine>() : null;
            _circleImage.enabled = true;
            _activeTimer += Time.deltaTime;
            _circleImage.fillAmount = _activeTimer/2f;
            GameController.Instance.Light.ActiveOn = true;
            if (_activeTimer > 2f)
            {
                UseActive(ray.origin, mine);
                return;
            }            
        }
        else
        {
            _activeTimer = 0f;
            _circleImage.enabled = false;
            GameController.Instance.Light.ActiveOn = false;
        }
    }

    private void UseActive(Vector3 position, Mine mine)
    {
        _unable = true;
        _activeTimer = 0f;
        _circleImage.enabled = false;
        GameController.Instance.Light.ActiveOn = false;
        switch (CurrentActive)
        {
            case ActiveSkillsEnum.Buoy:
                Instantiate(BuoyGameObject, position, Quaternion.identity);
                break;
            case ActiveSkillsEnum.Flare:
                Instantiate(FlareGameObject, position, Quaternion.identity);
                break;
            case ActiveSkillsEnum.Freeze:
                OnFreeze();
                break;
            case ActiveSkillsEnum.SecondLight:
                OnSecondLight();
                break;
            case ActiveSkillsEnum.Sapper:
                if(mine != null) mine.DisarmMine();
                break;
        }
    }
}
