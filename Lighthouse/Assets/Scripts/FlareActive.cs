using UnityEngine;
using System.Collections;

public class FlareActive : Active
{
	#region Variables

	#endregion Variables

	#region Monobehaviour Methods

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.layer == layer_ship)
		{
			Ship tmpShip = other.gameObject.GetComponent<Ship>();
			if(tmpShip != null)
			{
				tmpShip.NotifyCapture(UnityEngine.Time.deltaTime * PowerUpController.Instance.CaptureTimeScale);
			}
		}
	}

	#endregion Monobehaviour Methods

	#region Methods
	protected override IEnumerator Burn()
    {
        yield return base.Burn();
        gameObject.SetActive(false);
    }

	#endregion Methods
}
