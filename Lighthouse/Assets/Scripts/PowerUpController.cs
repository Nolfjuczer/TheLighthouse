using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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
                Debug.Log("CaptureSlower");
                break;
            case PowerUpType.NDirectionSwapper:
                Debug.Log("DirectionSwapper");
                break;
            case PowerUpType.PCaptureBooster:
                Debug.Log("CaptureBooster");
                break;
            case PowerUpType.PLightEnlarger:
                Debug.Log("LightEnlarger");
                break;
        }
    }
}
