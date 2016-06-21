using UnityEngine;
using System.Collections;

public class InstanceLord : Singleton<InstanceLord>
{
	#region Variables

	public enum InstanceType
	{
		IT_CIRCLE = 0,
        IT_FLARE,
        IT_BUOY,
        IT_MINE,
        IT_FERRY,
        IT_FREIGHTER,
        IT_KEELBOAT,
        IT_MOTORBOAT,

		IT_COUNT
	}

	[System.Serializable]
	public struct InstanceTypeInfo
	{
		public InstanceType type;
		public GameObject prefab;
		public Transform parent;
	    public int InitialPoolSize;
		[HideInInspector]
		public Utility.MemberObjectPool pool;	
	}

	[SerializeField]
	private InstanceTypeInfo[] _instanceTypeInfos = null;
	[SerializeField, HideInInspector]
	private int _instanceTypeCount = 0;

	#endregion Variables

	#region Monobehaviour Methods

	void OnValidate()
	{
		ValidateInstanceManager();
	}

	protected override void Awake()
	{
		base.Awake();
		InitializeInstanceManager();
	}

	#endregion Monobehaviour Methods

	#region Methods

	private void ValidateInstanceManager()
	{
		_instanceTypeCount = (int)InstanceType.IT_COUNT;
		InstanceTypeInfo[] oldInstanceTypeInfos = _instanceTypeInfos;
		int oldInstanceTypeCount = oldInstanceTypeInfos != null ? oldInstanceTypeInfos.Length : 0;
		if(oldInstanceTypeCount != _instanceTypeCount)
		{
			_instanceTypeInfos = new InstanceTypeInfo[_instanceTypeCount];
		}
		for(int i = 0;i < _instanceTypeCount;++i)
		{
			if(i < oldInstanceTypeCount)
			{
				_instanceTypeInfos[i] = oldInstanceTypeInfos[i];
			}
			_instanceTypeInfos[i].type = (InstanceType)i;
		}
	}

	private void InitializeInstanceManager()
	{
		for(int i = 0;i < _instanceTypeCount;++i)
		{
			if(_instanceTypeInfos[i].prefab != null)
			{
				_instanceTypeInfos[i].pool = new Utility.MemberObjectPool(_instanceTypeInfos[i].prefab, _instanceTypeInfos[i].parent, _instanceTypeInfos[i].InitialPoolSize);
			}
		}
	}

	public GameObject GetInstance(InstanceType instanceType)
	{
		GameObject result = null;
		int index = (int)instanceType;
		if(index >= 0 && index < _instanceTypeCount)
		{
			result = _instanceTypeInfos[index].pool.GetPooledObject();
		}
		return result;
	}

	#endregion Methods
}
