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
                Debug.Log("CaptureSlower");
                break;
            case PowerUpType.NDirectionSwapper:
                _directionSwapperTimer = 0f;
                StartCoroutine(DirectionSwapperEnumerator());
                Debug.Log("DirectionSwapper");
                break;
            case PowerUpType.PCaptureBooster:
                _captureBoosterTimer = 0f;
                StartCoroutine(CaptureBoosterEnumerator());
                Debug.Log("CaptureBooster");
                break;
            case PowerUpType.PLightEnlarger:
                _lightEnlargerTimer = 0f;
                StartCoroutine(LightEnlargerEnumerator());
                Debug.Log("LightEnlarger");
                break;
        }
    }

    protected IEnumerator CaptureSlowerEnumerator()
    {
        CaptureSlowerBegin();
        while (_captureSlowerTimer < 5f)
        {
            _captureSlowerTimer += Time.deltaTime;
            yield return null;
        }
        CaptureSlowerEnd();
    }

    protected IEnumerator CaptureBoosterEnumerator()
    {
        CaptureBoosterBegin();
        while (_captureBoosterTimer < 5f)
        {
            _captureBoosterTimer += Time.deltaTime;
            yield return null;
        }
        CaptureBoosterEnd();
    }

    protected IEnumerator DirectionSwapperEnumerator()
    {
        DirectionSwapperBegin();
        while (_directionSwapperTimer < 5f)
        {
            _directionSwapperTimer += Time.deltaTime;
            yield return null;
        }
        DirectionSwapperEnd();
    }

    protected IEnumerator LightEnlargerEnumerator()
    {
        LightEnlargerBegin();
        while (_lightEnlargerTimer < 5f)
        {
            _lightEnlargerTimer += Time.deltaTime;
            yield return null;
        }
        LightEnlargerEnd();
    }
}
