using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum PowerUpType
{
    PLightEnlarger = 0,
    PCaptureBooster,
    NDirectionSwapper,
    NCaptureSlower
}

public class PowerUpController : Singleton<PowerUpController>
{
    public GameObject PowerUpTemplate;
    public Sprite[] PowerUpSprites;

    public List<PowerUp> PossibleUps = new List<PowerUp>();

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

    public GameObject GetPowerUp(Vector3 position)
    {
        GameObject go;
        if (PossibleUps.Count > 0)
        {
            go = PossibleUps[0].gameObject;
            PossibleUps.RemoveAt(0);
            go.transform.position = position;
        }
        else
        {
            go = Instantiate(PowerUpTemplate,position,Quaternion.identity) as GameObject;
            
        }
        PowerUp pu = go.GetComponent<PowerUp>();
        if (pu == null) go.AddComponent<PowerUp>();
        pu.Type = (PowerUpType) Random.Range(0, 4);
        pu.GetComponent<SpriteRenderer>().sprite = PowerUpSprites[(int) pu.Type];
        go.SetActive(true);
        return go;
    }

    public void ReturnPowerUp(PowerUp powerUp)
    {
        PossibleUps.Add(powerUp);
        powerUp.gameObject.SetActive(false);
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
}
