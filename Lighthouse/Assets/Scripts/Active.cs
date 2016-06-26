using UnityEngine;
using System.Collections;

public abstract class Active : MonoBehaviour
{
	#region Variables

	public float Time = 4f;

	protected int layer_ship;

	#endregion Variables

	#region Monobehaviour Methods

	public virtual void Awake()
	{
		layer_ship = LayerMask.NameToLayer("Ship");
	}

	public virtual void OnEnable()
    {
        StartCoroutine(Burn());
    }

	#endregion Monobehaviour Methods

	#region Methods

	protected virtual IEnumerator Burn()
    {
        yield return new WaitForSeconds(Time);
    }

	#endregion Methods
}
