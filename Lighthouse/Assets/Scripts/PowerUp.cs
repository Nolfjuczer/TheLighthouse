using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    public PowerUpType Type;

    public float Duration = 3f;
    public float ExistTime = 4f;
    public float CaptureTime = 1f;

    private Image _circleImage;
    private float _captureTimer;
    private float _existTimer;
    private bool _immune;
    private bool _capture;

    private Vector3 _scale = new Vector3(0.3f,0.3f,1f);

    public void OnEnable()
    {
        gameObject.transform.localScale = Vector3.zero;

        _captureTimer = 0f;
        _existTimer = 0f;

        _immune = true;

        _circleImage = GameController.Instance.GetProgressCricle(transform.position);
        _circleImage.enabled = false;

        StartCoroutine(StartImmunity());
    }

    public IEnumerator StartImmunity()
    {
        while (_existTimer < 1f)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, _scale, _existTimer);
            yield return null;
        }
        _immune = false;
        _existTimer = 0.0f;
    }

    public void Update()
    {
        _existTimer += Time.deltaTime;
        if(_existTimer > ExistTime) CleanPowerUp();
    }

    protected void CleanPowerUp()
    {
        GameController.Instance.ReturnProgressCircle(_circleImage);
		this.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            StartCoroutine("CaptureByLightHouse");
            StopCoroutine("UncaptureByLightHouse");
        }
    }

    public void OnTriggerExit2D(Collider2D col2D)
    {
        if (col2D.gameObject.layer == LayerMask.NameToLayer("Light"))
        {
            StartCoroutine("UncaptureByLightHouse");
            StopCoroutine("CaptureByLightHouse");
        }
    }

    private IEnumerator UncaptureByLightHouse()
    {
        while (_captureTimer > CaptureTime)
        {
            if (!_immune)
            {
                _captureTimer -= Time.deltaTime;
                _circleImage.fillAmount = _captureTimer / CaptureTime;                
            }
            yield return null;
        }
        _circleImage.enabled = false;
    }

    private IEnumerator CaptureByLightHouse()
    {
        _circleImage.enabled = true;
        while (_captureTimer < CaptureTime)
        {
            if (!_immune)
            {
                _captureTimer += Time.deltaTime;
                _circleImage.fillAmount = _captureTimer / CaptureTime;
            }
            yield return null;
        }

        PowerUpController.Instance.ApplyPowerUp(Type);
        CleanPowerUp();
    }
}
